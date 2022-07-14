import {useContext, useMemo} from 'react';
import * as React from 'react';

import {interfaces} from 'inversify';

const InversifyContext = React.createContext<{
  container: interfaces.Container | null;
}>({
  container: null,
});

type Props = {
  children: any;
  container: interfaces.Container;
};

export const Provider: React.FC<Props> = props => {
  const value = useMemo(
    () => ({container: props.container}),
    [props.container]
  );

  return (
    <InversifyContext.Provider value={value}>
      {props.children}
    </InversifyContext.Provider>
  );
};

export function useInjection<T>(identifier: interfaces.ServiceIdentifier<T>) {
  const {container} = useContext(InversifyContext);

  if (!container) {
    throw new Error(
      'DI context not found. Is your component wrapped in <InversifyContext.Provider />?'
    );
  }
  return container.get<T>(identifier) as T;
}

export function withIoc<T>(getContainer: (props: T) => interfaces.Container) {
  return (Page: React.ComponentType<T>) => {
    return (props: T) => {
      const container = getContainer(props);
      return (
        <Provider container={container}>
          <Page {...props} />
        </Provider>
      );
    };
  };
}
