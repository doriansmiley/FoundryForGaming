import {curry} from 'ramda';

export type Factory = <Config, T>(key: string) => (config: Config) => T;

// IMPORTANT: Variadic types are unsupported: https://github.com/microsoft/TypeScript/issues/5453
// we've kept the generic T even though it's useless
export default curry(
  <T>(
    map: Record<string, (config: Record<string, unknown>) => unknown>,
    key: string,
    config: Record<string, unknown>
  ) => {
    return map[key](config) as T;
  }
);
