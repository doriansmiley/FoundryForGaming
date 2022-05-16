using ServerObjects;
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

        await player1Task;
        await player2Task;
    }

    async Task PlayerActions(Syncer syncer)
    {
        var id = Registry.GetId<AnalyticsUserSO>();

        var player = await syncer.Sync(id);

        var evtAttributes = new Dictionary<string, string>();
        evtAttributes["action"] = "Test action";
        evtAttributes["element"] = "Test element";
        evtAttributes["label"] = "Test label";
        evtAttributes["interaction"] = "Test interaction";
        evtAttributes["screen"] = "Test screen";
        await syncer.SendWait(player, new AnalyticsUserSO.Message { evtAttributes = evtAttributes });

        await Delay(2000);
    }
}