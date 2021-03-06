import * as React from 'react';
import styles from './earth.module.scss';
import {dispatch} from '@foundry-for-gaming/common';

export interface EarthProps {
  au: number;
}

export interface EarthBehaviors {
  onClick: (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

export enum EarthEvents {
  LIST_BEHAVIORS = 'BrnEarthBehaviors',
}

export type ListBehaviorsEvent = [
  EarthEvents.LIST_BEHAVIORS,
  {
    behaviors: string[];
  }
];

export function useLogic(props: EarthProps) {
  const newProps = {...props} as EarthProps & EarthBehaviors;
  newProps.onClick = e => {
    dispatch<ListBehaviorsEvent>(e.target, [
      EarthEvents.LIST_BEHAVIORS,
      {
        behaviors: ['Earth can spin!'],
      },
    ]);
  };
  return newProps;
}

export function Earth(props: EarthProps) {
  const {onClick} = useLogic(props);
  return (
    <div className={styles['container']}>
      <h1>Welcome to Earth!</h1>
      <p>Earth is {props.au} astronomical units from the sun</p>
      <button onClick={e => onClick(e)}>Click to find out what I can do</button>
    </div>
  );
}

export default Earth;
