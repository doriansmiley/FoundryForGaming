using GPF.ServerObjects;

[DataStorePath("sync.coin_admin")]
[Syncable]
[Register("coin_admin")]
public class CoinAdminSO : ServerObject
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

