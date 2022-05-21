import * as React from 'react';
import { factory as variantFactory } from '@foundry-for-gaming/common';
import { MutableRefObject } from 'react';

export type ExperimentFactory = <T>(
  key: string,
  variants: {
    [key: string]: (
      config?: Record<string, unknown>
    ) =>
      | React.ComponentType<T>
      | (({ ref }: { ref: MutableRefObject<any> }) => void);
  },
  config?: Record<string, unknown>
) => () =>
  | React.ComponentType<T>
  | boolean
  | (({ ref }: { ref: MutableRefObject<any> }) => void);

function useMockExperiments(key: string) {
  switch (key) {
    case 'experiment123':
    default:
      return {
        variant: '1',
        pending: false,
      };
  }
}

export const useExperiment: ExperimentFactory =
  (key, variants, config?) => () => {
    const { variant, pending } = useMockExperiments(key);
    const factory = variantFactory(variants);
    if (pending) {
      return false;
    }
    return factory(variant)(config);
  };
