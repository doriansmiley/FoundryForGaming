using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using GPFEditor;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
[TestOf(typeof(AnalyticsUserSO))]
sealed class ReactTest : ServerObjectTest
{
    public override async Task Run()
    {
        var player1Task = PlayerActions(
            CreateSyncer("player1")
            );

        var player2Task = PlayerActions(
            CreateSyncer("player2")
            );

        var abTestConrollerTask = ABTestController(
            CreateSyncer("abTest_controller")
            );

        await player1Task;
        await player2Task;
        await abTestConrollerTask;
    }

    async Task PlayerActions(Syncer syncer)
    {
        var id = Registry.GetId<AnalyticsUserSO>();

        var abTests = await syncer.Sync(Registry.GetId<ABTestsSO>("main"));

        var player = await syncer.Sync(id);

        var evtAttributes = new Dictionary<string, string>();
        evtAttributes["action"] = "Test action";
        evtAttributes["element"] = "Test element";
        evtAttributes["label"] = "Test label";
        evtAttributes["interaction"] = "Test interaction";
        evtAttributes["screen"] = "Test screen";
        await syncer.SendWait(player, new AnalyticsUserSO.Message { evtAttributes = evtAttributes });

        await Delay(2000);

        await syncer.WaitFor(abTests, nameof(ABTestsSO.Tests), FieldIs.WithCount(1));
    }

    async Task ABTestController(Syncer syncer)
    {
        var abTests = await syncer.Sync(Registry.GetId<ABTestsSO>("main"));

        await syncer.SendWait(abTests, new ABTestsSO.SetTest { name = "test1", value = "A" });
        await syncer.SendWait(abTests, new ABTestsSO.SetTest { name = "test1", value = "B" });

        await Delay(2000);
    }
}