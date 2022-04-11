using UnityEngine;
using ServerObjects;
using GPF.ServerObjects;
using GPF;
using GPF.UI.AutoComplete;

public class MatchController : MonoBehaviour
{
    [DataPaths]
    public class MatchDataPaths
    {
        [DataPaths.SoAlias(typeof(MatchPlayerSO))]
        public const string PLAYER = "match_player";
    }

    Syncer syncer;

    MatchPlayerSO matchPlayer;

    async void Start()
    {
        // Connect to GPF Backend
        syncer = Syncer.CreateSyncer();

        // Get the id of our player object to connect to
        var playerId = MatchPlayerSO.GetSOID(MatchmakerSO.MAIN_SUFFIX);

        // Sync to our player object
        matchPlayer = await syncer.Sync(playerId);

        // Make our player available to the UI under match_player
        syncer.Alias(matchPlayer, MatchDataPaths.PLAYER);
    }
        
    public void StartMatchMaking()
    {
        syncer.Send(matchPlayer.ID, new MatchPlayerSO.Match());
    }

    public void StopMatchMaking()
    {
        syncer.Send(matchPlayer.ID, new MatchPlayerSO.CancelMatch());
    }

    public void AcceptMatch()
    {
        syncer.Send(matchPlayer.ID, new MatchPlayerSO.Accept());
    }

    public void RejectMatch()
    {
        syncer.Send(matchPlayer.ID, new MatchPlayerSO.Reject());
    }

    public void Move(int move)
    {
        Move((MatchGameSO.Move) move);
    }

    void Move(MatchGameSO.Move move)
    {
        syncer.Send(matchPlayer.ID, new MatchPlayerSO.Move { move = move });
    }

    public void Leave()
    {
        syncer.Send(matchPlayer.ID, new MatchPlayerSO.LeaveTable());
    }

    private void OnApplicationPause(bool pause)
    {
        if (syncer != null)
        {
            if (pause)
                syncer.Pause();
            else
                syncer.Resume();
        }
    }

    private async void OnApplicationQuit()
    {
        if (syncer != null)
            await syncer.Disconnect();
    }
}