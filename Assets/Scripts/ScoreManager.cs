using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
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

        private GameObject scoreboardContent;
        private GameObject inGameScoreboardContent;

        private Transform ScoreTransform;
        
        // private Transform scoreboardContent;
        // private Transform inGameScoreboardContent;
        public float moveDuration = 0.5f;

        public Sprite spriteRed;
        public Sprite spriteBlue;
        public Sprite spriteGreen;
        public Sprite spriteYellow;
    
        public Sprite rank1;
        public Sprite rank2;
        public Sprite rank3;
        public Sprite rank4;

        private List<GameObject> playerObjectList = new List<GameObject>();
        
        private Color blue = new Vector4(.05f, .19f, .47f, 1f);
        private Color purple = new Vector4(0.29f, .04f, .63f, 1f);
        private Color green = new Vector4(.52f, .84f, .22f, 1f);
        private Color red = new Vector4(.55f, .03f, .17f, 1f);
        private Color yellow = new Vector4(.84f, .76f, .117f, 1f);

        private float timeRemainingGlobal = 1;
        
        private string timeLeftCounter = "null";
        private List<ScoreEntry> completePlayerList = new List<ScoreEntry>();
        

        private int gameType;
        
        public bool isTimerEnabled;
        public int TIMER_DURATION;

        // colorId (Color, Sprite) -- 003 (Red, RedSprite)
        private Dictionary<int, (Color, Sprite)> colorSpriteDictionary = new Dictionary<int, (Color, Sprite)>();
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
            // if (gameType == 1)
            // {
            //     StartCoroutine(CreateTimer(TIMER_DURATION));
            //     Debug.Log("Started Timer...");
            // }
        }

        // should probably call init function on each game start to reset stuff/set up appropriate update loop
        private void Update()
        {
            // if (gameType == 0)
            // { }
            // else
            // { GetDistanceToFinish(); }
            // Debug.Log("Update Called");
        }

        private void InitPlayerDataList()
        {
            //PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        }

        public void StartDistanceToFinishCoroutine()
        {
            Debug.Log("StartDistance coroutine started...");
            StartCoroutine(GetDistanceToFinish());
        }
        
        private IEnumerator GetDistanceToFinish()
        {
            // will want to create a seperate list of players 'in' the game/alive/not beaten yet
            while (timeRemainingGlobal > 0)
            {
                foreach (var entry in scoreEntries)
                {
                    if (completePlayerList.Contains(entry)) { continue; }
                    //if (entry.GetPlayerGameObject())
                    Vector2 playerVec = entry.GetPlayerLocation();
                    Vector2 finishVec = new Vector2(29.6f, 94.8f);
                    // sqrt[ ( a^2 + b^2) ] = c
                    var distanceAway =
                        Mathf.Sqrt(Mathf.Pow((finishVec.x - playerVec.x), 2) + Mathf.Pow((finishVec.y - playerVec.y), 2));
                    if (distanceAway <= 5)
                    {
                        entry.SetScore(-1, timeLeftCounter);
                        completePlayerList.Add(entry);
                        UpdatePlayerRank(entry, completePlayerList.Count);
                    }
                    else
                    {
                        entry.SetScore((int)distanceAway);
                    }
                }
                yield return null;
            }
            yield return null;
        }

        private IEnumerator CreateTimer(int duration, float timerStartDelay)
        {
            inGameTimerPrefab = Resources.Load<GameObject>("InGameTimer");
            var inGameTimer = Instantiate(inGameTimerPrefab, inGameScoreboardContent.transform);
            inGameTimer.transform.SetSiblingIndex(0);
            var timerUIController = inGameTimer.GetComponent<TimerUI>();
            float timeTracker = 0;
            float timeRemaining = duration;
            
            timerUIController.currentTime.text = GetTimerUpdateString(timeRemaining);
            timerUIController.currentTime.color = Color.black;
            
            yield return new WaitForSeconds(timerStartDelay);
            while (timeRemaining > 0)
            {
                timeTracker += Time.deltaTime;
                if (timeTracker >= 1)
                {
                    //Debug.Log(timeTracker + "||" + timeRemaining);
                    if (timeRemaining <= 11)
                    {
                        timerUIController.currentTime.color = Color.red;
                        if (timeRemaining <= 1)
                        {
                            //Debug.Log("Timer Complete. Should terminate/end minigame, go to score screen.");
                            timerUIController.currentTime.text = "0:00";
                            timeLeftCounter = "0:00";
                            foreach (var entry in completePlayerList)
                            {
                                entry.SetScore(-1, "DNF");
                            }

                            Debug.Log("Out of time! Ending Minigame...");
                            yield return new WaitForSeconds(3f);
                            
                            ScoreController scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();;
                            scoreController.CalculatePlayerScores();
                            break;
                        }
                    }

                    timeTracker = 0;
                    timeRemaining -= 1;
                    timeRemainingGlobal = timeRemaining;
                    timerUIController.currentTime.text = GetTimerUpdateString(timeRemaining);
                    timeLeftCounter = timerUIController.currentTime.text;
                }
                yield return null;
            }
        

            yield return null;
        }

        private string GetTimerUpdateString(float timeRemaining)
        {
            float minutesRemaining = Mathf.Floor(timeRemaining / 60);
            float secondsRemaining = Mathf.Floor(timeRemaining % 60);
            string minutesRemainingText = "0";
            string secondsRemainingText = "0";
            
            minutesRemainingText = minutesRemaining.ToString();
            secondsRemainingText = secondsRemaining.ToString();
            
            if (minutesRemaining == 0)
            {
                minutesRemainingText = "0";
            }
            
            if (secondsRemaining < 10)
            {
                secondsRemainingText = "0" + secondsRemaining;
            }

            return minutesRemainingText + ":" + secondsRemainingText;
        }

        private void CreateExampleInstance(ScoreManager scoreboard = null)
        {
            colorSpriteDictionary.Add(0, (Color.red, spriteRed));       // red
            colorSpriteDictionary.Add(1, (blue, spriteBlue));             // blue
            colorSpriteDictionary.Add(2, (Color.green, spriteGreen));   // green
            colorSpriteDictionary.Add(3, (Color.yellow, spriteYellow)); // yellow
            // playerColorDictionary.Add("Player0", "red");
            // playerColorDictionary.Add("Player1", "blue");
            // playerColorDictionary.Add("Player2", "yellow");
            // playerColorDictionary.Add("Player3", "green");
            //var getColor = RibbitRoyaleMultiplayer.GetPlayerColor(0);
            // scoreboard.UpdatePlayerScore("Player0", 29, false);
            // scoreEntries[0].SetPlayerGameObject(player0);
            // scoreboard.UpdatePlayerScore("Player1", 30, false);
            // scoreboard.UpdatePlayerScore("Player2", 15, false);
            // scoreboard.UpdatePlayerScore("Player3", 31, false);

            // CreatePlayerEntry(00001, 0, 00000, 1);
            //
            // scoreboard.UpdateRanking();
            // Debug.Log("ScoreManager example instance initialization complete.");
        }

        public void SetupScoreboard(Transform parent, int boardType, int playerCount, int timerDuration = 0, float timerStartDelay = 0f)
        {
            gameType = boardType;
            
            spriteRed = Resources.Load<Sprite>("frog_sprites/Red Idle");
            spriteBlue = Resources.Load<Sprite>("frog_sprites/Blue Idle");
            spriteGreen = Resources.Load<Sprite>("frog_sprites/Green Idle");
            spriteYellow = Resources.Load<Sprite>("frog_sprites/Yellow Idle");
            colorSpriteDictionary.Add(1, (red, spriteRed));       // red
            colorSpriteDictionary.Add(2, (blue, spriteBlue));             // blue
            colorSpriteDictionary.Add(0, (green, spriteGreen));   // green
            colorSpriteDictionary.Add(3, (yellow, spriteYellow)); // yellow
            //colorSpriteDictionary.Add(2, (purple, spritePurple)); // purple 

            rank1 = Resources.Load<Sprite>("ui_sprites/1");
            rank2 = Resources.Load<Sprite>("ui_sprites/2");
            rank3 = Resources.Load<Sprite>("ui_sprites/3");
            rank4 = Resources.Load<Sprite>("ui_sprites/4");
            
            
            if (boardType == 0)
            {
                // boardType == 0 -> RoundScoreboard
                 scoreboardContent = Instantiate(Resources.Load<GameObject>("Scoreboard"), parent);
            }
            else
            {
                // boardType == 1/else -> InGameScoreboard
                inGameScoreboardContent = Instantiate(Resources.Load<GameObject>("InGameScoreboard"), parent);
                StartCoroutine(CreateTimer(timerDuration, timerStartDelay));
            }

            while (playerCount > 0)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                playerObjectList.Add(playerObj);
                playerCount--;
            }
        }

        private ScoreEntry AddPlayer(ulong playerId, int boardType)
        {
            // Creates new ScoreEntry object, sets parameters, adds to scoreEntries list, returns scoreEntry object
            ScoreEntry scoreEntry = new ScoreEntry();
            scoreEntry.SetPlayerName(playerId);
            if (boardType == 0)
            {
                // Round Scoreboard
                scoreEntryPrefab = Resources.Load<GameObject>("ScoreEntry");
                scoreEntry.SetGameObject(Instantiate(scoreEntryPrefab, scoreboardContent.transform));
            }
            else
            {
                // In Game Scoreboard
                inGameScoreEntryPrefab = Resources.Load<GameObject>("InGameScoreEntry");
                scoreEntry.SetInGameGameObject(Instantiate(inGameScoreEntryPrefab, inGameScoreboardContent.transform));    
            }
            scoreEntry.SetEntryType(boardType);
            scoreEntry.Initialize(boardType);
            scoreEntries.Add(scoreEntry);
            //scoreEntry.SetEntryActive(alwaysShowScoreboard);
            return scoreEntry;
        }

        public void CreatePlayerEntry(ulong clientId, int score, int colorId, int boardType, int prevScore = 0)
        {
            ScoreEntry scoreEntry = AddPlayer(clientId, boardType);
            if (boardType == 1)
            {
                foreach (var player in playerObjectList)
                {
                    var gameObjectClientId = player.GetComponent<NetworkObject>().OwnerClientId;
                    if (gameObjectClientId == clientId)
                    {
                        Debug.Log("Set Player Entry GO : clientId: " + clientId + " | gameObjectClientId: " + gameObjectClientId);
                        scoreEntry.SetPlayerGameObject(player);
                    }
                }
            } else if (gameType == 0)
            {
                UpdatePlayerRank(scoreEntry, prevScore);
            }
            UpdateEntryColors(scoreEntry, 
                colorSpriteDictionary[colorId].Item1, colorSpriteDictionary[colorId].Item2);
            UpdateScoreText(scoreEntry, score);
            
        }

        public List<ScoreEntry> GetEntryList()
        {
            return scoreEntries;
        }

        public void UpdatePlayerRank(ScoreEntry entry, int rank)
        {
            switch (rank)
            {
                case 1:
                    entry.SetRank(rank1, 1);
                    return;
                case 2:
                    entry.SetRank(rank2, 2);
                    return;
                case 3:
                    entry.SetRank(rank3, 3);
                    return;
                case 4:
                    entry.SetRank(rank4, 4);
                    return;
            }
        }

        public void UpdatePlayerScore(ulong playerId, int score, bool isIncrement)
        {
            foreach (ScoreEntry entry in scoreEntries)
            {
                if (entry.GetPlayerName() == playerId)
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
        }

        public int GetPlayerScore(ulong playerId)
        {
            foreach (ScoreEntry entry in scoreEntries)
            {
                if (entry.GetPlayerName() == playerId)
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
        
            // Should only be enabled if doing roundscore
            //UpdateRanking();
        }

        public void UpdateRanking()
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
            StartCoroutine(AnimateRankChange(sortedP));
        }

        private IEnumerator AnimateRankChange(List<ScoreEntry> sortedP)
        {
            //Debug.Log("AnimateRankChange Entry");
            yield return null;
        
            Dictionary<GameObject, float> targetPositions = new Dictionary<GameObject, float>();
            // Gets the current y value of each entry sorted by score
            foreach (var player in sortedP)
            {
                targetPositions[player.GetGameObject()] = player.GetGameObject().transform.localPosition.y;
            }
            //Debug.Log("AnimateRankChange Pos1");
        
            float elapsed = 0f;

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
            //Debug.Log("AnimateRankChange While Complete");

            // After animation is done, iterates through sortedPlayers and assigns their rank to their index + 1
            // Really, each entry should be an object that can be easily updated, rework that next!
            for (int i = 0; i < sortedP.Count; i++)
            {
                //sortedP[i].SetRank(i+1);
                UpdatePlayerRank(sortedP[i], i+1);
            }
            Debug.Log("AnimateRankChange Complete");
        }
    }
}