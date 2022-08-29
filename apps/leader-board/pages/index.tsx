import * as React from 'react';

import styles from './index.module.scss';
import {
  init,
  JSONObject,
  RemoveTest,
  SendAnalytics,
  GetLeaderboardEntries,
  SetEntry,
  SetTest,
  RemoveEntry,
  useInjection,
} from '@foundry-for-gaming/common';
import { useCallback, useEffect, useState } from 'react';

export function Index() {
  const [scores, setScores] = useState([]);
  const [topScores, setTopScores] = useState([]);
  const [selectedScore, setSelectedScore] = useState<
    { key: string; username: string; score: number } | undefined
  >();
  const deleteScore = useCallback((id) => RemoveEntry(id), []);
  const onChange = useCallback(
    (event) => {
      if (event.target.value !== selectedScore.score) {
        console.log(`calling SetEntry: ${selectedScore.key}`);
        SetEntry(selectedScore.key, selectedScore.username, event.target.value);
      }
      console.log('setSelectedScore');
      setSelectedScore({
        key: selectedScore.key,
        username: selectedScore.username,
        score: event.target.value,
      });
    },
    [selectedScore]
  );

  if (selectedScore) {
    // console.log('calling SetEntry');
    //SetEntry(selectedScore.key, selectedScore.username, selectedScore.score);
  }

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
      case globalThis.gpfReact?.topScoresSoid:
        console.log(`Index.onSync: ${JSON.stringify(value.TopScores)}`);
        setTopScores(value.TopScores);
        break;
      default:
        console.log(`Index.onSync: no matching object found for id: ${id}`);
        console.log(`Index.onSync: value: ${JSON.stringify(value)}`);
    }
  }

  function onABTest(value: JSONObject) {
    console.log(`Index.onABTest: ${JSON.stringify(value)}`);
  }
  // note the empty deps array, this ensure this function is only called once
  useEffect(() => {
    const appId = 'main';
    //const userId =
    //appId + '-' + Math.floor(Math.random() * 100000000) + '-' + Date.now();
    // CodeStrap user
    const userId =
      'coin_player/pub/Fr7Zfm0x4QdOG76v3pMFbP1P6K5EPCTXPOWMzJEaUrc_';
    const userId2 =
      'coin_player/pub/weaS6m2KI7rGWvz5OQrwnD17kO~DakduDVCxGBGT4Os_';
    const userId3 =
      'coin_player/pub/XQCVzXgGez3K-JP8iiStSSBtymLGo6G-ECH49xnxN0o_';
    // TODO pass in abTestListener and soListener and show updated on screen
    // when the new leaderboard SO is created hook up and show adding a new score
    // and show the scores changing in real time as the result of game play
    init(appId, userId, 'unity', onABTest, onSync)
      .then((result) => {
        console.log(`result: ${result}`);
        SendAnalytics({
          evtAttributes: { action: 'shop', variant: 'a' },
        });
        SetTest('test1', 'C');
        SetTest('test2', 'test');
        SetTest('test3', 'deleteMe');
        GetLeaderboardEntries((result) => {
          (
            result as Array<{ key: string; username: string; score: number }>
          ).sort((first, second) => second.score - first.score);
          setScores(
            result as Array<{ key: string; username: string; score: number }>
          );
          // TODO set state
        });
        SetEntry(userId, 'CodeStrap', 30);
        SetEntry(userId2, 'iMANTheFlipper', 20);
        SetEntry(userId3, 'DanTheManLevitan', 40);
        RemoveTest('test3');
      })
      .catch((e) => {
        console.log(e);
      });
  }, []);

  return (
    <div className={styles.page}>
      <h1>Top Scores</h1>
      <ol>
        {topScores?.map((score) => (
          <li key={score.username}>
            {score.username} - {score.score}
          </li>
        ))}
      </ol>
      <h1>Leaderboard</h1>
      <ol>
        {scores.map((score) => (
          <li key={score.key}>
            {score.username} -{' '}
            {selectedScore?.key === score.key && (
              <input
                type="number"
                value={selectedScore.score}
                onChange={(event) => onChange(event)}
              />
            )}
            {selectedScore?.key !== score.key && (
              <button onClick={(e) => setSelectedScore(score)}>
                {score.score}
              </button>
            )}
            - <button onClick={(e) => deleteScore(score.key)}>delete</button>
          </li>
        ))}
      </ol>
    </div>
  );
}

export default Index;
