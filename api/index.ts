import {api, data, params} from '@serverless/cloud';
import S3 from 'aws-sdk/clients/s3';

import {v4 as uuidv4} from 'uuid';
import {GeoJSON, Point} from 'geojson';

const cached = new Map();
const s3 = new S3({credentials: {
    accessKeyId: params.AWS_ACCESS_KEY_ID,
    secretAccessKey: params.AWS_SECRET_ACCESS_KEY,
  }});

type Optional<T> = T | undefined;

type WithMeta<T> = T & { meta?: any };

type KeyValue<T> = {
  key: string;
  value: WithMeta<T>;
};

type GetResponse<T> = Optional<
  | {
      items: KeyValue<T>[];
      lastKey?: string;
      next?: () => Promise<GetResponse<T>>;
    }
  | T
>;

// the user type
export enum Actor {
  ANONYMOUS ='anonymous',
  REGISTERED_USER ='registeredUser',
  POWER_USER ='powerUser',
  ADMIN ='admin',
  SUPER_ADMIN ='superAdmin',
};

export enum Activities {
  REGISTRATION ='registration',
  LOGIN ='login',
  SHOP ='shop',
  CHECKOUT ='checkout',
  PURCHASE ='purchase',
  GAME_MATCH = 'gameMatch',
  GAME_PLAY = 'gamePlay',
  MESSAGING ='messaging',
  UPLOAD ='upload',
  DATA_STREAM ='dataStream',
}

// activities include state machines and controllers that
// can include experiments, ie registration, shop, checkout, etc
export type Activity = {
  type: Activities;
  variant?: Experiment;
}

export type Experiment = {
  id: string;
  value: string;
}

export type Demographics = {
  age: number;
  sex: 'm' | 'f';
  // ISO 639-2
  language: string;
}

export type Screen = {
  width: number;
  height: number;
  fps: number;
}

export type Event = {
  uid: string;
  ts: number;
  version: string;
  sessionId?: string;
  // OS
  platform?: string;
  // macbook, iPhone, etc
  device?: string;
  // browser version etc
  // TODO add to data pipeline and SO
  client?: string;
  // unique ID of the device
  deviceId?: string;
  // User's IP address
  // TODO add to data pipeline and SO
  ip?: string;
  // provided by geo-targeting services, ie CloudFlare, cloudFront ISO 3166
  // TODO add to data pipeline and SO
  countryName?: string;
  // TODO add to data pipeline and SO
  demographics?: Demographics;
  actor: Actor;
  // provided by geo-targeting services, ie CloudFlare, cloudFront
  // TODO add to data pipeline and SO
  location?: Point;
  evtAttributes: {
    // the web page or app screen
    screen?: string;
    // the ID of the element that triggered the interaction
    element?: string;
    // the label displayed to the user
    label?: string;
    // the type of interaction ie click, touch, scroll, etc or generic event like load, unload etc
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
  };
};

// Create GET route and return users
api.get('/events/:start/:limit', async (req, res) => {
  // Get users from Serverless Data
  const {items, lastKey, next} = await data.get(`events:>=${req.params.start}`,
      {
        limit: parseInt(req.params.limit),
      });
  const id = uuidv4();
  cached.set(id, {lastKey, next});
  // Return the results
  res.send({
    items,
    id,
  });
});

api.get('/next/:id', async (req, res) => {
  // Get users from Serverless Data
  try {
    const cachedResult: {
      lastKey: string;
      next: () => Promise<GetResponse<{ items; lastKey; next }>>;
    } = cached.get(req.params.id);
    const {items, lastKey, next} = await cachedResult.next();
    const id = uuidv4();
    // prevent overflow by removing cached keys once retrieved
    cached.delete(req.params.id);
    cached.set(id, {lastKey, next});
    // Return the results
    res.send({
      items,
      id,
    });
  } catch (e) {
    // TODO: not found
    res.status(400).send('Bad Request');
  }
});

api.post('/events', async (req, res) => {
  const date = new Date();
  const id = uuidv4();
  const events: Array<Event> = req.body.events;
  const promises = [];
  events.forEach((event) => {
    promises.push(data.set(`events:${event.ts}`, event));
  });
  await Promise.all(promises);
  // TODO add support for multi tenancy. Store retrieved keypairs in memory
  // to fetch a keypair not in memory use parameter store
  // create automation to insert client keys into parameter store as part of this API
  const s3Params = {
    Body: JSON.stringify(events),
    Bucket: params.BUCKET_NAME || 'foundry-for-gaming',
    Key: `${date.getUTCFullYear()}/${date.getUTCMonth()}/${date.getUTCDate()}/${date.getUTCHours()}/${id}.json`
  };
  await s3.putObject(s3Params).promise();
  res.send('Event added');
});

// Redirect to users endpoint
api.get('/*', (req, res) => {
  res.redirect('/events');
});
