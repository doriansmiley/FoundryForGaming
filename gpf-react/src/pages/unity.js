window.gpfReact = {}
window.gpfReact.loading = {}
window.gpfReact.loading.promise = new Promise((resolve, reject) => {
  window.gpfReact.loading.reject = reject
  window.gpfReact.loading.resolve = resolve
})
window.gpfReact.queries = {}
window.gpfReact.queries.next = 0
window.gpfReact.queries.inFlight = {}
window.gpfReact.onSOSync = soJson => {
  let so = JSON.parse(soJson)
  window.gpfReact.soListener(so)
}
window.gpfReact.onQuerySuccess = (msgJson, queryId) => {
  let msg = JSON.parse(msgJson)
  var task = window.gpfReact.queries.inFlight[queryId]
  delete window.gpfReact.queries.inFlight[queryId]
  task.resolve(msg)
}
window.gpfReact.onQueryFailure = (reason, queryId) => {
  var task = window.gpfReact.queries.inFlight[queryId]
  delete window.gpfReact.queries.inFlight[queryId]
  task.reject(reason)
}
window.gpfReact.soListener = so => {}
export async function Load(soListener) {
  window.gpfReact.soListener = soListener
  var buildUrl = "unity"
  var loaderUrl = buildUrl + "/web.loader.js"
  var config = {
    dataUrl: buildUrl + "/web.data",
    frameworkUrl: buildUrl + "/web.framework.js",
    codeUrl: buildUrl + "/web.wasm",
    streamingAssetsUrl: "StreamingAssets",
    companyName: "GPF Demos",
    productName: "Matchmaker",
    productVersion: "0.1",
    showBanner: false,
  }

  // By default Unity keeps WebGL canvas render target size matched with
  // the DOM size of the canvas element (scaled by window.devicePixelRatio)
  // Set this to false if you want to decouple this synchronization from
  // happening inside the engine, and you would instead like to size up
  // the canvas DOM size and WebGL render target sizes yourself.
  // config.matchWebGLToCanvasSize = false;

  var canvas = document.createElement("CANVAS")
  canvas.style.width = "100%"
  canvas.style.height = "100%"
  var script = document.createElement("script")
  script.src = loaderUrl
  var unity = null
  script.onload = () => {
    window["createUnityInstance"](canvas, config, progress => {})
      .then(unityInstance => {
        unity = unityInstance
        {
          let message = { cmd: "SYNC", soid: "match_player/afterLoad" }
          let jsonMsg = JSON.stringify(message)
          unityInstance.SendMessage("GPFShim", "OnReactMessage", jsonMsg)
        }

        window.gpfReact.Sync = soid => {
          let message = { cmd: "SYNC", soid }
          let jsonMsg = JSON.stringify(message)
          unityInstance.SendMessage("GPFShim", "OnReactMessage", jsonMsg)
        }

        window.gpfReact.Unsync = soid => {
          let message = { cmd: "UNSYNC", soid }
          let jsonMsg = JSON.stringify(message)
          unityInstance.SendMessage("GPFShim", "OnReactMessage", jsonMsg)
        }

        window.gpfReact.Send = (soid, type, json) => {
          let message = { cmd: "SEND", soid, msgType: type, msgJson: json }
          let jsonMsg = JSON.stringify(message)
          unityInstance.SendMessage("GPFShim", "OnReactMessage", jsonMsg)
        }

        window.gpfReact.SendQuery = (soid, type, json, queryId) => {
          let message = {
            cmd: "SEND_QUERY",
            soid,
            msgType: type,
            msgJson: json,
            queryId: queryId,
          }
          let jsonMsg = JSON.stringify(message)
          unityInstance.SendMessage("GPFShim", "OnReactMessage", jsonMsg)
        }

        window.gpfReact.loading.resolve()
      })
      .catch(message => {
        console.error(message)
        window.gpfReact.loading.reject()
      })
  }
  document.body.appendChild(script)
  await window.gpfReact.loading
}

export function Sync(soid) {
  window.gpfReact.loading.promise.then(() => {
    window.gpfReact.Sync(soid)
  })
}

export function Unsync(soid) {
  window.gpfReact.loading.promise.then(() => {
    window.gpfReact.Unsync(soid)
  })
}

export function Send(soid, type, message) {
  let json = JSON.stringify(message)
  window.gpfReact.loading.promise.then(() => {
    window.gpfReact.Send(soid, type, json)
  })
}

export async function SendQuery(soid, type, message) {
  let json = JSON.stringify(message)
  let queryId = window.gpfReact.queries.next++
  let task = new Promise()
  window.gpfReact.queries[queryId] = task
  window.gpfReact.loading.promise.then(() => {
    window.gpfReact.SendQuery(soid, type, json, queryId)
  })
  var result = await task
  return result
}
