using GPF.ServerObjects;
using System;
using System.Collections.Generic;

namespace ServerObjects
{
  [Register("experiment_rest")]
  public class ExperimentRestSO : ServerObject
  {
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
      var experimentsSoid = Registry.GetId<ExperimentSO>(ID.Suffix);

      switch (msg.Uri.AbsolutePath)
      {
        case "/set":
          if (msg.Body.Contains("=") && !msg.Body.Contains("=="))
          {
            var experiments = ParseNameVals(msg.Body);
            Send(msg.evt.source, new RESTResponseMessage
            {
              Body = $"{experiments.Count} experiments set",
            });
            Send(experimentsSoid, new ExperimentSO.ChangeExperiments
            {
              experiments = experiments,
            });
            return;
          }
          reason = "bad body";
          break;
        case "/reset":
          {
            var experiments = new Dictionary<string, string>();

            Send(msg.evt.source, new RESTResponseMessage
            {
              Body = "experiments reset",
            });
            Send(experimentsSoid, new ExperimentSO.ChangeExperiments
            {
              experiments = experiments,
            });
          }
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
