using GPF.ServerObjects;
using System.Collections.Generic;

namespace ServerObjects
{
    [Register("match_lobby")]
    public class MatchLobbySO : ServerObject
    {
        public enum Response { NONE, ACCEPT, REJECT}

        public Dictionary<SOID, Response> responses = new Dictionary<SOID, Response>();

        public bool resolved;

        public SOID matchmaker;

        public class Init : ServerObjectMessage
        {
            public List<SOID> players;
        }
        // From MatchmakerSO
        void Handler(Init msg)
        {
            responses = new Dictionary<SOID, Response>();
            resolved = false;
            matchmaker = msg.evt.source;
            foreach (var player in msg.players)
            {
                responses[player] = Response.NONE;
                Send(player, new MatchPlayerSO.EnterLobby());
            }
            Send(ID, new Timeout(), 10);
        }

        public class Accept : ServerObjectMessage { }
        // From MatchPlayerSO
        void Handler(Accept message)
        {
            SOID player = message.evt.source;
            ReceiveResponse(player, Response.ACCEPT);
        }

        public class Reject : ServerObjectMessage { }
        // From MatchPlayerSO
        void Handler(Reject message)
        {
            SOID player = message.evt.source;
            ReceiveResponse(player, Response.REJECT);
        }

        public class Timeout : ServerObjectMessage { }
        // From self
        void Handler(Timeout msg)
        {
            if (resolved)
                return;
            // If we are still waiting for a response, cancel the match
            CancelMatch(false);
        }

        void ReceiveResponse(SOID player, Response response)
        {
            // If a response comes in too late, it's too late
            if (resolved)
                return;

            if (!responses.ContainsKey(player))
            {
                Logger.Error($"Received {response} from player that isn't in the lobby {player}");
                return;
            }
            if (responses[player] != Response.NONE)
                Logger.Warning($"Received second response from {player} previous response {responses[player]} changing to {response}");

            responses[player] = response;

            if (response == Response.REJECT)
            {
                // One rejection will send everyone else back to the matchmaker
                CancelMatch(true);
            }
            else
            {
                bool allAccepted = true;
                foreach (var kvp in responses)
                {
                    if (kvp.Value != Response.ACCEPT)
                        allAccepted = false;
                }
                if (allAccepted)
                {
                    StartMatch();
                }
            }
        }

        void CancelMatch(bool rematchNoneResponses)
        {
            resolved = true;

            // Send everyone who accepted back to matchmaking
            // Kick people who did not accept out of matchmaking
            var rematch = new List<SOID>();
            foreach (var kvp in responses)
            {
                var player = kvp.Key;
                var response = kvp.Value;

                if (response == Response.ACCEPT ||
                    rematchNoneResponses && response == Response.NONE)
                    rematch.Add(player);
                else
                    Send(player, new MatchPlayerSO.LobbyKick());
            }
            if (rematch.Count > 0)
                Send(matchmaker, new MatchmakerSO.Rematch { players = rematch });
        }

        void StartMatch()
        {
            resolved = true;

            var game = Registry.GetId<MatchGameSO>();

            var userIds = new List<SOID>(responses.Keys);

            Send(game, new MatchGameSO.Start
            {
                player1 = userIds[0],
                player2 = userIds[1],
            });
        }
    }
}