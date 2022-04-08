import {api} from '@serverless/cloud';

test('should return users', async () => {
  const {body} = await api.get('/events/1649457528157/100').invoke();

  expect(body).toHaveProperty('items');
  expect(body.items.length).toBe(0);
});
