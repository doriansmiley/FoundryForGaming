export type JSONValue =
  | string
  | number
  | boolean
  | JSONObject
  | Array<JSONValue>;

export type JSONObject = {
  [key: string]: any;
};

export type gpfReact = {
  userId: string | undefined;
  appId: string | undefined;
  userSoid: string | undefined;
  abtestSoid: string | undefined;
  onSOSync: ((json: string) => void) | undefined;
  soListener: ((json: JSONObject) => void) | undefined;
  Sync: ((json: string) => void) | undefined;
  Unsync: ((json: JSONObject) => void) | undefined;
  Send: ((soid: string, type: string, json: string) => void) | undefined;
};

declare global {
  // eslint-disable-next-line no-var
  var gpfReact: gpfReact;
  // we have to disable type checks here as createUnityInstance is declared
  // in a .js file in the public folder of the app. If we don't disable the typechecks
  // we'll get a duplicate identifier error
  // eslint-disable-next-line @typescript-eslint/ban-ts-comment
  // @ts-ignore
  // eslint-disable-next-line no-var
  var createUnityInstance: (
    canvas: any,
    config: any,
    callback: (progress: number) => any
  ) => Promise<any>;
}

export async function Load(
  appId: string,
  userId: string,
  buildUrl: string,
  abTestListener: (tests: any) => void,
  soListener: (value: JSONObject) => void
) {
  return new Promise((resolve, reject) => {
    globalThis.gpfReact = {
      userSoid: undefined,
      abtestSoid: undefined,
      userId: undefined,
      appId: undefined,
      onSOSync: undefined,
      soListener: undefined,
      Send: undefined,
      Sync: undefined,
      Unsync: undefined,
    };

    globalThis.gpfReact.userId = userId;
    globalThis.gpfReact.appId = appId;
    globalThis.gpfReact.userSoid = 'analytics/' + userId;
    globalThis.gpfReact.abtestSoid = 'ab_tests/' + appId;

    // TODO refactor to call soListener, remove globalThis.gpfReact?.soListener?
    globalThis.gpfReact.onSOSync = (soJson) => {
      try {
        globalThis.gpfReact?.soListener?.(JSON.parse(soJson));
      } catch (e) {
        // TODO fix me with a proper logger like DEBUG
        console.log(e);
      }
    };
    if (globalThis.gpfReact?.soListener) {
      globalThis.gpfReact.soListener = soListener;
    }

    const loaderUrl = buildUrl + '/web.loader.js';
    const config = {
      dataUrl: buildUrl + '/web.data',
      frameworkUrl: buildUrl + '/web.framework.js',
      codeUrl: buildUrl + '/web.wasm',
      streamingAssetsUrl: 'StreamingAssets',
      companyName: 'GPF Demos',
      productName: 'Matchmaker',
      productVersion: '0.1',
      showBanner: false,
    };

    // By default Unity keeps WebGL canvas render target size matched with
    // the DOM size of the canvas element (scaled by window.devicePixelRatio)
    // Set this to false if you want to decouple this synchronization from
    // happening inside the engine, and you would instead like to size up
    // the canvas DOM size and WebGL render target sizes yourself.
    // config.matchWebGLToCanvasSize = false;

    const canvas = document.createElement('CANVAS');
    canvas.style.width = '100%';
    canvas.style.height = '100%';
    const script = document.createElement('script');
    script.src = loaderUrl;
    // TODO fix me with propper logger like Debug
    const progress = (progress: number) => {
      console.log(progress);
    };
    script.onload = () => {
      globalThis
        .createUnityInstance(canvas, config, progress)
        .then((unityInstance) => {
          globalThis.gpfReact.Sync = (soid) => {
            const message = { cmd: 'SYNC', soid };
            const jsonMsg = JSON.stringify(message);
            unityInstance.SendMessage('GPFShim', 'OnReactMessage', jsonMsg);
          };

          globalThis.gpfReact.Unsync = (soid) => {
            const message = { cmd: 'UNSYNC', soid };
            const jsonMsg = JSON.stringify(message);
            unityInstance.SendMessage('GPFShim', 'OnReactMessage', jsonMsg);
          };

          globalThis.gpfReact.Send = (soid, type, json) => {
            const message = { cmd: 'SEND', soid, msgType: type, msgJson: json };
            const jsonMsg = JSON.stringify(message);
            unityInstance.SendMessage('GPFShim', 'OnReactMessage', jsonMsg);
          };
          console.log('calling globalThis.gpfReact?.loading?.resolve?');
          resolve({ done: true });
        })
        .catch((message) => {
          // TODO fix me with proper logger like DEBUG
          console.error(message);
          reject(message);
        });
    };
    document.body.appendChild(script);
  });
}

export function Sync(soid: string) {
  globalThis.gpfReact?.Sync?.(soid);
}

export function Unsync(soid: JSONObject) {
  globalThis.gpfReact?.Unsync?.(soid);
}

export function Send(soid: string, type: string, message: JSONObject) {
  const json = JSON.stringify(message);
  globalThis.gpfReact?.Send?.(soid, type, json);
}
