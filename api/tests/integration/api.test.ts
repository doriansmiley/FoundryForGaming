import {api, data} from '@serverless/cloud';
import {v4 as uuidv4} from 'uuid';

beforeAll(async () => {
  await data.set('events:1649619374413', {
    uid: uuidv4(),
    ts: Date.now(),
    sessionId: 1648758830,
    deviceID: 1648758830,
    platform: 'iOS',
    version: '0.0.01',
    evtAttributes: {
      screen: 'screenA',
      element: 'ButtonA',
      label: 'ButtonText',
      interaction: 'touch',
    },
    session: 'test',
  });
});

afterAll(async () => {
  await data.remove('events:1649619374413');
  await data.remove('events:1');
  await data.remove('events:2');
  await data.remove('events:3');
});

test('should create events', async () => {
  const result = await api.post('/events').invoke(
      {
        events: [
          {
            uid: uuidv4(),
            ts: Date.now(),
            sessionId: 1648758830,
            deviceID: 1648758830,
            platform: 'iOS',
            version: '0.0.01',
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
            },
            session: 'test',
          },
          {
            uid: uuidv4(),
            ts: 1,
            sessionId: 1648758830,
            deviceID: 1648758830,
            platform: 'iOS',
            version: '0.0.01',
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
            },
            session: 'test',
          },
          {
            uid: uuidv4(),
            ts: 2,
            sessionId: 1648758830,
            deviceID: 1648758830,
            platform: 'iOS',
            version: '0.0.01',
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
            },
            session: 'test',
          },
          {
            uid: uuidv4(),
            ts: 3,
            sessionId: 1648758830,
            deviceID: 1648758830,
            platform: 'iOS',
            version: '0.0.01',
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
            },
            session: 'test',
          },
        ],
      },
  );

  expect(result.body).toBe('Event added');
});

test('should return events', async () => {
  const {body} = await api.get(`/events/1649619374413/1`).invoke();
  expect(body).toHaveProperty('items');
  expect(body.items.length).toBe(1);
  expect(body.id.length).toBeGreaterThan(0);
  const nextResult = await api.get(`/next/${body.id}`).invoke();
  expect(nextResult.body).toHaveProperty('items');
  expect(nextResult.body.items.length).toBe(1);
  expect(nextResult.body.id.length).toBeGreaterThan(0);
});
