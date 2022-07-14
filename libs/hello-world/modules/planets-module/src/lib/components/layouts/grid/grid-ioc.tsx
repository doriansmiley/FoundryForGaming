import * as React from 'react';
import { Layout, Header, Footer, Flex } from 'brainly-style-guide';
import { EarthProps } from '../../earth/earth';
import { ReactElement } from 'react';

export type GridPropsType = {
  earth: ReactElement<EarthProps>;
};

export function Grid(props: GridPropsType) {
  return (
    <Layout>
      <Header>Mercury | Venus | Earth</Header>
      <Flex>{props.earth}</Flex>
      <Footer>Footer</Footer>
    </Layout>
  );
}

export default Grid;
