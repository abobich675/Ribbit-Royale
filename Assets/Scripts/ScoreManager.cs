// Author - Ryan Dobkin
// Email - dobkinr@oregonstate.edu
// Last Updated - 3/9/25

// ScoreManager, a script which is spawned by ScoreController.
// When created, takes a given scoreboard type, 0 for round and 1 for in game
// Instantiates given elements, taking player data from RMM, maintains timer, rank animations,
// and or distance counter as the score. Updates round score via PlayerData.
// Automatically assigns correctly* colored score plates/avatars to relevant players.


using System;
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
    
        // GameObject definitions
        public GameObject scoreEntryPrefab;
        public GameObject inGameScoreEntryPrefab;
        public GameObject inGameTimerPrefab;
        private GameObject scoreboardContent;
        private GameObject inGameScoreboardContent;

        private Transform ScoreTransform;
        
        // Round Score animation duration
        public float moveDuration = 0.5f;

        
        // Sprite definitions
        public Sprite spriteRed;
        public Sprite spriteBlue;
        public Sprite spriteGreen;
        public Sprite spriteYellow;
    
        public Sprite rank1;
        public Sprite rank2;
        public Sprite rank3;
        public Sprite rank4;
        public Sprite rank_;
        
        
        // Color definitions
        private Color blue = new Vector4(.05f, .19f, .47f, 1f);
        private Color green = new Vector4(.52f, .84f, .22f, 1f);
        private Color red = new Vector4(.55f, .03f, .17f, 1f);
        private Color yellow = new Vector4(.84f, .76f, .117f, 1f);
        //private Color purple = new Vector4(0.29f, .04f, .63f, 1f);
        
        
        public bool stopCoroutines = false;
        
        private List<GameObject> playerObjectList = new List<GameObject>();

        
        private float timeRemainingGlobal = 1;
        
        private string timeLeftCounter = "null";
        private List<ScoreEntry> completePlayerList = new List<ScoreEntry>();
        
        private int gameType;
        
        public bool isTimerEnabled;
        public int TIMER_DURATION;

        // colorId (Color, Sprite) -- 003 (Red, RedSprite)
        private Dictionary<int, (Color, Sprite)> colorSpriteDictionary = new Dictionary<int, (Color, Sprite)>();
        private List<ulong> didFinishList = new List<ulong>();
        private List<ulong> disconnectList = new List<ulong>();

        //private Dictionary<string, (int score, GameObject entry)> playerEntries = new Dictionary<string, (int, GameObject)>();
        private List<ScoreEntry> scoreEntries = new List<ScoreEntry>();
        

        private void Awake()
        {
            Instance = this;
        }
        

        public void StartDistanceToFinishCoroutine()
        {
            Debug.Log("StartDistance coroutine started...");
            StartCoroutine(GetDistanceToFinish());
            StartCoroutine(CheckIfFinished());
        }
        
        private IEnumerator GetDistanceToFinish()
        {
            // will want to create a seperate list of players 'in' the game/alive/not beaten yet
            var finish = GameObject.FindGameObjectWithTag("FinishContainer");
            while (timeRemainingGlobal > 0)
            {
                if (stopCoroutines) { break; }
                foreach (var entry in scoreEntries)
                {
                    if (!entry.GetInGameGameObject()) { continue;}
                    if (completePlayerList.Contains(entry)) { continue; }
                    Vector2 playerVec = entry.GetPlayerLocation();
                    Vector2 finishVec = finish.transform.localPosition;
                    var distanceAway =
                        Mathf.Sqrt(Mathf.Pow((finishVec.x - playerVec.x), 2) + Mathf.Pow((finishVec.y - playerVec.y), 2));
                    entry.SetScore((int)distanceAway);
                }
                yield return null;
            }
            yield return null;
        }

        private IEnumerator CheckIfFinished()
        {
            Debug.Log("CHECKIFFINISHED ==== CompleteCount: " + completePlayerList.Count + "; EntriesCount: " + scoreEntries.Count );
            while (completePlayerList.Count < scoreEntries.Count)
            {
                if (stopCoroutines) { break; }

                foreach (var entry in scoreEntries)
                {
                    if (didFinishList.Contains(entry.GetPlayerName()) && !completePlayerList.Contains(entry))
                    {
                        completePlayerList.Add(entry);
                        entry.SetScore(-1, timeLeftCounter);
                        UpdatePlayerRank(entry, completePlayerList.Count);
                        Debug.Log("ScoreManager Set Finished playerId: " + entry.GetPlayerName());
                    }
                }
                
                if (completePlayerList.Count == scoreEntries.Count - disconnectList.Count)
                {
                    ScoreController scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
                    scoreController.CalculatePlayerScores();
                    break;
                }
                

                yield return new WaitForSeconds(0.1f);
                yield return null;
            }

            yield return null;
        }

        public void DidFinish(ulong playerId)
        {
            if (!didFinishList.Contains(playerId))
            {
                didFinishList.Add(playerId);
            }
        }

        private IEnumerator CreateTimer(int duration, float timerStartDelay)
        {
            // Creates a timer object as a Coroutine
            // Counts down from duration, stops if game finished
            // If the time runs out before game is finished, set unfinished player score to 'DNF'
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
                if (stopCoroutines) { break; }
                timeTracker += Time.deltaTime;
                if (timeTracker >= 1)
                {
                    if (timeRemaining <= 11)
                    {
                        timerUIController.currentTime.color = Color.red;
                        if (timeRemaining <= 1)
                        {
                            timerUIController.currentTime.text = "0:00";
                            timeLeftCounter = "0:00";
                            foreach (var entry in scoreEntries)
                            {
                                if (!completePlayerList.Contains(entry))
                                {
                                    entry.SetScore(-1, "DNF");
                                }
                            }

                            Debug.Log("Out of time! Ending Minigame...");
                            yield return new WaitForSeconds(1f);
                            
                            ScoreController scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
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

        public void SetPlayerEntryDNF(ulong playerId)
        {
            foreach (var entry in scoreEntries)
            {
                if (entry.GetPlayerName() == playerId)
                {
                    completePlayerList.Add(entry);
                    entry.SetScore(-1, "DNF");
                }
            }
        }

        public void GameOver_StopCoroutines()
        {
            stopCoroutines = true;
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

        public void SetupScoreboard(Transform parent, int boardType, int playerCount, int timerDuration = 0, float timerStartDelay = 0f)
        {
            gameType = boardType;
            
            // Load player icon sprites
            spriteRed = Resources.Load<Sprite>("frog_sprites/Red Idle");
            spriteBlue = Resources.Load<Sprite>("frog_sprites/Blue Idle");
            spriteGreen = Resources.Load<Sprite>("frog_sprites/Green Idle");
            spriteYellow = Resources.Load<Sprite>("frog_sprites/Yellow Idle");
            
            // Add player sprites + color into dictionary for color id
            // Dict <colorId, (Color, Sprite)>
            colorSpriteDictionary.Add(1, (red, spriteRed));         // red
            colorSpriteDictionary.Add(2, (blue, spriteBlue));       // blue
            colorSpriteDictionary.Add(0, (green, spriteGreen));     // green
            colorSpriteDictionary.Add(3, (yellow, spriteYellow));   // yellow
            //colorSpriteDictionary.Add(2, (purple, spritePurple)); // purple tbd

            // Load rank sprites
            rank1 = Resources.Load<Sprite>("ui_sprites/1_4x");
            rank2 = Resources.Load<Sprite>("ui_sprites/2_4x");
            rank3 = Resources.Load<Sprite>("ui_sprites/3_4x");
            rank4 = Resources.Load<Sprite>("ui_sprites/4_4x");
            rank_ = Resources.Load<Sprite>("ui_sprites/__4x");
            
            
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

            var objects = GameObject.FindGameObjectsWithTag("Player");
            foreach (var playerObj in objects)
            {
                playerObjectList.Add(playerObj);
                playerCount--;
                //Debug.Log("Recorded New Player with id: " + playerObj.GetComponent<NetworkObject>().OwnerClientId);
            }

            if (playerCount != 0)
            {
                //Debug.Log("PlayerCount != 0");
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
            // Updates ScoreEntry entry rankNum and rankImg values
            switch (rank)
            {
                case 0:
                    entry.SetRank(rank_, 0);
                    return;
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
        

        public void UpdateEntryColors(ScoreEntry entry, Color teamColor, Sprite avatar)
        {
            // Sets ScoreEntry entry avatar sprite and panel color
            entry.SetAvatar(avatar);
            entry.SetEntryColor(teamColor);
        }

        private void UpdateScoreText(ScoreEntry entry, int score)
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

        public void RemovePlayerEntry(ulong playerId)
        {
            foreach (ScoreEntry entry in scoreEntries)
            {
                Debug.Log("RemovePlayerEntry playerId: " + playerId + "; entry playerId: "+ entry.GetPlayerName());
                if (entry.GetPlayerName() == playerId)
                {
                    entry.SetConnected(false);
                    if (entry.GetGameObject())
                    {
                        Destroy(entry.GetGameObject());
                        //entry.GetGameObject().SetActive(false);
                        entry.SetGameObject(null);
                    }
                    else
                    {
                        Destroy(entry.GetInGameGameObject());
                        //entry.GetGameObject().SetActive(false);
                        entry.SetInGameGameObject(null);
                    }
                    disconnectList.Add(playerId);
                    Debug.Log("Destroyed Entry: " + playerId + " GameObject set null...");
                    //scoreEntries.Remove(entry);
                }
            }
        }
    }
}