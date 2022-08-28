import * as React from 'react';

import { PLANETS_MODULE } from '../ioc';
import styles from './index.module.scss';
import {
  init,
  JSONObject,
  RemoveTest,
  SendAnalytics,
  GetLeaderboardEntries,
  SetTest,
  useInjection,
} from '@foundry-for-gaming/common';
import { useEffect } from 'react';

export function Index() {
  function onSync(value: JSONObject) {
    // this global dync function requires introspection if the value to determine
    // what object was updates. Future version of sync will allow you to pass the
    // function as a param. globalThis.gpfReact?.userSoid and globalThis.gpfReact.abtestSoid
    // hold the ID value for analytics and A/B test objects respectively
    const id = value.ID.id;
    switch (id) {
      case globalThis.gpfReact.abtestSoid:
      case globalThis.gpfReact?.userSoid:
      case globalThis.gpfReact?.coinAdminSoid:
        console.log(`Index.onSync: ${JSON.stringify(value)}`);
        break;
      default:
        console.log(`Index.onSync: no matching object found for id: ${id}`);
    }
  }

  function onABTest(value: JSONObject) {
    console.log(`Index.onABTest: ${JSON.stringify(value)}`);
  }
  // note the empty deps array, this ensure this function is only called once
  useEffect(() => {
    const appId = 'main';
    const userId =
      appId + '-' + Math.floor(Math.random() * 100000000) + '-' + Date.now();
    // TODO pass in abTestListener and soListener and show updated on screen
    // when the new leaderboard SO is created hook up and show adding a new score
    // and show the scores changing in real time as the result of game play
    init(appId, userId, 'unity', onABTest, onSync)
      .then((result) => {
        console.log(`result: ${result}`);
        SendAnalytics({ action: 'test action' });
        SetTest('test1', 'C');
        SetTest('test2', 'test');
        SetTest('test3', 'deleteMe');
        GetLeaderboardEntries();
        RemoveTest('test3');
      })
      .catch((e) => {
        console.log(e);
      });
  }, []);
  const Planets = useInjection<() => React.ComponentType>(PLANETS_MODULE)();
  // if we are in a pending state planets will be false
  if (!Planets) {
    return <div>pending</div>;
  }
  return (
    <div className={styles.page}>
      <div className="wrapper">
        <div className="container">
          <Planets />
        </div>
      </div>
    </div>
  );
}

export default Index;
