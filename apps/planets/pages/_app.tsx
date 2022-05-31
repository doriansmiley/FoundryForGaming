import { AppProps } from 'next/app';
import Head from 'next/head';

import { Provider } from '@foundry-for-gaming/common';
import { useContainer } from '../ioc';
import './styles.css';

function CustomApp({ Component, pageProps }: AppProps) {
  const container = useContainer();

  return (
    <>
      <Head>
        <title>Welcome to planets!</title>
      </Head>
      <main className="app">
        <Provider container={container}>
          <Component {...pageProps} />
        </Provider>
      </main>
    </>
  );
}

export default CustomApp;
