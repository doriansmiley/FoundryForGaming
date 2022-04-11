using NUnit.Framework;
using GPFEditor;
using ServerObjects;
using GPF.ServerObjects;
using System.Collections.Generic;
using System.Linq;

namespace MatchmakerTests
{
    [TestFixture]
    sealed class MatchLobbyUnitTest
    {
        [Test]
        public void AllAccept()
        {
            var probe = SOUnitTester<MatchLobbySO>.Create();

            var player1 = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);
            var player2 = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);

            var initOut = probe.TrustedSend(new MatchLobbySO.Init { players = new List<SOID> { player1, player2 } });
            Assert.AreEqual(probe.so.responses.Count, 2);
            Assert.AreEqual(initOut.Count, 3);

            var accept1Out = probe.TrustedSend(player1, new MatchLobbySO.Accept());
            Assert.AreEqual(accept1Out.Count, 0);
            Assert.IsFalse(probe.so.resolved);

            var accept2Out = probe.TrustedSend(player2, new MatchLobbySO.Accept());
            Assert.AreEqual(accept2Out.Count, 1);
            Assert.IsTrue(probe.so.resolved);
        }

        [Test]
        public void Timeout()
        {
            var probe = SOUnitTester<MatchLobbySO>.Create();

            var player1 = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);
            var player2 = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);

            var initOut = probe.TrustedSend(new MatchLobbySO.Init { players = new List<SOID> { player1, player2 } });
            Assert.AreEqual(2, probe.so.responses.Count);
            Assert.AreEqual(3, initOut.Count);
            
            // Get the timeout message
            var timeoutMessage = initOut.First(msg => msg.target == probe.so.ID).soMsg;

            var timeoutOut = probe.TrustedSend(timeoutMessage);
            Assert.IsTrue(probe.so.resolved);
            Assert.AreEqual(2, timeoutOut.Count);
        }
    }

}
