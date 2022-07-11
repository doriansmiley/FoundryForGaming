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
    public SOID<ExperimentSO> RESTSOID = Registry.GetId<ExperimentSO>();

    public override async Task Run()
    {
      if (RestDisabled)
        Assert.Inconclusive("This test is only intended to run when REST is enabled");

      var syncer = CreateSyncer("default");
      ExperimentSO restSO = await syncer.Sync(RESTSOID);

      Debug.Log("Binded to: " + RESTSOID);

      var headers = new Dictionary<string, List<string>>
          {
              {"SOID", new List<string> { RESTSOID } }
          };

      var badRequest1 = new RESTRequest
      {
        Headers = headers,
        Method = "GET"
      };

      // BAD Path-------------------------------------------------
      var badResponse1 = await SendREST(badRequest1);

      Debug.Log("response: " + badResponse1.Body + " Status: " + badResponse1.httpStatusCode);

      Assert.AreEqual(400, (int)badResponse1.httpStatusCode);
      Assert.AreEqual("bad path", badResponse1.Body);

      // BAD Body-------------------------------------------------
      var badResponse2 = await SendREST(badRequest1, "set");

      Debug.Log("response: " + badResponse2.Body + " Status: " + badResponse2.httpStatusCode);

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

      Debug.Log("response: " + goodResponse.Body + " Status: " + goodResponse.httpStatusCode);

      // Good Payload should set-------------------------------------------------
      goodRequest.Body = "foo=bar&bar=foo";
      var goodResponse2 = await SendREST(goodRequest, "/set");

      Assert.AreEqual(200, (int)goodResponse2.httpStatusCode);
      Assert.AreEqual("2 experiments set", goodResponse2.Body);

      await syncer.WaitFor(restSO, "experiments", FieldIs.WithCount(2));
    }
  }
}
