using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using NUnit.Framework;
using GPFEditor;
using GPF.FieldOperations;
using System;

namespace CoinTests
{
  class CoinAdminTest : ServerObjectTest
  {
    public const int PLAYERS = 5;
    public override async Task Run()
    {
      Task[] tasks = new Task[PLAYERS];

      for (int i = 0; i < PLAYERS; i++)
        tasks[i] = PlayerActions(i);

      // Wait for all the tasks to finish.
      await Task.WhenAll(tasks);

      var adminSyncer = CreateSyncer("Admin");
      var adminSoid = Registry.GetId<CoinAdminSO>();
      var adminSo = await adminSyncer.Sync(adminSoid);
      var topScores = await adminSyncer.Sync(CoinTopScoresSO.MainSoid);

      var query = await adminSyncer.SendSessionMessage(CoinLeaderboardSO.MainSoid, new CoinLeaderboardSO.GetEntries());
      foreach(var kvp in query.scores)
      {
        adminSyncer.Send(adminSo, new CoinAdminSO.SetEntry { id = kvp.Key, username = kvp.Value.username, score = 100});
      }

      await adminSyncer.WaitFor(topScores, nameof(CoinTopScoresSO.TopScores), new TopScoreIs(100));

      foreach (var kvp in query.scores)
      {
        adminSyncer.Send(adminSo, new CoinAdminSO.RemoveEntry { id = kvp.Key });
      }

      await adminSyncer.WaitFor(topScores, nameof(CoinTopScoresSO.TopScores), FieldIs.WithCount(0));
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

    public class TopScoreIs : IFieldOperation
    {
      public int score;
      public TopScoreIs(int score)
      {
        this.score = score;
      }
      public bool Satisfied(object value, Type type)
      {
        var topScores = value as CoinLeaderboardSO.LeaderboardRow[];
        if (topScores == null || topScores.Length == 0)
          return false;
        return topScores[0].score == score;
      }
    }

  }
}
