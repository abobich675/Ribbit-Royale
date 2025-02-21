using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public GameObject scoreEntryPrefab;
    public GameObject inGameScoreEntryPrefab;
    public Transform scoreboardContent;
    public Transform inGameScoreboardContent;
    public float moveDuration = 10.0f;

    public Sprite spriteRed;
    public Sprite spriteBlue;
    public Sprite spriteGreen;
    public Sprite spriteYellow;

    private Dictionary<string, (Color, Sprite)> colorSpriteDictionary = new Dictionary<string, (Color, Sprite)>();
    private Dictionary<string, string> playerColorDictionary = new Dictionary<string, string>();

    //private Dictionary<string, (int score, GameObject entry)> playerEntries = new Dictionary<string, (int, GameObject)>();
    private List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        Debug.Log("ScoreManager Start() executing...");
        ScoreManager scoreboard = ScoreManager.Instance;
        CreateExampleInstance(scoreboard);
    }

    private void CreateExampleInstance(ScoreManager scoreboard)
    {
        colorSpriteDictionary.Add("red", (Color.red, spriteRed));
        colorSpriteDictionary.Add("blue", (Color.blue, spriteBlue));
        colorSpriteDictionary.Add("green", (Color.green, spriteGreen));
        colorSpriteDictionary.Add("yellow", (Color.yellow, spriteYellow));
        playerColorDictionary.Add("Player0", "red");
        playerColorDictionary.Add("Player1", "blue");
        playerColorDictionary.Add("Player2", "yellow");
        playerColorDictionary.Add("Player3", "green");
        //var getColor = RibbitRoyaleMultiplayer.GetPlayerColor(0);
        scoreboard.UpdatePlayerScore("Player0", 29, false);
        scoreboard.UpdatePlayerScore("Player1", 30, false);
        scoreboard.UpdatePlayerScore("Player2", 15, false);
        scoreboard.UpdatePlayerScore("Player3", 31, false);
        scoreboard.UpdateRanking();
        Debug.Log("ScoreManager example instance initialization complete.");
    }

    private ScoreEntry AddPlayer(string playerName)
    {
        // Creates new ScoreEntry object, sets parameters, adds to scoreEntries list, returns scoreEntry object
        ScoreEntry scoreEntry = new ScoreEntry();
        scoreEntry.SetPlayerName(playerName);
        scoreEntry.SetGameObject(Instantiate(scoreEntryPrefab, scoreboardContent));
        scoreEntry.SetInGameGameObject(Instantiate(inGameScoreEntryPrefab, inGameScoreboardContent));
        scoreEntry.Initialize();
        scoreEntries.Add(scoreEntry);
        return scoreEntry;
    }

    public void UpdatePlayerScore(string playerName, int score, bool isIncrement)
    {
        foreach (ScoreEntry entry in scoreEntries)
        {
            if (entry.GetPlayerName() == playerName)
            {
                if (isIncrement)
                {
                    UpdateScoreText(entry, entry.GetScore() + score);
                }
                else
                {
                    UpdateScoreText(entry, score);
                }
                return;
            }
        }
        
        // if creating new user in leaderboard
        {
            var entry = AddPlayer(playerName);
            UpdateScoreText(entry, score);
            
            // temp takes color+avatar from premade dict
            string playerColor = playerColorDictionary[playerName];
            var tupleColor = colorSpriteDictionary[playerColor];
            
            UpdateEntryColors(entry, tupleColor.Item1, tupleColor.Item2);
        }
    }

    public void UpdateEntryColors(ScoreEntry entry, Color teamColor, Sprite avatar)
    {
        entry.SetAvatar(avatar);
        entry.SetEntryColor(teamColor);
    }

    private void UpdateScoreText(ScoreEntry entry, int score)
    // Updates the score field of a given player
    // Takes GameObject entry [player entry in scoreboard], playerName [string], score [int]
    {
        entry.SetScore(score);
        
        //entry.transform.Find("RankText").GetComponent<Text>().text = "?";
        //entry.transform.Find("PointsText").GetComponent<Text>().text = $"{score}";
        UpdateRanking();
    }

    private void UpdateRanking()
    {
        // Creates a list <string, (score, entry)> sortedPlayers
        // List<KeyValuePair<string, (int score, ScoreEntry entry)>> sortedPlayers =
        //     new List<KeyValuePair<string, (int, ScoreEntry)>>(scoreEntries);
        
        var sortedP = scoreEntries;

        // Sorts sortedPlayers list by (a, b), and (b score CompareTo a score)
        
        //sortedPlayers.Sort((a, b) => b.Value.score.CompareTo(a.Value.score));
        
        // x.CompareTo(y) will return 0 if sorted (e.g. x == y), -1 if preceding (e.g. x < y), +1 if should follow (e.g. x > y)
        
        sortedP.Sort((a, b) => b.GetScore().CompareTo(a.GetScore()));
        
        // Creates an array startPositions of player-entry:y-value
        // Dictionary<GameObject, float> startPositions = new Dictionary<GameObject, float>();
        
        // foreach (var player in sortedPlayers)
        // {
        //     startPositions[player.Value.entry] = player.Value.entry.transform.localPosition.y;
        // }

        foreach (var entry in sortedP)
        {
            entry.SetY(entry.GetGameObject().transform.localPosition.y);
        }
        
        // Sets the siblingindex for each entry in sortedPlayers - reorders object hierarchy in scene 
        for (int i = 0; i < sortedP.Count; i++)
        {
            sortedP[i].GetGameObject().transform.SetSiblingIndex(i);
        }

        Debug.Log("Rankings Sorted, Starting AnimateRankChange() : " + scoreEntries + " : " + sortedP);
        //if (sortedP != scoreEntries)
        //{
            StartCoroutine(AnimateRankChange(sortedP));
        //}
    }

    private IEnumerator AnimateRankChange(List<ScoreEntry> sortedP)
    {
        Debug.Log("AnimateRankChange Entry");
        yield return null;
        
        Dictionary<GameObject, float> targetPositions = new Dictionary<GameObject, float>();
        // Gets the current y value of each entry sorted by score
        foreach (var player in sortedP)
        {
            targetPositions[player.GetGameObject()] = player.GetGameObject().transform.localPosition.y;
        }
        Debug.Log("AnimateRankChange Pos1");
        
        float elapsed = 0f;

        //Dictionary<GameObject, float> startPositions = new Dictionary<GameObject, float>();

        while (elapsed < moveDuration)
        // Iterates through animation time duration
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            // interpolates all entries, moving 1.y to 2.y and vice versa
            // if no change, moves 1.y to 1.y, etc.
            // needs updating to only interpolate relevant entries, and only call on relevant score update
            foreach (var player in sortedP)
            {
                GameObject playerEntry = player.GetGameObject();
                float startY = player.GetY();
                float targetY = targetPositions[playerEntry];

                playerEntry.transform.localPosition = new Vector3(
                    playerEntry.transform.localPosition.x,
                    Mathf.Lerp(startY, targetY, t),
                    playerEntry.transform.localPosition.z);
            }

            yield return null;
        }
        Debug.Log("AnimateRankChange While Complete");

        // After animation is done, iterates through sortedPlayers and assigns their rank to their index + 1
        // Really, each entry should be an object that can be easily updated, rework that next!
        for (int i = 0; i < sortedP.Count; i++)
        {
            sortedP[i].SetRank(i+1);
        }
        Debug.Log("AnimateRankChange Complete");
    }

//     public void RemovePlayer(string playerName)
//     {
//         if (playerEntries.ContainsKey(playerName))
//         {
//             Destroy(playerEntries[playerName].entry);
//             playerEntries.Remove(playerName);
//         }
//     }
}