using System;
using System.Collections.Generic;
using GPF.ServerObjects;
using ServerObjects;

[DataStorePath("sync.coin_leaderboard_admin")]
[Syncable]
[Register("coin_leaderboard_admin")]
public class CoinLeaderboardAdminSO : ServerObject
{
  public class SetEntry : ServerObjectMessage
  {
    public string id;
    public string username;
    public int score;
  }
  [FromClient] // <- [FromClient] allows a user to send this type of message
  void Handler(SetEntry message)
  {
    Send(
        CoinLeaderboardSO.MainSoid,
        new CoinLeaderboardSO.SetEntry
        {
          id = message.id,
          username = message.username,
          score = message.score,
        }
        );
  }

  public class RemoveEntry : ServerObjectMessage
  {
    public string id;
  }
  [FromClient] // <- [FromClient] allows a user to send this type of message
  void Handler(RemoveEntry message)
  {
    Send(
        CoinLeaderboardSO.MainSoid,
        new CoinLeaderboardSO.RemoveEntry
        {
          id = message.id,
        }
        );
  }
}

