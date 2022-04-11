import {api, data} from '@serverless/cloud';

import {v4 as uuidv4} from 'uuid';

const cached = new Map();

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
type Event = {
  uid: string;
  ts: number;
  sessionId: string;
  deviceID: string;
  platform: string;
  version: string;
  evtAttributes: {
    screen: string;
    element: string;
    label: string;
    interaction: string;
  };
  session: number;
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
  const events: Array<Event> = req.body.events;
  const promises = [];
  events.forEach((event) => {
    promises.push(data.set(`events:${event.ts}`, event));
  });
  await Promise.all(promises);
  res.send('Event added');
});

// Redirect to users endpoint
api.get('/*', (req, res) => {
  res.redirect('/events');
});
