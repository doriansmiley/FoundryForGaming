using GPF.ServerObjects;

[DataStorePath("sync.coin_top_scores")]
[Syncable]
[Register("coin_top_scores")]
public class CoinTopScoresSO : ServerObject
{
    public static SOID<CoinTopScoresSO> MainSoid = Registry.GetId<CoinTopScoresSO>(CoinLeaderboardSO.MainSoid.Suffix);

    [ExpandData]
    public CoinLeaderboardSO.LeaderboardRow[] TopScores;

    public class SetTopScores : ServerObjectMessage
    {
        public CoinLeaderboardSO.LeaderboardRow[] scores;
    }
    void Handler(SetTopScores message)
    {
        TopScores = message.scores;
    }
}