import * as React from 'react';
import {useMediator} from '@foundry-for-gaming/common';
import {EarthEvents, ListBehaviorsEvent} from '../components/earth/earth';

export function useMediators(ref: React.RefObject<HTMLDivElement | null>) {
  // Write your mediators here
  useMediator<ListBehaviorsEvent>(
    EarthEvents.LIST_BEHAVIORS,
    event => {
      let behaviors = '';
      event.payload.behaviors.forEach(
        behavior => (behaviors = behaviors.concat(`${behavior}\n\r`))
      );
      alert(behaviors);
    },
    ref
  );
}
