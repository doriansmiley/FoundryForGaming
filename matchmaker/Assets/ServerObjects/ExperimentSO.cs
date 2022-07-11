using GPF.ServerObjects;
using System;
using System.Collections.Generic;

namespace ServerObjects
{
  [DataStorePath("sync.experiment")]
  [Syncable]
  [Register("experiment")]
  public class ExperimentSO : ServerObject
  {
    public Dictionary<string, string> experiments = new Dictionary<string, string>();

    private static Dictionary<string, string> ParseNameVals(string queryString)
    {
      var myParams = new Dictionary<string, string>();
      var pairParts = queryString.Split('&');
      foreach (var pair in pairParts)
      {
        try
        {
          var parts = pair.Split('=');
          myParams[parts[0]] = parts[1];
        }
        catch (Exception)
        {

        }
      }
      return myParams;
    }

    public void Handler(RESTRequestMessage msg)
    {
      string reason = "bad path";

      switch (msg.Uri.AbsolutePath)
      {
        case "/set":
          if (msg.Body.Contains("=") && !msg.Body.Contains("=="))
          {
            experiments = ParseNameVals(msg.Body);
            Send(msg.evt.source, new RESTResponseMessage
            {
              Body = $"{experiments.Count} experiments set",
            });
            return;
          }
          reason = "bad body";
          break;
        case "/reset":
          experiments = new Dictionary<string, string>();

          Send(msg.evt.source, new RESTResponseMessage
          {
            Body = "experiments reset",
          });
          return;
      }

      // Not handled
      Send(msg.evt.source, new RESTResponseMessage
      {
        httpStatusCode = System.Net.HttpStatusCode.BadRequest,
        Body = reason
      }); 

    }
  }
}
