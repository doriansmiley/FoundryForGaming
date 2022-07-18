using NUnit.Framework;
using UnityEngine;
using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using GPFEditor;
using ServerObjects;
using System.Collections.Generic;

namespace REST
{
  [TestFixture]
  sealed class ExperimentTest : ServerObjectTest
  {
    public override async Task Run()
    {
      if (RestDisabled)
        Assert.Inconclusive("This test is only intended to run when REST is enabled");

      var experimentSoid = Registry.GetId<ExperimentSO>();
      var restSoid = Registry.GetId<ExperimentRestSO>(experimentSoid.Suffix);

      var syncer = CreateSyncer("default");
      ExperimentSO restSO = await syncer.Sync(experimentSoid);

      LogInfo("Binded to: " + experimentSoid);

      var headers = new Dictionary<string, List<string>>
          {
              {"SOID", new List<string> { restSoid } }
          };

      var badRequest1 = new RESTRequest
      {
        Headers = headers,
        Method = "GET"
      };

      // BAD Path-------------------------------------------------
      var badResponse1 = await SendREST(badRequest1);

      LogInfo("response: " + badResponse1.Body + " Status: " + badResponse1.httpStatusCode);

      Assert.AreEqual(400, (int)badResponse1.httpStatusCode);
      Assert.AreEqual("bad path", badResponse1.Body);

      // BAD Body-------------------------------------------------
      var badResponse2 = await SendREST(badRequest1, "set");

      LogInfo("response: " + badResponse2.Body + " Status: " + badResponse2.httpStatusCode);

      Assert.AreEqual(400, (int)badResponse2.httpStatusCode);
      Assert.AreEqual("bad body", badResponse2.Body);

      var goodRequest = new RESTRequest
      {
        Headers = headers,
        Method = "POST",
      };
      var goodResponse = await SendREST(goodRequest, "/reset");

      Assert.AreEqual(200, (int)goodResponse.httpStatusCode);

      Assert.AreEqual("experiments reset", goodResponse.Body);

      LogInfo("response: " + goodResponse.Body + " Status: " + goodResponse.httpStatusCode);

      // Good Payload should set-------------------------------------------------
      goodRequest.Body = "foo=bar&bar=foo";
      var goodResponse2 = await SendREST(goodRequest, "/set");

      Assert.AreEqual(200, (int)goodResponse2.httpStatusCode);
      Assert.AreEqual("2 experiments set", goodResponse2.Body);

      await syncer.WaitFor(restSO, "experiments", FieldIs.WithCount(2));
    }
  }
}
