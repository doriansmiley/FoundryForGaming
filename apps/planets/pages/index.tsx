import * as React from 'react';

import { PLANETS_MODULE } from '../ioc';
import styles from './index.module.scss';
import {
  init,
  RemoveTest,
  SendAnalytics,
  SetTest,
  useInjection,
} from '@foundry-for-gaming/common';
import { useEffect } from 'react';

export function Index() {
  // note the empty deps array, this ensure this function is only called once
  useEffect(() => {
    const appId = 'main';
    const userId =
      appId + '-' + Math.floor(Math.random() * 100000000) + '-' + Date.now();
    // TODO pass in abTestListener and soListener and show updated on screen
    // when the new leaderboard SO is created hook up and show adding a new score
    // and show the scores changing in real time as the result of game play
    init(appId, userId)
      .then((result) => {
        console.log(`result: ${result}`);
        SendAnalytics({ action: 'test action' });
        SetTest('test1', 'C');
        SetTest('test2', 'test');
        SetTest('test3', 'deleteMe');
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
