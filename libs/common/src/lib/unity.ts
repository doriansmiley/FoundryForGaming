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
  analytics: {
    promise: Promise<any> | undefined;
    reject: ((e: any) => void) | undefined;
    resolve: ((result: { done: boolean }) => void) | undefined;
  };
  onSOSync: ((json: string) => void) | undefined;
  soListener: ((json: JSONObject) => void) | undefined;
  Sync: ((json: string) => void) | undefined;
  Unsync: ((json: JSONObject) => void) | undefined;
  Send: ((soid: string, type: string, json: string) => void) | undefined;
  loading: {
    // TODO fix me
    promise: Promise<any> | undefined;
    reject: ((e: any) => void) | undefined;
    resolve: ((result: { done: boolean }) => void) | undefined;
  };
};

declare global {
  interface Window {
    gpfReact: gpfReact;
    createUnityInstance: (
      canvas: any,
      config: any,
      callback: (progress: number) => any
    ) => Promise<any>;
  }
}

export async function Load(
  appId: string,
  userId: string,
  buildUrl: string,
  abTestListener: (tests: any) => void,
  soListener: (value: JSONObject) => void
) {
  window.gpfReact = {
    userSoid: undefined,
    abtestSoid: undefined,
    analytics: {
      promise: undefined,
      reject: undefined,
      resolve: undefined,
    },
    userId: undefined,
    appId: undefined,
    onSOSync: undefined,
    soListener: undefined,
    Send: undefined,
    Sync: undefined,
    Unsync: undefined,
    loading: {
      promise: undefined,
      reject: undefined,
      resolve: undefined,
    },
  };

  window.gpfReact.userId = userId;
  window.gpfReact.appId = appId;
  window.gpfReact.userSoid = 'analytics/' + userId;
  window.gpfReact.abtestSoid = 'ab_tests/' + appId;

  window.gpfReact.analytics = {
    reject: (e) => undefined,
    resolve: ({ done: boolean }) => undefined,
    promise: new Promise((resolve, reject) => {
      window.gpfReact.analytics.reject = reject;
      window.gpfReact.analytics.resolve = resolve;
    }),
  };

  window.gpfReact.loading = {
    reject: (e) => undefined,
    resolve: ({ done: boolean }) => undefined,
    promise: new Promise((resolve, reject) => {
      window.gpfReact.loading.reject = reject;
      window.gpfReact.loading.resolve = resolve;
    }),
  };

  window.gpfReact.onSOSync = (soJson) => {
    try {
      window.gpfReact?.soListener?.(JSON.parse(soJson));
    } catch (e) {
      // TODO fix me with a proper logger like DEBUG
      console.log(e);
    }
  };
  if (window.gpfReact?.soListener) {
    window.gpfReact.soListener = soListener;
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
    window['createUnityInstance'](canvas, config, progress)
      .then((unityInstance) => {
        window.gpfReact.Sync = (soid) => {
          const message = { cmd: 'SYNC', soid };
          const jsonMsg = JSON.stringify(message);
          unityInstance.SendMessage('GPFShim', 'OnReactMessage', jsonMsg);
        };

        window.gpfReact.Unsync = (soid) => {
          const message = { cmd: 'UNSYNC', soid };
          const jsonMsg = JSON.stringify(message);
          unityInstance.SendMessage('GPFShim', 'OnReactMessage', jsonMsg);
        };

        window.gpfReact.Send = (soid, type, json) => {
          const message = { cmd: 'SEND', soid, msgType: type, msgJson: json };
          const jsonMsg = JSON.stringify(message);
          unityInstance.SendMessage('GPFShim', 'OnReactMessage', jsonMsg);
        };

        window.gpfReact?.loading?.resolve?.({ done: true });
      })
      .catch((message) => {
        // TODO fix me with proper logger like DEBUG
        console.error(message);
        window.gpfReact.loading.reject?.(message);
      });
  };
  document.body.appendChild(script);
  await window.gpfReact.loading;
}

export function Sync(soid: string) {
  window.gpfReact?.loading?.promise?.then(() => {
    window.gpfReact?.Sync?.(soid);
  });
}

export function Unsync(soid: JSONObject) {
  window.gpfReact?.loading?.promise?.then(() => {
    window.gpfReact?.Unsync?.(soid);
  });
}

export function Send(soid: string, type: string, message: JSONObject) {
  const json = JSON.stringify(message);
  window.gpfReact?.loading?.promise?.then(() => {
    window.gpfReact?.Send?.(soid, type, json);
  });
}
