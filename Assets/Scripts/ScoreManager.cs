using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    
    public Text scoreText;

    int score = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        scoreText.text = score.ToString() + " POINTS";
    }

    public void AddPoint()
    {
        Debug.Log("Score Manager - incrementing score");
        score += 1;
        scoreText.text = score.ToString() + " POINTS";
        PlayerPrefs.SetInt("roundScore", score);
    }
}