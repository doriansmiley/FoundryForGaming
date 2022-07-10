using GPF.ServerObjects;
using System.Collections.Generic;

namespace ServerObjects.Test
{
    [DataStorePath("sync.experiment")]
    [Syncable]
    [Register("experiment")]
    public class ExperimentSO : RESTBaseSO
    {
        public Dictionary<string, string> experiments = new Dictionary<string, string>();

        public void Handler(RESTRequestMessage msg)
        {
            experiments = GetParamsFromQueryString(msg.Uri.Query);
      
            Send(msg.evt.source, new RESTResponseMessage
            {
              Body = $"{experiments.Count} experiments set",
            });

        }
    }
}
