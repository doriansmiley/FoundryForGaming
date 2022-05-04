import * as React from 'react';
import styles from './earth.module.scss';

export interface EarthProps {
  au: number;
}

export interface EarthBehaviors {
  onClick: () => void;
}

export function useLogic(props: EarthProps) {
  const newProps = {...props} as EarthProps & EarthBehaviors;
  newProps.onClick = () => {
    alert('earth can spin!');
  };
  return newProps;
}

export function Earth(props: EarthProps) {
  const {onClick} = useLogic(props);
  return (
    <div className={styles['container']}>
      <h1>Welcome to Earth!</h1>
      <p>Earth is {props.au} astronomical units from the sun</p>
      <button onClick={onClick}>Click to find out what I can do</button>
    </div>
  );
}

export default Earth;
