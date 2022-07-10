using NUnit.Framework;
using UnityEngine;
using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using GPFEditor;
using ServerObjects.Test;
using System.Collections.Generic;

namespace REST
{
    [TestFixture]
    sealed class ExperimentTest: ServerObjectTest
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

            var req = new RESTRequest
            {
                Headers = headers,
                Method = "GET"
            };

            var response = await SendREST(req, "?foo=bar&bar=foo");

            Debug.Log("response: " + response.Body);

            Assert.AreEqual(200, (int)response.httpStatusCode);
            Assert.AreEqual("2 experiments set", response.Body);

            await syncer.WaitFor(restSO, "experiments", FieldIs.WithCount(2) );
        }
    }
}
