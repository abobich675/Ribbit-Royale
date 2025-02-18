using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry
{

    private Color entryColor;
    private int entryRank;
    private int entryScore;
    private Image entryAvatar;
    private string entryPlayerName;
    private GameObject entryGameObject;
    private float yPos;

    private ScoreEntryUI uiController;

    public void Initialize()
    {
        uiController = entryGameObject.GetComponent<ScoreEntryUI>();
        entryColor = Color.white;
        entryRank = 0;
        entryScore = 0;
        //entryPlayerName = "default";
        //entryGameObject = null;
        yPos = 0;
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
    }
    
    // get, set avatar
    public Image GetAvatar()
    {
        return entryAvatar;
    }

    public void SetAvatar(Image newAvatar)
    {
        entryAvatar = newAvatar;
        uiController.avatarImage = entryAvatar;
    }
    
    // get, set rank
    public int GetRank()
    {
        return entryRank;
    }

    public void SetRank(int rank)
    {
        entryRank = rank;
        uiController.rankText.text = entryRank.ToString();
    }
    
    // get, set score
    public int GetScore()
    {
        return entryScore;
    }

    public void SetScore(int score)
    {
        entryScore = score;
        uiController.scoreText.text = entryScore.ToString();
    }
}