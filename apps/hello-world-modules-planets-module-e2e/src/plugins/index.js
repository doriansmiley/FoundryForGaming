const browserify = require('@cypress/browserify-preprocessor');
const cucumber = require('cypress-cucumber-preprocessor').default;

module.exports = (on, config) => {
  const options = {
    ...browserify.defaultOptions,
    typescript: require.resolve('typescript', { baseDir: config.projectRoot }),
  };

  on('file:preprocessor', cucumber(options));

  // for the Stripe form inputs type purpose
  // for the muting video ads  when test locally
  on('before:browser:launch', (browser, launchOptions) => {
    if (browser.name === 'chrome') {
      launchOptions.args.push('--disable-dev-shm-usage');
      launchOptions.args.push('--no-sandbox');
      launchOptions.args.push('--disable-site-isolation-trials');
      launchOptions.args.push('--mute-audio');
      return launchOptions;
    }
  });
};
