using NUnit.Framework;
using System;
using GPFEditor;
using System.Threading.Tasks;

namespace CoinTests
{
    [TestFixture]
    sealed class CoinPlayerSOUnit
    {
        [Test]
        public void Run()
        {
            var probe = SOUnitTester<CoinPlayerSO>.Create();
            
            var setUsernameOut = probe.ClientSend(new CoinPlayerSO.SetUsername { username = "a"});
            Assert.That(probe.so.username, Is.EqualTo("a"));
            Assert.That(setUsernameOut, Has.Count.EqualTo(1) );
            var sendScore = (CoinLeaderboardSO.SendScore)setUsernameOut[0].soMsg;
            Assert.NotNull(sendScore);
            var topScores = new CoinLeaderboardSO.LeaderboardRow[2];
            topScores[0] = new CoinLeaderboardSO.LeaderboardRow { username = "b", score = 100 };
            topScores[1] = new CoinLeaderboardSO.LeaderboardRow { username = "c", score = 50 };

            var heads_covered = false;
            var tails_covered = false;
            do
            {
                var flipOut = probe.ClientSend(new CoinPlayerSO.Flip());
                Assert.That(flipOut, Has.Count.EqualTo(1));
                Assert.That(flipOut[0].delay ,Is.EqualTo(1));
                var flipOutMessage = (CoinPlayerSO.FlipResult)flipOut[0].soMsg;
                Assert.NotNull(flipOutMessage);
                var flipResultOut = probe.TrustedSend(flipOutMessage);
                if (probe.so.state == CoinPlayerSO.CoinState.HEADS)
                {
                    Assert.That(flipResultOut, Has.Count.EqualTo(1));
                    var sendscore = (CoinLeaderboardSO.SendScore)flipResultOut[0].soMsg;
                    Assert.NotNull(sendScore);
                    heads_covered = true;
                 }
                else
                {
                    Assert.That(flipResultOut, Has.Count.EqualTo(0));
                    tails_covered = true;
                }
            }
            while (!heads_covered || !tails_covered);
            
         
        }
    }

}
