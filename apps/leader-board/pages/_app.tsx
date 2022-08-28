import { AppProps } from 'next/app';
import Head from 'next/head';
import './styles.css';

import { Provider } from '@foundry-for-gaming/common';
import { useContainer } from '../ioc';

function CustomApp({ Component, pageProps }: AppProps) {
  const container = useContainer();

  return (
    <>
      <Head>
        <title>Welcome to leader-board!</title>
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
