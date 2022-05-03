import { Given, Then } from 'cypress-cucumber-preprocessor/steps';

Given('I visit PlanetsModule storybook', () => {
  cy.visit('/iframe.html?id=planetsmodule');
});

Then('I see PlanetsModule is displayed', () => {
  cy.findByTestId('planets-module-id').should('be.visible');
});
