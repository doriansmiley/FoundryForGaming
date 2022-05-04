export function loadUnity() {
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

  var canvas = document.querySelector("#unity-canvas")
  canvas.style.width = "100%"
  canvas.style.height = "100%"

  var script = document.createElement("script")
  script.src = loaderUrl
  var unity = null
  script.onload = () => {
    window["createUnityInstance"](canvas, config, progress => {})
      .then(unityInstance => {
        unity = unityInstance
      })
      .catch(message => {
        alert(message)
      })
  }
  document.body.appendChild(script)
}
