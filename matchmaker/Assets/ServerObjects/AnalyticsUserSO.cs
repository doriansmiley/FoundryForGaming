using GPF.ServerObjects;
using ServerObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[DataStorePath("sync.analytics")]
[Syncable]
[Register("analytics")]
public class AnalyticsUserSO : ServerObject
{
    static TimeSpan SessionTimeout = TimeSpan.FromMinutes(5);

    public int CurrentSession;
    public DateTime LastMessageTime;

    public class Message : AnalyticsMessage
    {
        public Dictionary<string, string> evtAttributes = new Dictionary<string, string>();
    }
    [FromClient]
    void Handler(Message msg)
    {
        var sessionId = GetSessionId(msg.timestamp);
        Dictionary<string, object> evtAttributes = new Dictionary<string, object>();
        foreach (var kvp in msg.evtAttributes)
            evtAttributes.Add(kvp.Key, kvp.Value);
        RunTask(SendAnalytics, msg, evtAttributes, sessionId);
    }

    async Task SendAnalytics(Message message, Dictionary<string,object> evtAttributes, string sessionId)
    {
        await Foundry.SendAnalytics(message, evtAttributes, sessionId, PublicID, UpdateOrder, Env, Logger);
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
