using GPF.ServerObjects;
using System.Collections.Generic;

[DataStorePath("sync.ab_tests")]
[Syncable]
[Register("ab_tests")]
public class ABTestsSO : ServerObject
{
    public Dictionary<string, string> Tests = new Dictionary<string, string>();

    public class SetTest : ServerObjectMessage
    {
        public string name;
        public string value;
    }
    [FromClient]
    void Handler(SetTest msg)
    {
        if (string.IsNullOrEmpty(msg.name))
        {
            Logger.Warning($"name cannot be null or empty");
            return;
        }
        Tests[msg.name] = msg.value;
    }

    public class RemoveTest : ServerObjectMessage
    {
        public string name;
    }
    [FromClient]
    void Handler(RemoveTest msg)
    {
        if (string.IsNullOrEmpty(msg.name))
        {
            Logger.Warning($"name cannot be null or empty");
            return;
        }
        if (Tests.ContainsKey(msg.name))
            Tests.Remove(msg.name);
    }
}
