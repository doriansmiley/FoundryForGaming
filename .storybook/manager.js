import {addons} from '@storybook/addons';
import {create} from '@storybook/theming/create';

addons.setConfig({
  theme: create({
    base: 'light',
    brandTitle: 'Foundry for Gaming',
    brandUrl: 'https://github.com/doriansmiley/FoundryForGaming',
  }),
});
