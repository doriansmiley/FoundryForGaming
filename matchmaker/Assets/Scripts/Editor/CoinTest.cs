using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using NUnit.Framework;
using GPFEditor;

namespace CoinTests
{
    class CoinTest : ServerObjectTest
    {
        public const int PLAYERS = 1;
        public override async Task Run()
        {
            Task[] tasks = new Task[PLAYERS];

            for (int i = 0; i < PLAYERS; i++)
                tasks[i] = PlayerActions(i);

            // Wait for all the tasks to finish.
            await Task.WhenAll(tasks);
        }

        async Task PlayerActions(int playerNumber)
        {
            string username = $"Player {playerNumber}";

            Syncer syncer = CreateSyncer(username);

            var playerID = Registry.GetId<CoinPlayerSO>();

            CoinPlayerSO me = await syncer.Sync(playerID);

            CoinTopScoresSO topScores = await syncer.Sync(CoinTopScoresSO.MainSoid);

            // Send the username
            await syncer.SendWait(me, new CoinPlayerSO.SetUsername { username = username });

            // Start flipping
            for (int i = 0; i < playerNumber + 1; i++)
            {
                await syncer.SendWait(me, new CoinPlayerSO.Flip());

                // Wait for the coin flip to resolve to heads or tails
                await syncer.WaitFor(
                    me,
                    nameof(CoinPlayerSO.state),
                    FieldIs.EqualToItemIn(CoinPlayerSO.CoinState.HEADS, CoinPlayerSO.CoinState.TAILS)
                    );
            }
            await syncer.WaitFor(topScores, nameof(CoinTopScoresSO.TopScores), FieldIs.Initialized());
            Assert.AreNotEqual(0, topScores.TopScores.Length);
        }
    }
}