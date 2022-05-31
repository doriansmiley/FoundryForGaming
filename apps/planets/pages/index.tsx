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
    init(appId, userId)
      .then((result) => {
        console.log(`result: ${result}`);
        SendAnalytics({ action: 'test action' });
        SetTest('test1', 'A');
        SetTest('test1', 'B');
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
