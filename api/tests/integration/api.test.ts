import {api, data} from '@serverless/cloud';
import {v4 as uuidv4} from 'uuid';
import {Activities, Actor} from '../../index';

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

/*
            uid: uuidv4(),
            ts: Date.now(),
            sessionId: 1648758830,
            deviceID: 1648758830,
            device: 'iPhone',
            client: 'Chrome/101.0.4951.54',
            platform: 'iOS',
            version: '0.0.01',
            actor: Actor.ANONYMOUS,
            demographics: {
              age: 45,
              sex: 'm',
              language: 'eng',
            },
            ip: '0.0.0.1',
            countryName: 'United States',
            location: {
              type: 'Point',
              coordinates: [33.528323710650426, -117.71235319824873]
            },
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
              variant: {
                  id: '994835780',
                  value: '1'
              },
              activity: {
                type: Activities.DATA_STREAM,
                experiment: {
                  id: '123454234',
                  value: '0'
                }
              },
              screenInfo: {
                width: 1440,
                height: 900,
                fps: 60,
              },
            }

  // provided by geo-targeting services, ie CloudFlare, cloudFront
  // TODO add to data pipeline and SO
  location?: Point;
  evtAttributes: {
    // TODO add to data pipeline and SO
    type: string;
    // TODO add to data pipeline and SO
    value?: string;
    // variants represent different version of UI elements rendered as part of an experiment
    // TODO add to data pipeline and SO
    variant?: Experiment;
    // TODO add to data pipeline and SO
    activity?: Activity;
    // TODO implement `${screen.width} x ${screen.height} @ ${fps}Hz`
    // get the Hz using http://jsfiddle.net/rBGPk/
    screenInfo?: Screen;
  }
* */

test('should create events', async () => {
  const result = await api.post('/events').invoke(
      {
        events: [
          {
            uid: uuidv4(),
            ts: Date.now(),
            sessionId: 1648758830,
            deviceID: 1648758830,
            device: 'iPhone',
            client: 'Chrome/101.0.4951.54',
            platform: 'iOS',
            version: '0.0.01',
            actor: Actor.ANONYMOUS,
            demographics: {
              age: 45,
              sex: 'm',
              language: 'eng',
            },
            ip: '0.0.0.1',
            countryName: 'United States',
            location: {
              type: 'Point',
              coordinates: [33.528323710650426, -117.71235319824873]
            },
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
              variant: {
                id: '994835780',
                value: '1'
              },
              activity: {
                type: Activities.DATA_STREAM,
                experiment: {
                  id: '123454234',
                  value: '0'
                }
              },
              screenInfo: {
                width: 1440,
                height: 900,
                fps: 60,
              },
            }
          },
          {
            uid: uuidv4(),
            ts: Date.now(),
            sessionId: 1648758830,
            deviceID: 1648758830,
            device: 'iPhone',
            client: 'Chrome/101.0.4951.54',
            platform: 'iOS',
            version: '0.0.01',
            actor: Actor.ANONYMOUS,
            demographics: {
              age: 45,
              sex: 'm',
              language: 'eng',
            },
            ip: '0.0.0.1',
            countryName: 'United States',
            location: {
              type: 'Point',
              coordinates: [33.528323710650426, -117.71235319824873]
            },
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
              variant: {
                id: '994835780',
                value: '1'
              },
              activity: {
                type: Activities.DATA_STREAM,
                experiment: {
                  id: '123454234',
                  value: '0'
                }
              },
              screenInfo: {
                width: 1440,
                height: 900,
                fps: 60,
              },
            }
          },
          {
            uid: uuidv4(),
            ts: Date.now(),
            sessionId: 1648758830,
            deviceID: 1648758830,
            device: 'iPhone',
            client: 'Chrome/101.0.4951.54',
            platform: 'iOS',
            version: '0.0.01',
            actor: Actor.ANONYMOUS,
            demographics: {
              age: 45,
              sex: 'm',
              language: 'eng',
            },
            ip: '0.0.0.1',
            countryName: 'United States',
            location: {
              type: 'Point',
              coordinates: [33.528323710650426, -117.71235319824873]
            },
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
              variant: {
                id: '994835780',
                value: '1'
              },
              activity: {
                type: Activities.DATA_STREAM,
                experiment: {
                  id: '123454234',
                  value: '0'
                }
              },
              screenInfo: {
                width: 1440,
                height: 900,
                fps: 60,
              },
            }
          },
          {
            uid: uuidv4(),
            ts: Date.now(),
            sessionId: 1648758830,
            deviceID: 1648758830,
            device: 'iPhone',
            client: 'Chrome/101.0.4951.54',
            platform: 'iOS',
            version: '0.0.01',
            actor: Actor.ANONYMOUS,
            demographics: {
              age: 45,
              sex: 'm',
              language: 'eng',
            },
            ip: '0.0.0.1',
            countryName: 'United States',
            location: {
              type: 'Point',
              coordinates: [33.528323710650426, -117.71235319824873]
            },
            evtAttributes: {
              screen: 'screenA',
              element: 'ButtonA',
              label: 'ButtonText',
              interaction: 'touch',
              variant: {
                id: '994835780',
                value: '1'
              },
              activity: {
                type: Activities.DATA_STREAM,
                experiment: {
                  id: '123454234',
                  value: '0'
                }
              },
              screenInfo: {
                width: 1440,
                height: 900,
                fps: 60,
              },
            }
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
