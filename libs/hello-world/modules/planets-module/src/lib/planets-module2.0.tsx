import * as React from 'react';
import { useInit as coreInit } from './planets-module';
import Grid from './components/layouts/grid/grid-earth2.0';

export function useInit() {
  const { ref } = coreInit();
  // add your custom init or override the init sequence
  // for example:
  /*
    * const ref = setUpRefs(); // from './planets-module'
      setUpMediators({ref}); // new function defined here
      return {ref};
    * */
  return { ref };
}

export function PlanetsModule() {
  const { ref } = useInit();

  return (
    <div ref={ref} data-testid="hello-world-module-id">
      <h2>Hello Planets 2.0</h2>
      <Grid></Grid>
    </div>
  );
}
