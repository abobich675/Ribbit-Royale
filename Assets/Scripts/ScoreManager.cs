using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public GameObject scoreEntryPrefab;
    public Transform scoreboardContent;
    public float moveDuration = 10.0f;

    public Sprite spriteRed;
    public Sprite spriteBlue;
    public Sprite spriteGreen;
    public Sprite spriteYellow;

    private Dictionary<string, (Color, Sprite)> colorSpriteDictionary = new Dictionary<string, (Color, Sprite)>();
    private Dictionary<string, string> playerColorDictionary = new Dictionary<string, string>();

    private Dictionary<string, (int score, GameObject entry)> playerEntries = new Dictionary<string, (int, GameObject)>();


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
        scoreboard.UpdatePlayerScore("Player0", 29);
        scoreboard.UpdatePlayerScore("Player1", 30);
        scoreboard.UpdatePlayerScore("Player2", 15);
        scoreboard.UpdatePlayerScore("Player3", 31);
        scoreboard.UpdateRanking();
        Debug.Log("ScoreManager example instance initialization complete.");
    }

    public void UpdatePlayerScore(string playerName, int score)
    {
        // if already existing profile
        if (playerEntries.ContainsKey(playerName))
        {
            playerEntries[playerName] = (score, playerEntries[playerName].entry);
            UpdateScoreText(playerEntries[playerName].entry, score);
        }
        else
        // if creating new user in leaderboard
        {
            Debug.Log("Instantiating new profile in scoreboard " + playerName);
            GameObject newEntry = Instantiate(scoreEntryPrefab, scoreboardContent);
            UpdateScoreText(newEntry, score);
            string playerColor = playerColorDictionary[playerName];
            var tupleColor = colorSpriteDictionary[playerColor];
            newEntry.transform.Find("AvatarImage").GetComponent<Image>().sprite = tupleColor.Item2;
            newEntry.transform.Find("AvatarBorder").GetComponent<Image>().color = tupleColor.Item1;
            playerEntries.Add(playerName, (score, newEntry));
        }
    }
    
    public void IncrementPlayerScore(string playerName, int amount)
    {
        if (playerEntries.ContainsKey(playerName))
        {
            int newScore = playerEntries[playerName].score + amount;
            UpdatePlayerScore(playerName, newScore);
        }
        else
        {
            Debug.Log("Player not instantiated, IncrementPlayerScore() ERROR");
        }
    }

    private void UpdateScoreText(GameObject entry, int score)
    // Updates the score field of a given player
    // Takes GameObject entry [player entry in scoreboard], playerName [string], score [int]
    {
        entry.transform.Find("RankText").GetComponent<Text>().text = "?";
        entry.transform.Find("PointsText").GetComponent<Text>().text = $"{score}";
        UpdateRanking();
    }

    private void UpdateRanking()
    {
        // Creates a list <string, (score, entry)> sortedPlayers
        List<KeyValuePair<string, (int score, GameObject entry)>> sortedPlayers =
            new List<KeyValuePair<string, (int, GameObject)>>(playerEntries);
        
        // Sorts sortedPlayers list by (a, b), and (b score CompareTo a score)
        sortedPlayers.Sort((a, b) => b.Value.score.CompareTo(a.Value.score));
        // x.CompareTo(y) will return 0 if sorted (e.g. x == y), -1 if preceding (e.g. x < y), +1 if should follow (e.g. x > y)
        
        // Creates an array startPositions of player-entry:y-value
        Dictionary<GameObject, float> startPositions = new Dictionary<GameObject, float>();
        foreach (var player in sortedPlayers)
        {
            startPositions[player.Value.entry] = player.Value.entry.transform.localPosition.y;
        }
        
        // Sets the siblingindex for each entry in sortedPlayers - reorders object hierarchy in scene 
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            sortedPlayers[i].Value.entry.transform.SetSiblingIndex(i);
        }

        Debug.Log("Rankings Sorted, Starting AnimateRankChange()");
        StartCoroutine(AnimateRankChange(sortedPlayers, startPositions));
    }

    private IEnumerator AnimateRankChange(List<KeyValuePair<string, (int score, GameObject entry)>> sortedPlayers, Dictionary<GameObject, float> startPositions)
    {
        
        yield return null;
        
        Dictionary<GameObject, float> targetPositions = new Dictionary<GameObject, float>();
        // Gets the current y value of each entry sorted by score
        foreach (var player in sortedPlayers)
        {
            targetPositions[player.Value.entry] = player.Value.entry.transform.localPosition.y;
        }
        
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
            foreach (var player in sortedPlayers)
            {
                GameObject playerEntry = player.Value.entry;
                float startY = startPositions[playerEntry];
                float targetY = targetPositions[playerEntry];

                playerEntry.transform.localPosition = new Vector3(
                    playerEntry.transform.localPosition.x,
                    Mathf.Lerp(startY, targetY, t),
                    playerEntry.transform.localPosition.z);
            }

            yield return null;
        }

        // After animation is done, iterates through sortedPlayers and assigns their rank to their index + 1
        // Really, each entry should be an object that can be easily updated, rework that next!
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            GameObject playerEntry = sortedPlayers[i].Value.entry;
            playerEntry.transform.Find("RankText").GetComponent<Text>().text = (i + 1).ToString();
        }
    }

    public void RemovePlayer(string playerName)
    {
        if (playerEntries.ContainsKey(playerName))
        {
            Destroy(playerEntries[playerName].entry);
            playerEntries.Remove(playerName);
        }
    }
}