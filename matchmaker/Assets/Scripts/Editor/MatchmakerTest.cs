using ServerObjects;
using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using GPFEditor;
using NUnit.Framework;

namespace MatchmakerTests
{
    [TestFixture]
    [TestOf(typeof(MatchPlayerSO))]
    sealed class MatchmakerTest : ServerObjectTest
    {
        public override async Task Run()
        {
            var player1Name = "Gina";
            var player2Name = "Bob";
            var matchmakerId = MatchmakerSO.GetTestSuffix();

            var player1Task = PlayerActions(
                CreateSyncer("player1"),
                MatchPlayerSO.GetSOID(matchmakerId),
                player1Name,
                MatchGameSO.Move.ROCK,
                MatchPlayerSO.State.GAME_LOST
                );

            var player2Task = PlayerActions(
                CreateSyncer("player2"),
                MatchPlayerSO.GetSOID(matchmakerId),
                player2Name,
                MatchGameSO.Move.PAPER,
                MatchPlayerSO.State.GAME_WON
                );

            await player1Task;
            await player2Task;
        }

        async Task PlayerActions(Syncer syncer, SOID<MatchPlayerSO> id, string name, MatchGameSO.Move move, MatchPlayerSO.State outcome)
        {
            var player = await syncer.Sync(id);

            // Wait for the server object to finish initialization
            await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.NotEqual(MatchPlayerSO.State.INITIAL));

            // Request a match
            syncer.Send(id, new MatchPlayerSO.Match());

            // Wait for the matchmaker to find a match
            await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.Equal(MatchPlayerSO.State.TABLE_OFFERED));

            // Accept the offer to join the game
            syncer.Send(id, new MatchPlayerSO.Accept());

            // Wait for the other player to accept the match
            await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.Equal(MatchPlayerSO.State.GAME_STARTED));

            // submit our move
            syncer.Send(id, new MatchPlayerSO.Move { move = move });

            // Wait for the game to resolve
            await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.Equal(outcome));
        }
    }
}