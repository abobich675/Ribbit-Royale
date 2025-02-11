using UnityEditor.Rendering;
using UnityEngine;

public class CTAMain : MonoBehaviour
{

    public float GAME_LENGTH = 10;
    public float MIN_SPAWN_DELAY = 0.1f;
    public float MAX_SPAWN_DELAY = 1f;

    // Barriers
    public GameObject leftBarrier;
    public GameObject rightBarrier;

    // Animal Prefabs
    public GameObject birdPrefab;
    public GameObject flyPrefab;
    public GameObject monkeyPrefab;
    public GameObject snakePrefab;
    public GameObject turtlePrefab;

    // Count the number of animals as they're created
    public int birdCount = 0;
    public int flyCount = 0;
    public int monkeyCount = 0;
    public int snakeCount = 0;
    public int turtleCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start the game timer
        Invoke("EndGame", GAME_LENGTH);

        Invoke("SummonRandomAnimal", MAX_SPAWN_DELAY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SummonRandomAnimal()
    {
        // Randomly select an animal to spawn
        int animalIndex = Random.Range(0, 5);
        switch (animalIndex)
        {
            case 0:
                SummonAnimal(flyPrefab, "fly");
                flyCount++;
                break;
            case 1:
                SummonAnimal(flyPrefab, "bird");
                birdCount++;
                break;
            case 2:
                SummonAnimal(flyPrefab, "monkey");
                monkeyCount++;
                break;
            case 3:
                SummonAnimal(flyPrefab, "turtle");
                turtleCount++;
                break;
            case 4:
                SummonAnimal(flyPrefab, "snake");
                snakeCount++;
                break;
        }

        // Randomly select a time to wait before spawning the next animal
        float delay = Random.Range(MIN_SPAWN_DELAY, MAX_SPAWN_DELAY);
        Invoke("SummonRandomAnimal", delay);
    }

    void SummonAnimal(GameObject animalPrefab, string animalType)
    {
        GameObject animal = Instantiate(animalPrefab);
        CTAAnimalScript animalScript = animal.GetComponent<CTAAnimalScript>();
        animalScript.animalType = animalType;
        animalScript.leftBarrier = leftBarrier;
        animalScript.rightBarrier = rightBarrier;
    }

    void EndGame()
    {
        // Stop the game
        Time.timeScale = 0;

        // Display the results
        Debug.Log("Fly Count: " + flyCount);
        Debug.Log("Bird Count: " + birdCount);
        Debug.Log("Monkey Count: " + monkeyCount);
        Debug.Log("Turtle Count: " + turtleCount);
        Debug.Log("Snake Count: " + snakeCount);
    }
}
