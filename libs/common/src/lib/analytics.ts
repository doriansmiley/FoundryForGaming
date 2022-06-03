import * as Unity from './unity';
import { JSONObject } from './unity';

export async function init(
  appId: string,
  userId: string,
  buildUrl = 'unity',
  abTestListener: (tests: any) => void = () =>
    console.log('abTestListener called')
) {
  try {
    console.log(`loading unity: ${new Date().getTime()}`);
    await Unity.Load(appId, userId, buildUrl, abTestListener, (so) => {
      if (so.ID?.id === window.gpfReact.abtestSoid) abTestListener(so.Tests);
    });
    console.log(`loading unity: ${new Date().getTime()}`);
  } catch (e) {
    console.error(e);
  }
  console.log('unity loaded');
  if (window.gpfReact.abtestSoid) {
    console.log('calling Unity.Sync abtestSoid');
    Unity.Sync(window.gpfReact.abtestSoid);
  }
  if (window.gpfReact.userSoid) {
    console.log('calling Unity.Sync userSoid');
    Unity.Sync(window.gpfReact.userSoid);
  }
  console.log('returning done');

  return { done: true };
}

export function SendAnalytics(evtAttributes: JSONObject) {
  if (window.gpfReact?.userSoid) {
    console.log(`Unity.Send sending analytics data name ${evtAttributes}`);
    Unity.Send(window.gpfReact?.userSoid, 'AnalyticsUserSO+Message', {
      evtAttributes,
    });
  }
}

export function SetTest(testName: string, testValue: string) {
  if (window.gpfReact.abtestSoid) {
    console.log(
      `Unity.Send sending test data name ${testName} value ${testValue}`
    );
    Unity.Send(window.gpfReact.abtestSoid, 'ABTestsSO+SetTest', {
      name: testName,
      value: testValue,
    });
  }
}

export function RemoveTest(testName: string) {
  if (window.gpfReact.abtestSoid) {
    Unity.Send(window.gpfReact.abtestSoid, 'ABTestsSO+RemoveTest', {
      name: testName,
    });
  }
}
