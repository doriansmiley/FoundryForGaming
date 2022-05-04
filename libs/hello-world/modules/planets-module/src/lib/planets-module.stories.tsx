import * as React from 'react';
import { storiesOf } from '@storybook/react';
import { PlanetsModule } from './planets-module';
import {PlanetsModule as PlanetsModule2} from './planets-module2.0';
import { StorybookProviders } from '@ffg/e2e-testing-providers';

storiesOf('PlanetsModule', module)
  .addDecorator((storyFn) => (
    <StorybookProviders>{storyFn()}</StorybookProviders>
  ))
  .add('Default view', () => <PlanetsModule />)
  .add('Planets 2.0', () => <PlanetsModule2 />);
