import * as React from 'react';
import { Layout, Header, Footer, Flex } from 'brainly-style-guide';
import Earth from '../../earth/earth';

export function Grid() {
  return (
    <Layout>
      <Header>Mercury | Venus | Earth</Header>
      <Flex>
        <Earth au={1}></Earth>
      </Flex>
      <Footer>Footer</Footer>
    </Layout>
  );
}

export default Grid;
