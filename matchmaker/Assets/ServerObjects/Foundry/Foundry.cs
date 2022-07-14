using GPF.ServerObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerObjects
{
    public static class Foundry
    {
        public const string URL = "https://bright-app-v2ivb.cloud.serverless.com/events";

        public class Event
        {
            public const string SchemaVersion = "0.0.01";

            public string uid;
            public string ts;
            public string sessionId;
            public string deviceID;
            public string platform;
            public string version = SchemaVersion;
            public string language;
            public string device;
            public string resolution;
            public Dictionary<string, object> evtAttributes = new Dictionary<string, object>();
        }

        class ServerMessage
        {
            public List<Event> events = new List<Event>();
        }

        public static async Task SendAnalytics(AnalyticsMessage message, Dictionary<string, object> evtAttributes, string sessionId, string PublicID, ulong UpdateOrder, ServerObjectEnv Env, IGPFLogger Logger)
        {
            var uid = $"{PublicID}-{UpdateOrder}";
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var secondsSinceEpoch = Convert.ToInt64((message.timestamp - epoch).TotalMilliseconds);
            var e = new Event
            {
                uid = uid,
                ts = secondsSinceEpoch.ToString(),
                sessionId = sessionId,
                deviceID = message.DeviceId,
                platform = message.Platform,
                language = message.Language,
                device = message.DeviceName,
                resolution = message.Resolution,
            };
            e.evtAttributes = evtAttributes;

            await SendAnalytics(e, Env, Logger);
        }

        static async Task SendAnalytics(Event e, ServerObjectEnv Env, IGPFLogger Logger)
        {
            var serverMessage = new ServerMessage();
            serverMessage.events.Add(e);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(serverMessage);

            if (Env[ServerObjectEnv.Included.BACKEND] == "Simulation")
            {
                Logger.Log("Event: " + json);
            }
            else
                await SendRequest(json);
        }

        static async Task SendRequest(string json)
        {
            var uri = new Uri(URL);

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Headers.Add("Accept", "application/json");

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            message.Content = data;

            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                response = await client.SendAsync(message);
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
