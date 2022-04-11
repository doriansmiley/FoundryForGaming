using NUnit.Framework;
using GPFEditor;
using ServerObjects;
using GPF.ServerObjects;
using System.Collections.Generic;
using System.Linq;

namespace MatchmakerTests
{
    [TestFixture]
    sealed class MatchmakerUnitTest
    {
        [Test]
        public void Match()
        {
            var probe = SOUnitTester<MatchmakerSO>.Create();

            var player1 = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);
            var player2 = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);

            var match1SendsOut = probe.TrustedSend(player1, new MatchmakerSO.Match());
            Assert.AreEqual(probe.so.players.Count, 1);
            Assert.AreEqual(match1SendsOut.Count, 0);

            var match2SendsOut = probe.TrustedSend(player2, new MatchmakerSO.Match());
            Assert.AreEqual(probe.so.players.Count, 0);
            Assert.AreEqual(match2SendsOut.Count, 1);
        }
    }

}
