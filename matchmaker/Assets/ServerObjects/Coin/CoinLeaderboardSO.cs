using GPF.ServerObjects;
using System;
using System.Collections.Generic;
using System.Linq;

// This class is not [Syncable]
// Even with the id, users cannot access its contents
// No Handlers will have the [FromClient] on them
[Register("coin_leaderboard")]
public class CoinLeaderboardSO : ServerObject
{
  [ExpandData]
  public class LeaderboardRow
  {
    public string username;
    public int score;
  }

  // Use the same SOID to create a singleton
  public static SOID<CoinLeaderboardSO> MainSoid = Registry.GetId<CoinLeaderboardSO>("main");

  public const int TOP_SCORE_RETURN_LENGTH = 3;

  public const double MAX_UPDATE_RATE = 0.3;

  public Dictionary<SOID<CoinPlayerSO>, LeaderboardRow> scores = new Dictionary<SOID<CoinPlayerSO>, LeaderboardRow>();

  public bool scoresChanged;

  public DateTime lastSendTime;

  public bool waitingForRefresh;

  public class SendScore : ServerObjectMessage, ITimeStampReceiver
  {
    public string username;
    public int score;

    public DateTime timestamp { get; set; }
  }
  // Add/update a user's score and username
  // Respond with the current top scores
  void Handler(SendScore message)
  {
    scores[message.evt.source] = new LeaderboardRow { username = message.username, score = message.score };
    scoresChanged = true;
    SendScoresToView(message.timestamp);
  }

  public class Refresh : ServerObjectMessage, ITimeStampReceiver
  {
    public DateTime timestamp { get; set; }
  }
  void Handler(Refresh message)
  {
    waitingForRefresh = false;
    SendScoresToView(message.timestamp);
  }

  public class SetEntry : ServerObjectMessage, ITimeStampReceiver
  {
    public string id;
    public string username;
    public int score;
    public DateTime timestamp { get; set; }
  }
  void Handler(SetEntry message)
  {
    scores[message.id] = new LeaderboardRow { username = message.username, score = message.score };
    scoresChanged = true;
    SendScoresToView(message.timestamp);
  }

  public class RemoveEntry : ServerObjectMessage, ITimeStampReceiver
  {
    public string id;
    public DateTime timestamp { get; set; }
  }
  void Handler(RemoveEntry message)
  {
    if (scores.ContainsKey(message.id))
      scores.Remove(message.id);
    scoresChanged = true;
    SendScoresToView(message.timestamp);
  }

  public class GetEntries : ServerObjectMessage
  {
    public Dictionary<SOID<CoinPlayerSO>, LeaderboardRow> scores;
  }
  [MessageHook(HookSecurity.SHARED_WITH_CLIENT)]
  static void InjectEntryInfo(GetEntries msg, HookContext context)
  {
    var me = context.Self as CoinLeaderboardSO;
    msg.scores = me.scores;
  }
  [ClientSession]
  void Handler(GetEntries message)
  {
    // We don't have to effect any state
  }

  void SendScoresToView(DateTime timestamp)
  {
    // Limit the rate that we update the viewable LeaderBoard to conserve bandwidth
    var secondsSinceLastUpdate = (timestamp - lastSendTime).TotalSeconds;
    if (secondsSinceLastUpdate >= MAX_UPDATE_RATE)
    {
      UpdateView(timestamp);
    }
    else
    {
      RefreshAfterDelay();
    }
  }

  void UpdateView(DateTime timestamp)
  {
    // Send the top scores to the view
    var topScores = FindTopScores();
    var topScoresSoid = Registry.GetId<CoinTopScoresSO>(ID.Suffix);
    Send(topScoresSoid, new CoinTopScoresSO.SetTopScores { scores = topScores });
    lastSendTime = timestamp;
    scoresChanged = false;
  }

  void RefreshAfterDelay()
  {
    if (!waitingForRefresh && scoresChanged)
    {
      Send(ID, new Refresh(), (float)MAX_UPDATE_RATE);
      waitingForRefresh = true;
    }
  }

  LeaderboardRow[] FindTopScores()
  {
    var count = TOP_SCORE_RETURN_LENGTH < scores.Count ? TOP_SCORE_RETURN_LENGTH : scores.Count;

    var topScores = scores.OrderByDescending(entry => entry.Value.score).Take(count).ToArray();
    var result = new LeaderboardRow[count];
    for (int i = 0; i < topScores.Length; i++)
    {
      result[i] = topScores[i].Value;
    }
    return result;
  }
}
