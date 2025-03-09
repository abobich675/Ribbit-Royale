using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Scoreboard;
using Unity.VisualScripting;
using UnityEditor;
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
    public int timerDuration = 90;
    private GameObject infoPanel;
    private GameObject gameOver;
    private float timerStartDelay = 8f;
    private List<ScoreEntry> scoreEntries = new List<ScoreEntry>();
    
    private Dictionary<ulong, int> rankedIds = new Dictionary<ulong, int>();
    private Dictionary<ulong, int> countDict = new Dictionary<ulong, int>();

    void Awake()
    {
        Debug.Log("ScoreController Awake...");
        scoreCanvas = GameObject.FindGameObjectWithTag("ScoreControllerGO");
        DontDestroyOnLoad(scoreCanvas);
    }
    
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
        try
        {
            Destroy(nonpersistScoreCanvas);
        } catch (Exception e)
        {
            Debug.Log("DestroyScoreboard No ScoreboardManager to destroy..." + e);
        }
        
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
        StartCoroutine(CoroutineTransitionToRoundScoreboard());
    }

    private IEnumerator CoroutineTransitionToRoundScoreboard()
    {
        yield return new WaitForSeconds(3f);
        DestroyScoreboard();
        Loader.LoadNetwork(Loader.Scene.ScoreboardScene);
        yield return new WaitForSeconds(0.4f);
        SpinUpNewRoundScoreManager();
        yield return new WaitForSeconds(5f);
        LoadPreLobbyScene();
        yield return null;
    }

    private void LoadPreLobbyScene()
    {
        Debug.Log("LoadPreLobbyScene");
        Loader.LoadNetwork(Loader.Scene.PreLobbyScene);
    }

    public void CalculatePlayerScores()
    {
        scoreEntries = scoreManager.GetEntryList();
        SpinUpNewGameOverPanel();
        ulong entryId;
        int entryRank;
        rankedIds.Clear();
        foreach (var entry in scoreEntries)
        {
            // Gets player ulong id and rank, adds to rankedIds dict
            entryId = entry.GetPlayerName();
            entryRank = entry.GetRank();
            rankedIds.Add(entryId, entryRank);
        }

        foreach (var rankEntry in rankedIds)
        {
            if (rankEntry.Value == 1)
            {
                RibbitRoyaleMultiplayer.Instance.IncPlayerScore(1, rankEntry.Key);
            }
        }
        TransitionToRoundScoreboard();
        return;
    }

    public void CTA_CalculatePlayerScores(int playerCount, int finalCount)
    {

        // Gets owner clientID
        PlayerData ctaPlayerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        ulong ctaPlayerId = ctaPlayerData.clientId;
        ulong ownerId = NetworkManager.Singleton.CurrentSessionOwner;
        var isHost = ctaPlayerId == ownerId;
        if (!isHost)
        {
            return;
        }
        
        // Need to check highest score, single player implementation below
        // Should check who's score has the smallest difference to finalCount, award multiple if ties
        RibbitRoyaleMultiplayer.Instance.IncPlayerScore(1, ctaPlayerId);
        TransitionToRoundScoreboard();
    }

    public void SetPlayerFinished()
    {
        Debug.Log("ScoreController SetPlayerFinished() Started...");
        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        ulong playerId = playerData.clientId;
        Debug.Log("ScoreController SetPlayerFinished() Calling SM SetPlayerFinished() with value: " + playerId + "...");
        scoreManager.SetFinished(playerId);
    }

    private PlayerData GetPlayerData(ulong playerId)
    {
        return RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(playerId);
    }
    
    private ScoreManager SpinUpNewRoundScoreManager()
    {
        InstantiateNonPersistScoreCanvas();
        scoreManager = nonpersistScoreCanvas.AddComponent<ScoreManager>();
        scoreManager.SetupScoreboard(nonpersistScoreCanvas.transform, 0, _playerCount, timerDuration, timerStartDelay);
        foreach (var entry in playerDataDict)
        {
            Debug.Log("Adding Player to Round Score Manager => clientId: " + entry.Value.clientId);
            var pData = GetPlayerData(entry.Key);
            var score = pData.GetPlayerScore();
            var prevScore = pData.previousRoundPlayerScore;
            scoreManager.CreatePlayerEntry(entry.Key, score, entry.Value.colorId, 0, prevScore);
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
        infoPanel.GetComponent<infoUI>().infoTitle.color = Color.black;
        infoPanel.GetComponent<infoUI>().infoText.color = Color.black;
        if (gameType == 1)
        {
            // TongueSwing
            infoPanel.GetComponent<infoUI>().infoTitle.text = "Lickity Split";
            infoPanel.GetComponent<infoUI>().infoText.text =
                "Welcome to Lickity Split! To move, use WASD. \n" +
                "To swing, click near a grapple to pull yourself towards it. \n" +
                "Reach the final platform at the top before time runs out! \n" +
                "Your score is your distance to the finish in meters.";
        } else if (gameType == 2)
        {
            // Count The Animals
            infoPanel.GetComponent<infoUI>().infoTitle.text = "Count The Animals";
            infoPanel.GetComponent<infoUI>().infoText.text =
                "Welcome to Count The Animals! \n" +
                "In this game, different animals will cross your screen." +
                "You must keep track of your animal, specified on the right of your screen." +
                "Click whenever you see your animal and try to get as close as you can to win!";
        }
        Invoke(nameof(DestroyInfoPanel), 8f);
    }

    private void SpinUpNewGameOverPanel()
    {
        // Stop Timer
        scoreManager.GameOver_StopCoroutines();
        // Create GameOver overlay
        var gameOverPrefab = Resources.Load<GameObject>("gameOverOverlay");
        gameOver = Instantiate(gameOverPrefab, nonpersistScoreCanvas.transform);
        gameOver.GetComponent<infoUI>().infoTitle.color = Color.black;
        gameOver.GetComponent<infoUI>().infoText.color = Color.black;

        gameOver.GetComponent<infoUI>().infoTitle.text = "Game Over";
        var bodyText = "";
        foreach (var entry in scoreEntries)
        {
            bodyText += "Player " + entry.GetPlayerName() + ": " + entry.GetScoreString() + "\n";
        }
        gameOver.GetComponent<infoUI>().infoText.text = bodyText;
        
    }

    private void DestroyInfoPanel()
    {
        Destroy(infoPanel);
    }

    public float GetPopupTimer(int gameType)
    {
        // if TS
        if (gameType == 0)
        {
            return timerStartDelay;
        }
        else
        {
            return 0;
        }
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

    public void RemovePlayerScore(ulong playerId)
    {
        // Remove player if Network calls disconnection hook
        scoreManager.RemovePlayerEntry(playerId);
        playerDataDict.Remove(playerId);
        Debug.Log("Removed Player: '" + playerId + "' from scoreboard...");
    }
    
}