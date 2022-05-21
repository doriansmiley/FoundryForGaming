import * as Unity from './unity';
import { JSONObject } from './unity';

export async function init(
  appId: string,
  userId: string,
  buildUrl = 'unity',
  abTestListener: (tests: any) => void = () =>
    console.log('abTestListener called')
) {
  await Unity.Load(appId, userId, buildUrl, abTestListener, (so) => {
    if (so.ID?.id === window.gpfReact.abtestSoid) abTestListener(so.Tests);
  })
    .then(() => {
      if (window.gpfReact.abtestSoid) {
        Unity.Sync(window.gpfReact.abtestSoid);
      }
      if (window.gpfReact.userSoid) {
        Unity.Sync(window.gpfReact.userSoid);
      }

      window.gpfReact?.analytics?.resolve?.({ done: true });
    })
    .catch((message) => {
      console.error(message);
      window.gpfReact?.analytics?.reject?.(message);
    });
}

export function SendAnalytics(evtAttributes: JSONObject) {
  window.gpfReact?.analytics?.promise?.then(() => {
    if (window.gpfReact?.userSoid) {
      Unity.Send(window.gpfReact?.userSoid, 'AnalyticsUserSO+Message', {
        evtAttributes,
      });
    }
  });
}

export function SetTest(testName: string, testValue: string) {
  window.gpfReact?.analytics?.promise?.then(() => {
    if (window.gpfReact.abtestSoid) {
      Unity.Send(window.gpfReact.abtestSoid, 'ABTestsSO+SetTest', {
        name: testName,
        value: testValue,
      });
    }
  });
}

export function RemoveTest(testName: string) {
  window.gpfReact?.analytics?.promise?.then(() => {
    if (window.gpfReact.abtestSoid) {
      Unity.Send(window.gpfReact.abtestSoid, 'ABTestsSO+RemoveTest', {
        name: testName,
      });
    }
  });
}
