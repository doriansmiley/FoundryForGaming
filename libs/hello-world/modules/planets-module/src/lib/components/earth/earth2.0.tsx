import * as React from 'react';
import styles from './earth.module.scss';
import {
  EarthProps,
  EarthBehaviors,
  useLogic as coreLogic,
  ListBehaviorsEvent,
  EarthEvents,
} from './earth';
import {dispatch} from '@foundry-for-gaming/common';

export interface EarthPropsExtended extends EarthBehaviors {
  continents: number;
}

export function useLogic(props: EarthProps) {
  const oldProps = coreLogic(props) as EarthPropsExtended;
  const newProps = {...oldProps};
  newProps.continents = 7;
  newProps.onClick = e => {
    oldProps.onClick(e);
    dispatch<ListBehaviorsEvent>(e.target, [
      EarthEvents.LIST_BEHAVIORS,
      {
        behaviors: ['earth can also orbit the sun!'],
      },
    ]);
  };
  return newProps;
}
// Note, the original props are the same, but they don't need to be
// I just wanted to demonstrate that you can get a different shape
// without changing the original props type
export function Earth(props: EarthProps) {
  const {onClick, continents} = useLogic(props);
  return (
    <div className={styles['container']}>
      <h1>Welcome to Earth!</h1>
      <p>Earth is {props.au} astronomical units from the sun</p>
      <p>Earth has {continents} continents</p>
      <button onClick={e => onClick(e)}>Click to find out what I can do</button>
    </div>
  );
}

export default Earth;
