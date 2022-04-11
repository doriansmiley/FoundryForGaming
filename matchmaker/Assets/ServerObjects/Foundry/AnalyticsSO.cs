using GPF.ServerObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerObjects
{
    public abstract class AnalyticsSO : ServerObject
    {
        public const string URL = "TBD";
        static TimeSpan SessionTimeout = TimeSpan.FromMinutes(5);

        class Event
        {
            public const string SchemaVersion = "0.0.01";

            public string uid;
            public string ts;
            public string sessionId;
            public string deviceID;
            public string platform;
            public string version = SchemaVersion;
            public List<Dictionary<string, object>> evtAttributes = new List<Dictionary<string, object>>();
        }

        public int CurrentSession;
        public DateTime LastMessageTime;

        protected override void PreMessage(ServerObjectMessage message)
        {
            base.PreMessage(message);
            var analyticsMessage = message as AnalyticsMessage;
            if (analyticsMessage != null)
            {
                var sessionId = GetSessionId(analyticsMessage.timestamp);
                var currentState = GetAnalyticsState();
                RunTask(SendAnalytics, analyticsMessage, currentState, sessionId);
            }
        }

        protected abstract Dictionary<string, object> GetAnalyticsState();

        async Task SendAnalytics(AnalyticsMessage message, Dictionary<string, object> state, string sessionId)
        {
            var uid = $"{PublicID}-{UpdateOrder}";
            var eventVariables = GetMessageFields(message);
            var e = new Event
            {
                uid = uid,
                ts = message.timestamp.ToFileTimeUtc().ToString(),
                sessionId = sessionId,
                deviceID = message.DeviceId,
                platform = message.Platform,
            };
            foreach (var kvp in state)
            {
                if (!eventVariables.ContainsKey(kvp.Key))
                    eventVariables[kvp.Key] = kvp.Value;
            }
            e.evtAttributes.Add(eventVariables);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(e);

            if (URL == "TBD")
                Logger.Log("Event: " + json);
            else
                await SendRequest(json);
        }

        async Task SendRequest(string json)
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

        Dictionary<string, object> GetMessageFields(AnalyticsMessage message)
        {
            var result = new Dictionary<string, object>();
            var builtinFields = typeof(AnalyticsMessage).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var fields = message.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var builtinFieldNames = new HashSet<string>();
            foreach (var field in builtinFields)
            {
                builtinFieldNames.Add(field.Name);
            }
            foreach (var field in fields)
            {
                if (builtinFieldNames.Contains(field.Name))
                    continue;
                result.Add(field.Name, field.GetValue(message));
            }
            result["action"] = message.GetType().Name;
            result["element"] = message.Element;
            result["label"] = message.Label;
            result["interaction"] = message.Interaction;

            return result;
        }

        string GetSessionId(DateTime now)
        {
            if (now - LastMessageTime > SessionTimeout)
            {
                CurrentSession++;
            }
            LastMessageTime = now;
            return $"{PublicID}-{CurrentSession}";
        }
    }
}
