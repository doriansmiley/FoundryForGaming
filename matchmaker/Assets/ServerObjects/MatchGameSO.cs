using System.Collections.Generic;
using GPF.ServerObjects;

namespace ServerObjects
{
    [Register("match_game")]
    public class MatchGameSO : ServerObject
    {
        public enum Move { NONE = 0, ROCK , PAPER, SCISSORS }

        public enum Result { DRAW = 0, WIN, LOSE }

        public bool resultSent = false;
        
        public Dictionary<SOID, Move> moves = new Dictionary<SOID, Move>();

        public class Start : ServerObjectMessage
        {
            public SOID player1;
            public SOID player2;
        }
        // From MatchLobbySO
        void Handler(Start msg)
        {
            resultSent = false;
            moves.Add(msg.player1, Move.NONE);
            moves.Add(msg.player2, Move.NONE);

            Send(msg.player1, new MatchPlayerSO.StartGame { });
            Send(msg.player2, new MatchPlayerSO.StartGame { });

            Send(ID, new TimeIsUp(), 10);
        }

        public class TimeIsUp : ServerObjectMessage { }
        // From MatchGameSO
        public void Handler(TimeIsUp msg)
        {
            if (resultSent)
                return;
            EndGame();
        }

        public class SetMove : ServerObjectMessage
        {
            public Move move;
        }
        // From MatchPlayerSO
        public void Handler(SetMove msg)
        {
            var src = msg.evt.source;
            if (!moves.ContainsKey(src))
            {
                Logger.Error($"Received move message from bad actor: {msg.evt.source}");
                return;
            }
            moves[src] = msg.move;

            if (AllPlayersSubmittedMoves() && !resultSent)
            {
                EndGame();
            }
        }

        public override string ToString()
        {
            string s = $"Game with {moves.Count} moves: {string.Join(",", moves)}.";
            return s;
        }

        bool AllPlayersSubmittedMoves()
        {
            foreach(var kvp in moves)
            {
                if (kvp.Value == Move.NONE)
                    return false;
            }
            return true;
        }

        private void EndGame()
        {
            foreach (var kvp in moves)
            {
                var playerId = kvp.Key;
                var result = GetResult(playerId);
                Send(playerId, new MatchPlayerSO.EndGame
                {
                    result = result,
                    yourMove = kvp.Value,
                    opponentMove = GetOpponentMove(playerId)
                });
            }
            resultSent = true;
        }

        Result GetResult(SOID player)
        {
            var move = moves[player];
            var opponentMove= GetOpponentMove(player);
            if (move == opponentMove)
                return Result.DRAW;
            if (Beats(move, opponentMove))
                return Result.WIN;
            else
                return Result.LOSE;
        }

        bool Beats(Move m1, Move m2)
        {
            return m2 == Move.NONE ||
                m1 == Move.ROCK && m2 == Move.SCISSORS ||
                m1 == Move.SCISSORS && m2 == Move.PAPER ||
                m1 == Move.PAPER && m2 == Move.ROCK;
        }

        Move GetOpponentMove(SOID player)
        {
            foreach (var kvp in moves)
            {
                if (kvp.Key != player)
                {
                    return kvp.Value;
                }
            }
            return Move.NONE;
        }
    }
}
