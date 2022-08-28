using System;
using System.Collections.Generic;
using GPF.ServerObjects;
using ServerObjects;

[DataStorePath("sync.coin_players")]
[Syncable]
[Register("coin_player")]
public class CoinPlayerSO : MatchAnalyticsSO
{
  // Set the time it takes to flip a coin (in seconds)
  // Because this value is in the SO, it can be enforced by the server
  public const int FLIP_SECONDS = 1;

  public enum CoinState
  {
    HEADS,
    TAILS,
    FLIPPING
  }
  public CoinState state;

  public int currentStreak = 0;

  public int longestStreak = 0;

  public string username = "Anonymous";

  public class SetUsername : AnalyticsMessage
  {
    public string username;
  }
  [FromClient] // <- [FromClient] allows a user to send this type of message
  void Handler(SetUsername message)
  {
    username = message.username;
    // Update our user name on the leaderboard
    Send(
        CoinLeaderboardSO.MainSoid,
        new CoinLeaderboardSO.SendScore
        {
          score = longestStreak,
          username = username
        }
        );
  }

  public class Flip : AnalyticsMessage { }

  [FromClient] // <- [FromClient] allows a user to send this type of message
  void Handler(Flip message)
  {
    // To prevent bad actors from from flipping faster than the flip time
    // we keep track of the coin state, and don't allow simultaneous flips
    if (state != CoinState.FLIPPING)
    {
      state = CoinState.FLIPPING;

      // Send the result of the flip to ourselves after a delay
      Send(ID, new FlipResult(), FLIP_SECONDS);
    }
  }

  public class FlipResult : ServerObjectMessage
  {
    public bool isHeads;
  }
  // All message handlers must leave the SO in a deterministic state
  // If you generate random numbers, new object IDs, or GUIDs you
  // can set them in the message before the handler using hooks.
  [MessageHook(HookSecurity.SHARED_WITH_CLIENT)]
  public static void CoinFlipHook(FlipResult msg)
  {
    Random rand = new Random();
    msg.isHeads = rand.Next(2) == 0;
  }
  // This handler does not have [FromClient] so only the server can send it 
  void Handler(FlipResult message)
  {
    if (message.isHeads)
    {
      currentStreak++;
      state = CoinState.HEADS;
    }
    else
    {
      currentStreak = 0;
      state = CoinState.TAILS;
    }
    if (currentStreak > longestStreak)
    {
      longestStreak = currentStreak;
      Send(
          CoinLeaderboardSO.MainSoid,
          new CoinLeaderboardSO.SendScore
          {
            score = longestStreak,
            username = username
          }
          );
    }
  }

  public override string ToString()
  {
    string s = $"CoinPlayer: {username} currentStreak: {currentStreak} currentStreak: {longestStreak}";
    return s;
  }

  protected override Dictionary<string, object> GetAnalyticsState()
  {
    return new Dictionary<string, object>
    {
      ["screen"] = $"Streak{currentStreak}",
    };
  }
}
