import * as React from 'react';
import {MutableRefObject, useRef} from 'react';
import {useMediators} from './mediators/useMediators';
import Grid from './components/layouts/grid/grid';

export type initParams = {
  ref: MutableRefObject<any>;
};

export function setUpMediators({ref}: initParams) {
  useMediators(ref);
}

export function setUpRefs() {
  return useRef(null);
}

export function useInit() {
  const ref = setUpRefs();
  setUpMediators({ref});
  return {ref};
}

export function PlanetsModule() {
  const {ref} = useInit();

  return (
    <div ref={ref} data-testid="hello-world-module-id">
      <h2>Hello Planets Module</h2>
      <Grid></Grid>
    </div>
  );
}
