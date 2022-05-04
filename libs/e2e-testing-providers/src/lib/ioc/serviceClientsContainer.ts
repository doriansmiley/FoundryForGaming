import {Container} from 'inversify';
import {factory, Factory, ServiceTypes} from '@foundry-for-gaming/common';
import {QueryClient} from 'react-query';

const queryClient = new QueryClient();

export function useServiceClientsContainer() {
  const clients = {
    [ServiceTypes.reactQuery]: () => {
      return queryClient;
    },
  };

  const container = new Container();

  container.bind<Factory>('serviceFactory').toFunction(factory(clients));

  return container;
}
