using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;


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
    }

    public void UpdatePlayerScore(string playerName, int score)
    {
        // if already existing profile
        if (playerEntries.ContainsKey(playerName))
        {
            playerEntries[playerName] = (score, playerEntries[playerName].entry);
            UpdateScoreText(playerEntries[playerName].entry, playerName, score);
        }
        else
        // if creating new user in leaderboard
        {
            Debug.Log("Instantiating new profile in scoreboard " + playerName);
            GameObject newEntry = Instantiate(scoreEntryPrefab, scoreboardContent);
            UpdateScoreText(newEntry, playerName, score);
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
        Debug.Log("Player not instantiated, ERROR");
    }

    private void UpdateScoreText(GameObject entry, string playerName, int score)
    // Updates the score field of a given player
    // Takes GameObject entry [player entry in scoreboard], playerName [string], score [int]
    {
        entry.transform.Find("RankText").GetComponent<Text>().text = "?";
        entry.transform.Find("PointsText").GetComponent<Text>().text = $"{score}";
        UpdateRanking();
    }

    private void UpdateRanking()
    {
        List<KeyValuePair<string, (int score, GameObject entry)>> sortedPlayers =
            new List<KeyValuePair<string, (int, GameObject)>>(playerEntries);
        sortedPlayers.Sort((a, b) => b.Value.score.CompareTo(a.Value.score));

        Dictionary<GameObject, float> startPositions = new Dictionary<GameObject, float>();
        foreach (var player in sortedPlayers)
        {
            startPositions[player.Value.entry] = player.Value.entry.transform.localPosition.y;
        }
        
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            sortedPlayers[i].Value.entry.transform.SetSiblingIndex(i);
        }

        StartCoroutine(AnimateRankChange(sortedPlayers, startPositions));
    }

    private IEnumerator AnimateRankChange(List<KeyValuePair<string, (int score, GameObject entry)>> sortedPlayers, Dictionary<GameObject, float> startPositions)
    {
        
        yield return null;
        
        Dictionary<GameObject, float> targetPositions = new Dictionary<GameObject, float>();
        foreach (var player in sortedPlayers)
        {
            targetPositions[player.Value.entry] = player.Value.entry.transform.localPosition.y;
        }
        
        float elapsed = 0f;

        //Dictionary<GameObject, float> startPositions = new Dictionary<GameObject, float>();

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

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