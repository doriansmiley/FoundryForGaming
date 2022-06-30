import random from 'random';
import {v4 as uuidv4} from 'uuid';

export default async (count) => {

  count = parseInt(count);

  if (count < 0 || count > 1000) {
    console.log(`count: ${count}`);
    throw new Error('count must be a number, greater than zero, and less than 1000');
  }

  let url = 'https://bright-app-v2ivb.cloud.serverless.com/events';

  const events = [];

  while(count > 0 ) {
    const event = {
      uid: uuidv4(),
      ts: Date.now(),
      sessionId: uuidv4(),
      deviceID: uuidv4(),
      device: 'iPhone 6',
      client: 'Unity v1234',
      platform: 'iOS',
      version: '15',
      actor: 'anonymous',
      demographics: {
        age: random.int(18,65),
        sex: ['m','f'][random.int(0,1)],
        language: ['eng','esp'][random.int(0,1)],
      },
      ip: `${random.int(0,50)}.${random.int(0,50)}.${random.int(0,50)}.${random.int(0,50)}`,
      countryName: 'United States',
      location: {
        type: 'Point',
        coordinates: [33.528323710650426, -117.71235319824873]
      },
      evtAttributes: {
        screen: ['screenA','screenB','screenC'][random.int(0,2)],
        element: ['ButtonA','ButtonB','ButtonC'][random.int(0,2)],
        label: 'ButtonText',
        interaction: 'touch',
        variant: [
          {
            id: '884835770',
            value: '1'
          },
          {
            id: '994835780',
            value: '2'
          }
        ][random.int(0,1)],
        activity: {
          type: ['registration', 'login', 'shop', 'checkout', 'purchase', 'match', 'play'][random.int(0,6)],
          experiment: [
            {
              id: '123454234',
              value: '0'
            },
            {
              id: '133334235',
              value: '1'
            }
          ][random.int(0,1)],
        },
        screenInfo: {
          width: 750,
          height: 1334,
          fps: 60,
        },
      }
    };
    events.push(event);
    count--;
  }

  await fetch(url,
    {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({events})
    });

  console.log('Events sent');
  return "Completed";
};
