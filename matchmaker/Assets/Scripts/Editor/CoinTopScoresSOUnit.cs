using NUnit.Framework;
using System;
using GPFEditor;
using System.Threading.Tasks;

namespace CoinTests
{
    [TestFixture]
    sealed class CoinTopScoresSOUnit
    {
        [Test]
        public void Run()
        {
            var probe = SOUnitTester<CoinTopScoresSO>.Create();

            var topScores = new CoinLeaderboardSO.LeaderboardRow[2];
            topScores[0] = new CoinLeaderboardSO.LeaderboardRow { username = "b", score = 100 };
            topScores[1] = new CoinLeaderboardSO.LeaderboardRow { username = "c", score = 50 };

            probe.TrustedSend(new CoinTopScoresSO.SetTopScores { scores = topScores });
            Assert.That(probe.so.TopScores, Has.Length.EqualTo(2));
            Assert.AreEqual(probe.so.TopScores[0].username, "b");
            Assert.AreEqual(probe.so.TopScores[0].score, 100);
            Assert.AreEqual(probe.so.TopScores[1].username, "c");
            Assert.AreEqual(probe.so.TopScores[1].score, 50);
        }
    }

}
