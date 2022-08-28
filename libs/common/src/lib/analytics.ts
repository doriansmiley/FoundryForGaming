import * as Unity from './unity';
import { JSONObject } from './unity';

export async function init(
  appId: string,
  userId: string,
  buildUrl = 'unity',
  abTestListener: (tests: any) => void = () =>
    console.log('abTestListener called'),
  soListener: (value: JSONObject) => void
) {
  try {
    console.log(`loading unity: ${new Date().getTime()}`);
    await Unity.Load(appId, userId, buildUrl, abTestListener, soListener);
    console.log(`loading unity: ${new Date().getTime()}`);
  } catch (e) {
    console.error(e);
  }
  console.log('unity loaded');
  if (globalThis.gpfReact.abtestSoid) {
    console.log('calling Unity.Sync abtestSoid');
    Unity.Sync(globalThis.gpfReact.abtestSoid);
  }
  if (globalThis.gpfReact.userSoid) {
    console.log('calling Unity.Sync userSoid');
    Unity.Sync(globalThis.gpfReact.userSoid);
  }
  if (globalThis.gpfReact.coinAdminSoid) {
    console.log('calling Unity.Sync userSoid');
    Unity.Sync(globalThis.gpfReact.coinAdminSoid);
  }
  console.log('returning done');

  return { done: true };
}

export function SendAnalytics(evtAttributes: JSONObject) {
  if (globalThis.gpfReact?.userSoid) {
    console.log(`Unity.Send sending analytics data name ${evtAttributes}`);
    Unity.Send(globalThis.gpfReact?.userSoid, 'AnalyticsUserSO+Message', {
      evtAttributes,
    });
  }
}

export function SetTest(testName: string, testValue: string) {
  if (globalThis.gpfReact.abtestSoid) {
    console.log(
      `Unity.Send sending test data name ${testName} value ${testValue}`
    );
    Unity.Send(globalThis.gpfReact.abtestSoid, 'ABTestsSO+SetTest', {
      name: testName,
      value: testValue,
    });
  }
}

export function RemoveTest(testName: string) {
  if (globalThis.gpfReact.abtestSoid) {
    Unity.Send(globalThis.gpfReact.abtestSoid, 'ABTestsSO+RemoveTest', {
      name: testName,
    });
  }
}

export function SetEntry(id, username, score) {
  if (globalThis.gpfReact.coinAdminSoid) {
    Unity.Send(globalThis.gpfReact.coinAdminSoid, 'CoinAdminSO+SetEntry', {
      id,
      username,
      score,
    });
  }
}

export function RemoveEntry(id) {
  if (globalThis.gpfReact.coinAdminSoid) {
    Unity.Send(globalThis.gpfReact.coinAdminSoid, 'CoinAdminSO+RemoveEntry', {
      id,
    });
  }
}

export async function GetLeaderboardEntries() {
  if (globalThis.gpfReact.coinAdminSoid) {
    console.log(`Unity.Send getting leaderboard`);
    Unity.Send(
      globalThis.gpfReact.coinAdminSoid,
      'CoinLeaderboardSO+GetEntries',
      {}
    );
  }
}
