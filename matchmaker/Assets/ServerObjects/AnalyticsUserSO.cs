using GPF.ServerObjects;
using ServerObjects;
using System.Collections.Generic;

[DataStorePath("sync.analytics")]
[Syncable]
[Register("analytics")]
public class AnalyticsUserSO : AnalyticsSO
{
    public class Message : AnalyticsMessage
    {
        // TODO: Add relevant fields
    }
    [FromClient]
    void Handler(Message msg)
    {
    }

    protected override Dictionary<string, object> GetAnalyticsState()
    {
        return new Dictionary<string, object>();
    }
}
