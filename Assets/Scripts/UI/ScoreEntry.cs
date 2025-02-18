using UnityEngine;

public class ScoreEntry
{
    private Color entryColor;
    private int entryRank;
    private int entryScore;
    private string entryPlayerName;
    private GameObject entryGameObject;
    
    
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
    public Color GetColor()
    {
        return entryColor;
    }

    public void SetColor(Color color)
    {
        entryColor = color;
    }
    
    // get, set rank
    public int GetRank()
    {
        return entryRank;
    }

    public void SetRank(int rank)
    {
        entryRank = rank;
    }
    
    // get, set score
    public int GetScore()
    {
        return entryScore;
    }

    public void SetScore(int score)
    {
        entryScore = score;
    }
}