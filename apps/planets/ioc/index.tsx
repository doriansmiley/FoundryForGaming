import * as React from 'react';
import { Container } from 'inversify';
import {
  PlanetsModule,
  PlanetsModule20,
} from '@foundry-for-gaming/planets-module';
import { useExperiment } from '@foundry-for-gaming/common';

export const PLANETS_MODULE = Symbol.for('planetsModule');
export const COLLAPSE_EXPERIMENT = Symbol.for('collapseExperiment');

export function useContainer() {
  const container = new Container();
  container.bind(PLANETS_MODULE).toFunction(
    useExperiment('experiment123', {
      '1': (config) => PlanetsModule,
      '2': (config) => PlanetsModule20,
    })
  );
  return container;
}
