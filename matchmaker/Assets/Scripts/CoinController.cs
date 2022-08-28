using UnityEngine;
using GPF;
using GPF.ServerObjects;
using GPF.UI.AutoComplete;

[DataPaths]
public class CoinDataPaths
{
    [DataPaths.SoAlias(typeof(CoinPlayerSO))]
    public const string PLAYER = "coin.player";

    [DataPaths.SoAlias(typeof(CoinTopScoresSO))]
    public const string TOP_SCORES = "coin.top_scores";

    [DataPaths.Value(typeof(bool))]
    public const string ANIMATING_FLIP = "coin.animations.flipping";

    [DataPaths.Value(typeof(CoinController.LoadingState))]
    public const string LOADING = "coin.loading";
}

public class CoinController : MonoBehaviour
{
    public enum LoadingState { LOADING, COMPLETE }

    Syncer syncer;
    CoinPlayerSO player;
    CoinTopScoresSO topScores;

    void Awake()
    {
        DataStore.Instance.Set(CoinDataPaths.LOADING, LoadingState.LOADING);
    }

    async void Start()
    {
        // Create syncer to communicate with our ServerObjects
        syncer = Syncer.CreateSyncer();

        // Generate a unique (and secure) CoinPlayerSO ID
        var playerID = Registry.GetId<CoinPlayerSO>();

        // Sync to the CoinPlayerSO with playerID
        player = await syncer.Sync(playerID);

        // Sync to the main CoinTopScoresSO
        topScores = await syncer.Sync(CoinTopScoresSO.MainSoid);

        // Our CoinPlayerSO's state is available in the DataStore under:
        // sync.coin_players.<playerID>
        // Next make the CoinPlayerSO's state consistently available to the UI
        // Use the alias "player" for the UI to access the player's state
        syncer.Alias(player, CoinDataPaths.PLAYER);

        syncer.Alias(topScores, CoinDataPaths.TOP_SCORES);

        DataStore.Instance.Set(CoinDataPaths.LOADING, LoadingState.COMPLETE);
    }

    // Call Flip from a button press
    public void Flip()
    {
       syncer.Send(player, new CoinPlayerSO.Flip());
    }

    // Call set username from an input field
    public void SetUsername(string username)
    {
        syncer.Send(player, new CoinPlayerSO.SetUsername { username = username });
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
