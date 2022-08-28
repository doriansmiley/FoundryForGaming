using NUnit.Framework;
using GPFEditor;

namespace CoinTests
{
    [TestFixture]
    sealed class CoinLeaderBoardSOUnit
    {
        [Test]
        public void Run()
        {
            var probe = SOUnitTester<CoinLeaderboardSO>.Create();

            var topScores = new CoinLeaderboardSO.LeaderboardRow[2];
            topScores[0] = new CoinLeaderboardSO.LeaderboardRow { username = "b", score = 100 };
            topScores[1] = new CoinLeaderboardSO.LeaderboardRow { username = "c", score = 50 };

            var firstOutputs = probe.TrustedSend(new CoinLeaderboardSO.SendScore { username = "a", score = 5 });

            Assert.AreEqual(1, firstOutputs.Count);
            Assert.IsInstanceOf<CoinTopScoresSO.SetTopScores>(firstOutputs[0].soMsg);

            var secondOutputs = probe.TrustedSend(new CoinLeaderboardSO.SendScore { username = "b", score = 7 });

            Assert.AreEqual(1, secondOutputs.Count);
            Assert.IsInstanceOf<CoinLeaderboardSO.Refresh>(secondOutputs[0].soMsg);

            var thirdOutputs = probe.TrustedSend(secondOutputs[0].soMsg, secondOutputs[0].delay);

            Assert.AreEqual(1, thirdOutputs.Count);
            Assert.IsInstanceOf<CoinTopScoresSO.SetTopScores>(thirdOutputs[0].soMsg);
        }
    }
}
