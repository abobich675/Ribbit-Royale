using UnityEngine;

public class PLGameCollision : MonoBehaviour
{
    public PLChooseGame gameChooser;
    public PLChooseGame.Game gameInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameChooser.AddGame(gameInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("Player touched game collider");
            gameInfo.count++;
            gameChooser.UpdateGameCount(gameInfo.name, gameInfo.count);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("Player left game collider");
            gameInfo.count--;
            gameChooser.UpdateGameCount(gameInfo.name, gameInfo.count);
        }
    }
}
