using GPF.ServerObjects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerObjects
{
    public abstract class MatchAnalyticsSO : ServerObject
    {
        static TimeSpan SessionTimeout = TimeSpan.FromMinutes(5);

        // TODO: Wrap fields in a subclass so they don't clutter the SO
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
                // TODO: Send the analytics to a central SO, so they can be batched (maybe recorded/displayed in GPF)
                RunTask(SendAnalytics, analyticsMessage, currentState, sessionId);
            }
        }

        // TODO: Create a field attribute to add things to analytics instead. ex:
        // [Analytics("screen")]
        // public State state;
        protected abstract Dictionary<string, object> GetAnalyticsState();

        async Task SendAnalytics(AnalyticsMessage message, Dictionary<string, object> state, string sessionId)
        {
            var evtAttributes = GetMessageFields(message);
            foreach (var kvp in state)
            {
                if (!evtAttributes.ContainsKey(kvp.Key))
                    evtAttributes[kvp.Key] = kvp.Value;
            }

            await Foundry.SendAnalytics(message, evtAttributes, sessionId, PublicID, UpdateOrder, Env, Logger);
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
            result["screen"] = message.Screen;

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
