using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace UI.Scoreboard
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
    
        public GameObject scoreEntryPrefab;
        public GameObject inGameScoreEntryPrefab;
        public GameObject inGameTimerPrefab;
        public Transform scoreboardContent;
        public Transform inGameScoreboardContent;
        public float moveDuration = 10.0f;

        public Sprite spriteRed;
        public Sprite spriteBlue;
        public Sprite spriteGreen;
        public Sprite spriteYellow;
    
        public Sprite rank1;
        public Sprite rank2;
        public Sprite rank3;
        public Sprite rank4;

        public GameObject player0;
        private string timeLeftCounter = "null";
        private List<ScoreEntry> completePlayerList = new List<ScoreEntry>();

        public bool enableScoreboard = true;

        // 0 == default, 1 == tongueSwing
        public int gameType = 1;

        public bool alwaysShowScoreboard;
        public bool isTimerEnabled;
        public int TIMER_DURATION;

        private Dictionary<string, (Color, Sprite)> colorSpriteDictionary = new Dictionary<string, (Color, Sprite)>();
        private Dictionary<string, string> playerColorDictionary = new Dictionary<string, string>();

        //private Dictionary<string, (int score, GameObject entry)> playerEntries = new Dictionary<string, (int, GameObject)>();
        private List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

        public ScoreManager()
        {
            //Awake();
            //Start();
        }

        private void Awake()
        {
            Instance = this;
        }
    
        private void Start()
        {
            if (enableScoreboard)
            {
                Debug.Log("ScoreManager Start() executing...");
                ScoreManager scoreboard = ScoreManager.Instance;
                CreateExampleInstance(scoreboard);
                if (isTimerEnabled)
                {
                    StartCoroutine(CreateTimer(TIMER_DURATION));
                    Debug.Log("Started Timer...");
                }
            }
        }

        // should probably call init function on each game start to reset stuff/set up appropriate update loop
        private void Update()
        {
            switch (gameType)
            {
                case 0: // default 
                    break;
                case 1: // tongue swing game
                    GetDistanceToFinish();
                    break;
            }
        }

        private void GetDistanceToFinish()
        {
            // will want to create a seperate list of players 'in' the game/alive/not beaten yet
            foreach (ScoreEntry entry in scoreEntries)
            {
                if (!entry.GetPlayerGameObject()) { continue; }
                if (completePlayerList.Contains(entry)) { continue; }
                Vector2 playerVec = entry.GetPlayerLocation();
                Vector2 finishVec = new Vector2(29.6f, 94.8f);
                // sqrt[ ( a^2 + b^2) ] = c
                var distanceAway =
                    Mathf.Sqrt(Mathf.Pow((finishVec.x - playerVec.x), 2) + Mathf.Pow((finishVec.y - playerVec.y), 2));
                if (distanceAway <= 1)
                {
                    entry.SetScore(-1, timeLeftCounter);
                    completePlayerList.Add(entry);
                }
                else
                {
                    entry.SetScore((int)distanceAway);
                }
            }
        }

        private IEnumerator CreateTimer(int duration)
        {
            var inGameTimer = Instantiate(inGameTimerPrefab, inGameScoreboardContent);
            inGameTimer.transform.SetSiblingIndex(2);
            var timerUIController = inGameTimer.GetComponent<TimerUI>();
            float timeTracker = 0;
            float timeRemaining = duration;
            timerUIController.currentTime.text = (Mathf.Floor(timeRemaining / 60) + ":" + Mathf.Floor(timeRemaining % 60));
            timerUIController.currentTime.color = Color.black;
            while (timeRemaining > 0)
            {
                timeTracker += Time.deltaTime;
                if (timeTracker >= 1)
                {
                    //Debug.Log(timeTracker + "||" + timeRemaining);
                    if (timeRemaining <= 1)
                    {
                        //Debug.Log("Timer Complete. Should terminate/end minigame, go to score screen.");
                        timerUIController.currentTime.text = "0:00";
                        timerUIController.currentTime.color = Color.red;
                        timeLeftCounter = "0:00";
                        break;
                    }
                    timeTracker = 0;
                    timeRemaining -= 1;
                    timerUIController.currentTime.text = (Mathf.Floor(timeRemaining / 60) + ":" + Mathf.Floor(timeRemaining % 60));
                    timeLeftCounter = timerUIController.currentTime.text;
                }
                yield return null;
            }
        

            yield return null;
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
            scoreEntries[0].SetPlayerGameObject(player0);
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
            scoreEntry.SetEntryActive(alwaysShowScoreboard);
            return scoreEntry;
        }

        public void UpdatePlayerRank(ScoreEntry entry, int rank)
        {
            switch (rank)
            {
                case 1:
                    entry.SetRank(rank1);
                    return;
                case 2:
                    entry.SetRank(rank2);
                    return;
                case 3:
                    entry.SetRank(rank3);
                    return;
                case 4:
                    entry.SetRank(rank4);
                    return;
            }
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

        public int GetPlayerScore(string playerName)
        {
            foreach (ScoreEntry entry in scoreEntries)
            {
                if (entry.GetPlayerName() == playerName)
                {
                    return entry.GetScore();
                }
            }

            return -1;
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
            var sortedP = scoreEntries;

            // Sorts sortedPlayers list by (a, b), and (b score CompareTo a score)
        
            sortedP.Sort((a, b) => b.GetScore().CompareTo(a.GetScore()));
        
            // Creates an array startPositions of player-entry:y-value

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
                //sortedP[i].SetRank(i+1);
                UpdatePlayerRank(sortedP[i], i+1);
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
}