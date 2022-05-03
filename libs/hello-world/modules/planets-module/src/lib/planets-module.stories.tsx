import * as React from 'react';
import { storiesOf } from '@storybook/react';
import { PlanetsModule } from './planets-module';
import { StorybookProviders } from '@brainly/e2e-testing-providers';

storiesOf('PlanetsModule', module)
  .addDecorator((storyFn) => (
    <StorybookProviders>{storyFn()}</StorybookProviders>
  ))
  .add('Default view', () => <PlanetsModule />);
