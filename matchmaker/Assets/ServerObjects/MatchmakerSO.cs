using GPF.ServerObjects;
using System.Collections.Generic;

namespace ServerObjects
{
    [Register("matchmaker")]
    public class MatchmakerSO : ServerObject
    {
        // Matchmaker is a singleton, use the main suffix for the SOID to access it
        public const string MAIN_SUFFIX = "matchmaker_main";

        public const int PLAYERS_IN_GAME = 2;

        // Players waiting for a match
        public LinkedList<SOID<MatchPlayerSO>> players = new LinkedList<SOID<MatchPlayerSO>>();
        
        public class Match : ServerObjectMessage { }
        // From MatchPlayerSO
        void Handler(Match message)
        {
            SOID<MatchPlayerSO> matchedPlayer = message.evt.source;
            players.AddLast(matchedPlayer);
            MatchUsers();
        }

        public class CancelMatch : ServerObjectMessage { }
        // From MatchPlayerSO
        void Handler(CancelMatch message)
        {
            SOID<MatchPlayerSO> cancelingPlayer = message.evt.source;
            if (players.Contains(cancelingPlayer))
            {
                players.Remove(cancelingPlayer);
            }
        }

        public class Rematch : ServerObjectMessage
        {
            public List<SOID> players;
        }
        // From MatchLobbySO
        void Handler(Rematch msg)
        {
            foreach (var player in msg.players)
            {
                // Add players back with high priority
                players.AddFirst(player);
            }

            MatchUsers();
            
            foreach (var player in msg.players)
            {
                if (players.Contains(player))
                {
                    Send(player, new MatchPlayerSO.Rematching());
                }
            }
        }

        void MatchUsers()
        {
            while (players.Count >= PLAYERS_IN_GAME)
            {
                var seat1 = players.First.Value;
                players.RemoveFirst();
                var seat2 = players.First.Value;
                players.RemoveFirst();
                // Don't match a user with themselves! Even if they asked twice
                if (seat1 == seat2)
                {
                    Logger.Warning($"Matchmaker {ID} had duplicate match requests from {seat1}, deduplicating");
                    players.AddFirst(seat1);
                    continue;
                }

                var lobbySOID = Registry.GetId<MatchLobbySO>();

                Send(lobbySOID, new MatchLobbySO.Init
                {
                    players = new List<SOID> { seat1, seat2 },
                });
            }
        }

        public override string ToString()
        {
            string s = $"MatchMaker: Count: {players.Count} Players: {string.Join(",", players)}";
            return s;
        }

        public static string GetTestSuffix()
        {
            // Generate a "random" suffix so the matchmaker is in a known new state
            var time = (System.DateTime.UtcNow.ToFileTimeUtc() & 0xFFFFFF).ToString("x2");
            return "matchmaker-" + time;
        }
    }
}