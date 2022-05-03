import * as React from 'react';
import { useRef } from 'react';
import { useMediators } from './mediators/useMediators';

export function PlanetsModule() {
  const ref = useRef(null);

  useMediators(ref);

  return (
    <div ref={ref} data-testid="planets-module-id">
      <h2>Hello PlanetsModule</h2>
    </div>
  );
}
