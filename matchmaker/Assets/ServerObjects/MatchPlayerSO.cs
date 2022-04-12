using GPF.ServerObjects;
using System.Collections.Generic;

namespace ServerObjects
{
    [DataStorePath("sync.match_players")]
    [Syncable]
    [Register("match_player")]
    public class MatchPlayerSO : AnalyticsSO
    {
        public enum State
        {
            INITIAL = 0,
            IDLE,
            MATCHMAKING,
            TABLE_OFFERED,
            TABLE_ACCEPTED,
            GAME_STARTED,
            GAME_WAITING,
            GAME_WON,
            GAME_LOST,
            GAME_DRAW,
            PURCHASE_OFFER,
        };
        
        public State state = State.IDLE;

        public SOID<MatchGameSO> game;

        public SOID<MatchLobbySO> lobby;

        public MatchGameSO.Move move;

        public MatchGameSO.Move opponentMove;

        public class Match : AnalyticsMessage { }
        [FromClient]
        void Handler(Match msg)
        {
            state = State.MATCHMAKING;
            Send(GetMyMatchmakerID(), new MatchmakerSO.Match());
        }

        public class Accept : AnalyticsMessage { }
        [FromClient]
        void Handler(Accept msg)
        {
            state = State.TABLE_ACCEPTED;
            if (lobby != null)
            {
                Send(lobby, new MatchLobbySO.Accept());
            }
            else
            {
                Logger.Warning("Trying to accept match, but not in lobby");
            }
        }

        public class Reject : AnalyticsMessage { }
        [FromClient]
        void Handler(Reject message)
        {
            state = State.IDLE;
            if (lobby != null)
            {
                Send(lobby, new MatchLobbySO.Reject());
            }
            else
            {
                Logger.Warning("Trying to reject match, but not in lobby");
            }
        }

        public class CancelMatch : AnalyticsMessage { }
        [FromClient]
        void Handler(CancelMatch msg)
        {
            state = State.IDLE;
            Send(GetMyMatchmakerID(), new MatchmakerSO.CancelMatch());
        }

        public class LeaveTable : AnalyticsMessage { }
        [FromClient]
        void Handler(LeaveTable msg)
        {
            state = State.IDLE;
        }

        public class Move : AnalyticsMessage
        {
            public MatchGameSO.Move move;
        }
        [FromClient]
        void Handler(Move msg)
        {
            if (state == State.GAME_STARTED)
            {
                Send(game, new MatchGameSO.SetMove { move = msg.move });
                state = State.GAME_WAITING;
                move = msg.move;
            }
        }

        public class GoToPurchaseOffer : ServerObjectMessage {}
        [FromClient]
        void Handler(GoToPurchaseOffer msg)
        {
            state = State.PURCHASE_OFFER;
        }

        public class MakePurchase : AnalyticsMessage { }
        [FromClient]
        void Handler(MakePurchase msg)
        {
            state = State.IDLE;
        }

        public class RejectPurchase : AnalyticsMessage { }
        [FromClient]
        void Handler(RejectPurchase msg)
        {
            state = State.IDLE;
        }

        // From Matchmaker
        public class EnterLobby : ServerObjectMessage { }
        void Handler(EnterLobby msg)
        {
            lobby = msg.evt.source;
            state = State.TABLE_OFFERED;
        }

        // From Game
        public class StartGame : ServerObjectMessage { }
        void Handler(StartGame msg)
        {
            state = State.GAME_STARTED;
            game = msg.evt.source;
        }

        // From Game
        public class EndGame : ServerObjectMessage
        {
            public MatchGameSO.Result result;
            public MatchGameSO.Move yourMove;
            public MatchGameSO.Move opponentMove;
        }
        void Handler(EndGame msg)
        {
            if (msg.result == MatchGameSO.Result.DRAW)
                state = State.GAME_DRAW;
            else if (msg.result == MatchGameSO.Result.WIN)
                state = State.GAME_WON;
            else if (msg.result == MatchGameSO.Result.LOSE)
                state = State.GAME_LOST;

            move = msg.yourMove;
            opponentMove = msg.opponentMove;
        }

        // From Lobby
        public class LobbyKick : ServerObjectMessage { }
        void Handler(LobbyKick msg)
        {
            if (lobby == msg.evt.source)
            {
                lobby = null;
                state = State.IDLE;
            }
        }

        // From Matchmaker
        public class Rematching : ServerObjectMessage { }
        void Handler(Rematching msg)
        {
            state = State.MATCHMAKING;
        }
        
        public override string ToString()
        {
            var gameString = string.IsNullOrEmpty(game) ? "" : $" gameId: {game}";
            string s = $"Player: {state} {gameString}";
            return s;
        }

        SOID<MatchmakerSO> GetMyMatchmakerID()
        {
            if (!ID.Suffix.Contains("/"))
            {
                Logger.Warning($"{GetType().Name} invalid SOID suffix. It should contain the matchmaker ID.  Current ID: {ID} ");
            }
            int slashIndex = ID.Suffix.IndexOf("/");
            string matchmakerId = ID.Suffix.Substring(0, slashIndex);
            return Registry.GetId<MatchmakerSO>(matchmakerId);
        }

        public static SOID<MatchPlayerSO> GetSOID(string matchmakerSuffix)
        {
            var playerSuffix = Registry.GenerateID();
            var suffix = $"{matchmakerSuffix}/{playerSuffix}";
            return Registry.GetId<MatchPlayerSO>(suffix);
        }
        public static SOID<MatchPlayerSO> GetSOID(string playerSuffix, string matchmakerSuffix)
        {
            var suffix = $"{matchmakerSuffix}/{playerSuffix}";
            return Registry.GetId<MatchPlayerSO>(suffix);
        }

        protected override Dictionary<string, object> GetAnalyticsState()
        {
            return new Dictionary<string, object> {
                ["screen"] = state.ToString(),
            };
        }
    }
}
