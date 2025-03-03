using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Scoreboard;
using Unity.VisualScripting;
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
    private GameObject infoPanel;
    private float timerStartDelay = 8f;

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
        SpinUpNewInfoPanel(1);
        //Instantiate(scoreManager, GetComponent<Camera>());
    }
    
    private void DestroyScoreboard()
    {
        Debug.Log("Destroying nonpersistScoreCanvas, scoreManager=null...");
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
        DestroyScoreboard();
        Loader.LoadNetwork(Loader.Scene.ScoreboardScene);
        Invoke(nameof(SpinUpNewRoundScoreManager), 1f);
        Invoke(nameof(LoadPreLobbyScene), 5f);
    }

    private void LoadPreLobbyScene()
    {
        Debug.Log("LoadPreLobbyScene");
        Loader.LoadNetwork(Loader.Scene.PreLobbyScene);
    }
    

    private ScoreManager SpinUpNewRoundScoreManager()
    {
        InstantiateNonPersistScoreCanvas();
        scoreManager = nonpersistScoreCanvas.AddComponent<ScoreManager>();
        scoreManager.SetupScoreboard(nonpersistScoreCanvas.transform, 0, _playerCount, timerDuration, timerStartDelay);
        foreach (var entry in playerDataDict)
        {
            //Debug.Log("COLOR: " + entry.Value.clientId);
            var score = entry.Value.playerScore;
            scoreManager.CreatePlayerEntry(entry.Key, score, entry.Value.colorId, 0);
        }

        scoreManager.UpdateRanking();
        Debug.Log("New ScoreboardRound set up...");
        //scoreManager.UpdateRanking();
        return scoreManager;
    }

    private void InstantiateNonPersistScoreCanvas()
    {
        nonpersistScoreCanvas = Instantiate(Resources.Load<GameObject>("nonpersistScoreCanvas"));
    }

    private void SpinUpNewInfoPanel(int gameType)
    {
        // Get infoUI prefab, instantiate as child of existing nonpersistScoreCanvas
        var infoPrefab = Resources.Load<GameObject>("infoPopup");
        infoPanel = Instantiate(infoPrefab, nonpersistScoreCanvas.transform);
        if (gameType == 1)
        {
            // TongueSwing
            infoPanel.GetComponent<infoUI>().infoTitle.text = "Tongue Swing";
            infoPanel.GetComponent<infoUI>().infoText.text = "Welcome to Tongue Swing! To move, use WASD. " +
             "To swing, click near a grapple to pull yourself towards it. " +
             "Get to the green finish platform at the top before time runs out!";
        }
        Invoke(nameof(DestroyInfoPanel), 8f);
    }

    private void DestroyInfoPanel()
    {
        Destroy(infoPanel);
    }

    private ScoreManager SpinUpNewScoreManager()
    {
        InstantiateNonPersistScoreCanvas();
        scoreManager = nonpersistScoreCanvas.AddComponent<ScoreManager>();
        scoreManager.SetupScoreboard(nonpersistScoreCanvas.transform, 1, _playerCount, timerDuration, timerStartDelay);
        // Create player entries for all players in playerDataDict 
        foreach (var entry in playerDataDict)
        {
            //Debug.Log("COLOR: " + entry.Value.clientId);
            scoreManager.CreatePlayerEntry(entry.Key, 0, entry.Value.colorId, 1);
        }
        
        scoreManager.StartDistanceToFinishCoroutine();
        return scoreManager;
    }

    public void SetFinished()
    {
        
    }
}