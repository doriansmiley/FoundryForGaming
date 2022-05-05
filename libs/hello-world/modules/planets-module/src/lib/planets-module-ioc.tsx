import * as React from 'react';
import {setUpRefs, setUpMediators, initParams} from './planets-module';
import {useInjection} from '@foundry-for-gaming/common';
import {EARTH_ELEMENT, GRID_ELEMENT, MEDIATORS} from './ioc';
import {GridPropsType, Grid} from './components/layouts/grid/grid-ioc';
import {EarthProps, Earth} from './components/earth/earth';

export function useInit() {
  const ref = setUpRefs();
  return {ref};
}

export function PlanetsModule() {
  const {ref} = useInit();
  let GridElement: boolean | React.ComponentType<GridPropsType> = Grid;
  let EarthElement: boolean | React.ComponentType<EarthProps> = Earth;
  let mediators: boolean | (({ref}: initParams) => void) = setUpMediators;

  try {
    GridElement =
      useInjection<() => React.ComponentType<GridPropsType>>(GRID_ELEMENT)();
    EarthElement =
      useInjection<() => React.ComponentType<EarthProps>>(EARTH_ELEMENT)();
    mediators = useInjection<() => ({ref}: initParams) => void>(MEDIATORS)();
  } catch (e) {
    console.log('using default components');
  }

  // useExperiment will return false while loading
  if (!EarthElement || !GridElement || !mediators) {
    return <div>Solar system Pending</div>;
  }

  mediators({ref});
  const earth = <EarthElement au={1} />;

  return (
    <div ref={ref} data-testid="hello-world-module-id">
      <h2>Hello HelloWorldModule</h2>
      <GridElement earth={earth}></GridElement>
    </div>
  );
}
