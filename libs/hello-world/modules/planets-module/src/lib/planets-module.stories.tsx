import * as React from 'react';
import { storiesOf } from '@storybook/react';
import {Container} from 'inversify';
import { PlanetsModule } from './planets-module';
import {PlanetsModule as PlanetsModule2} from './planets-module2.0';
import {PlanetsModule as PlanetsModuleIoc} from './planets-module-ioc';
import { StorybookProviders } from '@ffg/e2e-testing-providers';
import {EARTH_ELEMENT, GRID_ELEMENT, MEDIATORS, useExperiment} from './ioc';
import Earth20 from './components/earth/earth2.0';
import {Grid} from './components/layouts/grid/grid-ioc';
import {useMediators} from './mediators/useMediators';
import {MutableRefObject} from "react";

storiesOf('PlanetsModule', module)
  .addDecorator(storyFn => {
    const container = new Container();
    container.bind(EARTH_ELEMENT).toFunction(
      useExperiment('experiment123', {
        '1': config => Earth20,
      })
    );
    container.bind(GRID_ELEMENT).toFunction(
      useExperiment('experiment123', {
        '1': config => Grid,
      })
    );
    container.bind(MEDIATORS).toFunction(
      useExperiment('experiment123', {
        '1':
          config =>
            ({ref}: {ref: MutableRefObject<any>}) =>
              useMediators(ref),
      })
    );

    return (
      <StorybookProviders additionalContainers={[container]}>
        {storyFn()}
      </StorybookProviders>
    );
  })
  .add('Default view', () => <PlanetsModule />)
  .add('Planets IOC', () => <PlanetsModuleIoc />)
  .add('Planets 2.0', () => <PlanetsModule2 />);
