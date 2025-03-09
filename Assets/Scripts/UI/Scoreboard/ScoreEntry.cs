using UnityEditor;
using UnityEngine;

namespace UI.Scoreboard
{
    public class ScoreEntry
    {

        private Color entryColor;
        private int entryRank;
        private int entryScore;
        private string entryScoreString;
        private Sprite entryAvatar;
        private ulong entryPlayerName;
        private GameObject entryGameObject;
        private GameObject inGameEntryGameObject;
        private GameObject playerGameObject = null;
        private float yPos;
        private Sprite entryRankImage;
        private bool isShown;
        private bool isTest = false;
        private int entryType;

        private ScoreEntryUI uiController;
        private ScoreEntryUI uiController_inGame;

        public ScoreEntry()
        {
            //Initialize();
        }

        public void SetEntryType(int entryGameType)
        {
            entryType = entryGameType;
        }

        public void Initialize(int boardType)
        {
            if (boardType == 0)
            {
                uiController = entryGameObject.GetComponent<ScoreEntryUI>();
            }
            else
            {
                uiController_inGame = inGameEntryGameObject.GetComponent<ScoreEntryUI>();
            }
            entryColor = Color.white;
            entryRank = 0;
            entryScore = 0;
            //entryPlayerName = "default";
            //entryGameObject = null;
            yPos = 0;
            isShown = true;
        }

        public void SetPlayerGameObject(GameObject playerGO)
        {
            playerGameObject = playerGO;
        }

        public GameObject GetPlayerGameObject()
        {
            return playerGameObject;
        }

        public Vector2 GetPlayerLocation()
        {
            if (!playerGameObject)
            {
                return new Vector2(0, 0);
            }
            return playerGameObject.transform.localPosition;
        }

        public void SetIsTest(bool updateIsTest)
        {
            isTest = updateIsTest;
        }

        public void SetEntryActive(bool isActive)
        {
            isShown = isActive;
            entryGameObject.SetActive(false);
        }

        public void SetInGameGameObject(GameObject inGameObject)
        {
            inGameEntryGameObject = inGameObject;
        }
    
        public GameObject GetInGameGameObject()
        {
            if (inGameEntryGameObject)
            {
                return inGameEntryGameObject;
            }

            return null;
        }
    
    
        // get, set ypos
        public float GetY()
        {
            return yPos;
        }

        public void SetY(float newY)
        {
            yPos = newY;
        }
    
    
        // get, set gameobject
        public GameObject GetGameObject()
        {
            if (entryGameObject)
            {
                return entryGameObject;
            }

            return null;
        }

        public void SetGameObject(GameObject gameObject)
        {
            entryGameObject = gameObject;
        }
    
        // get, set name
        public ulong GetPlayerName()
        {
            return entryPlayerName;
        }

        public void SetPlayerName(ulong playerName)
        {
            entryPlayerName = playerName;
        }

        // get, set color
        public Color GetEntryColor()
        {
            return entryColor;
        }

        public void SetEntryColor(Color color)
        {
            entryColor = color;
            if (entryType == 0)
            {
                uiController.avatarBorder.color = entryColor;
            }
            else
            {
                uiController_inGame.avatarBorder.color = entryColor;
            }
        }
    
        // get, set avatar
        public Sprite GetAvatar()
        {
            return entryAvatar;
        }

        public void SetAvatar(Sprite newAvatar)
        {
            //Debug.Log("SetAvatar: " + newAvatar);
            entryAvatar = newAvatar;
            if (entryType == 0)
            {
                uiController.avatarImage.sprite = entryAvatar;
            }
            else
            {
                uiController_inGame.avatarImage.sprite = entryAvatar;
            }
        }
    
        // get, set rank
        public int GetRank()
        {
            return entryRank;
        }
        
        public Sprite GetRankImg()
        {
            return entryRankImage;
        }

        public void SetRank(Sprite rank, int rankNum)
        {
            //Debug.Log("SetRank: " + rank);
            //entryRank = rank;
            entryRank = rankNum;
            entryRankImage = rank;
            if (entryType == 0)
            {
                //uiController.rankText.text = entryRank.ToString();
                uiController.rankImg.sprite = entryRankImage;
                uiController.rankImg.color = Color.white;
            }
            else
            {
                //uiController_inGame.rankText.text = entryRank.ToString();
            }
        }
    
        // get, set score
        public int GetScore()
        {
            return entryScore;
        }
        
        public string GetScoreString()
        {
            return entryScoreString;
        }

        public void SetScore(int score, string optionalScore = "null")
        {
            //Debug.Log("SetScore: " + score);
            entryScore = score;
            if (score == -1)
            {
                uiController_inGame.scoreText.text = optionalScore;
                entryScoreString = optionalScore;
            } else if (!isTest)
            {
                if (entryType == 0)
                {
                    uiController.scoreText.text = entryScore.ToString();
                }
                else
                {
                    uiController_inGame.scoreText.text = entryScore.ToString();
                }
            }
        }
        
        
    }
}