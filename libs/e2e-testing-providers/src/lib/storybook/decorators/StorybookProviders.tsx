import * as React from 'react';
import { Provider } from '@foundry-for-gaming/common';

import styles from './StorybookProviders.module.css';
import { useServiceClientsContainer } from '../../ioc';
import { Container, interfaces } from 'inversify';

export function StorybookProviders({
  children,
  additionalContainers = [],
}: {
  children: React.ReactNode;
  additionalContainers?: interfaces.Container[];
}) {
  const serviceClientsContainer = useServiceClientsContainer();

  const storybookContainer = additionalContainers
    ? Container.merge(
        serviceClientsContainer,
        new Container(), // placeholder, required parameter
        ...additionalContainers
      )
    : serviceClientsContainer;

  return (
    <Provider container={storybookContainer}>
      <div className={styles.moduleWrapper}>{children}</div>
    </Provider>
  );
}
