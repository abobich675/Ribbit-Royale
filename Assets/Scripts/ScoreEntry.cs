using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry
{

    private Color entryColor;
    private int entryRank;
    private int entryScore;
    private Sprite entryAvatar;
    private string entryPlayerName;
    private GameObject entryGameObject;
    private GameObject inGameEntryGameObject;
    private float yPos;
    private Sprite entryRankImage;
    private bool isShown;

    private ScoreEntryUI uiController;
    private ScoreEntryUI uiController_inGame;

    public void Initialize()
    {
        uiController = entryGameObject.GetComponent<ScoreEntryUI>();
        uiController_inGame = inGameEntryGameObject.GetComponent<ScoreEntryUI>();
        entryColor = Color.white;
        entryRank = 0;
        entryScore = 0;
        //entryPlayerName = "default";
        //entryGameObject = null;
        yPos = 0;
        isShown = true;
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
        return inGameEntryGameObject;
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
        return entryGameObject;
    }

    public void SetGameObject(GameObject gameObject)
    {
        entryGameObject = gameObject;
    }
    
    // get, set name
    public string GetPlayerName()
    {
        return entryPlayerName;
    }

    public void SetPlayerName(string playerName)
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
        uiController.avatarBorder.color = entryColor;
        uiController_inGame.avatarBorder.color = entryColor;
    }
    
    // get, set avatar
    public Sprite GetAvatar()
    {
        return entryAvatar;
    }

    public void SetAvatar(Sprite newAvatar)
    {
        Debug.Log("SetAvatar: " + newAvatar);
        entryAvatar = newAvatar;
        uiController.avatarImage.sprite = entryAvatar;
        uiController_inGame.avatarImage.sprite = entryAvatar;
    }
    
    // get, set rank
    public int GetRank()
    {
        return entryRank;
    }

    public void SetRank(Sprite rank)
    {
        //Debug.Log("SetRank: " + rank);
        //entryRank = rank;
        entryRankImage = rank;
        //uiController.rankText.text = entryRank.ToString();
        uiController.rankImg.sprite = entryRankImage;
        //uiController_inGame.rankText.text = entryRank.ToString();
    }
    
    // get, set score
    public int GetScore()
    {
        return entryScore;
    }

    public void SetScore(int score)
    {
        Debug.Log("SetScore: " + score);
        entryScore = score;
        uiController.scoreText.text = entryScore.ToString();
        uiController_inGame.scoreText.text = entryScore.ToString();
    }
}