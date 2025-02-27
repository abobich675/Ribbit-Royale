using Unity.Netcode;
using System;
using System.Collections.Generic;
using UI.Scoreboard;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreController : NetworkBehaviour
{
    public static ScoreController Instance { get; private set; }
    private int _playerCount;
    private Dictionary<ulong, PlayerData> playerDataDict = new Dictionary<ulong, PlayerData>();
    private ScoreManager scoreManager; // currently existing scoremanager object
    private GameObject scoreCanvas;
    private GameObject nonpersistScoreCanvas;
    public int boardType;
    public int timerDuration;

    void Awake()
    {
        Debug.Log("ScoreController Awake...");
        scoreCanvas = GameObject.FindGameObjectWithTag("ScoreControllerGO");
        DontDestroyOnLoad(scoreCanvas);
    }
    
    // 
    void Start()
    {
        Debug.Log("ScoreController Start...");
        //CreateInGameScoreboard();
    }

    public void CreateInGameScoreboard()
    {
        // Creates new inGameScoreboard when called
        scoreManager = SpinUpNewScoreManager();
        //Instantiate(scoreManager, GetComponent<Camera>());
    }
    
    private void DestroyInGameScoreboard()
    {
        Destroy(nonpersistScoreCanvas);
        scoreManager = null;
    }

    public void InitializeTS()
    {
        _playerCount = RibbitRoyaleMultiplayer.Instance.GetPlayerCount();
        
        // Create dict of <clientId, gameData> pairs playerDataDict
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (!playerDataDict.ContainsKey(client.Key))
            {
                playerDataDict.Add(client.Key, RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(client.Key));
            }
        }

        // Make sure player count is correct
        if (playerDataDict.Count != _playerCount)
        {
            Debug.Log("_playerCount != true player count! ERROR");
        }
        
        CreateInGameScoreboard();
    }

    public void TransitionToRoundScoreboard()
    {
        DestroyInGameScoreboard();
        Loader.LoadNetwork(Loader.Scene.ScoreboardScene);
        scoreManager = SpinUpNewRoundScoreManager();
        Invoke(nameof(LoadPreLobbyScene), 5f);
    }

    private void LoadPreLobbyScene()
    {
        Loader.LoadNetwork(Loader.Scene.PreLobbyScene);
    }
    

    private ScoreManager SpinUpNewRoundScoreManager()
    {
        scoreManager = gameObject.AddComponent<ScoreManager>();
        //Instantiate(scoreManager);
        return scoreManager;
    }

    private ScoreManager SpinUpNewScoreManager()
    {
        nonpersistScoreCanvas = Instantiate(Resources.Load<GameObject>("nonpersistScoreCanvas"));
        scoreManager = nonpersistScoreCanvas.AddComponent<ScoreManager>();
        scoreManager.SetupScoreboard(nonpersistScoreCanvas.transform, boardType, _playerCount, timerDuration);
        // Create player entries for all players in playerDataDict 
        foreach (var entry in playerDataDict)
        {
            //Debug.Log("COLOR: " + entry.Value.clientId);
            scoreManager.CreatePlayerEntry(entry.Key, 0, entry.Value.colorId, boardType);
        }
        
        scoreManager.StartDistanceToFinishCoroutine();
        return scoreManager;
    }

    public void SetFinished()
    {
        
    }
}