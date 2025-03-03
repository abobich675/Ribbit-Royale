using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class CTAMain : MonoBehaviour
{

    public float GAME_LENGTH = 10;
    public float MIN_SPAWN_DELAY = 0.1f;
    public float MAX_SPAWN_DELAY = 1f;



    // Animal struct for each animal type
    // Contains the name of the animal, the prefab of the animal, and the count of the animal
    [Serializable]
    public struct Animal
    {
        public string name;
        public GameObject animalPrefab;
        public int count;
    }



    // Barriers
    public GameObject leftBarrier;
    public GameObject rightBarrier;

    // Final Counted Animals Text
    public TMPro.TextMeshProUGUI finalCountedAnimalsText;

    // Chosen animal for the players to count and it's image
    string countedAnimal = "";
    public Image countedAnimalImage;

    // Array of animals
    public Animal[] animals;


    bool gameActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameActive = true;
        finalCountedAnimalsText.text = "";
        finalCountedAnimalsText.gameObject.SetActive(false);

        ChooseAnimalToCount();

        // Start the game timer
        Invoke("StopSpawning", GAME_LENGTH - 3);
        Invoke("EndGame", GAME_LENGTH);

        Invoke("SummonRandomAnimal", MAX_SPAWN_DELAY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Animal GetAnimal(string name) {
        foreach (Animal animal in animals) {
            if (animal.name == name) {
                return animal;
            }
        }
        return new Animal();
    }

    
    // Randomly select an animal to count
    void ChooseAnimalToCount()
    {
        int animalIndex = Random.Range(0, 5);
        countedAnimal = animals[animalIndex].name;
        countedAnimalImage.sprite = animals[animalIndex].animalPrefab.GetComponent<SpriteRenderer>().sprite;
    }

    // Summon a Random Animal
    // Invokes this function again to spawn a random animal at a delay
    void SummonRandomAnimal()
    {
        // Randomly select an animal to spawn
        int animalIndex = Random.Range(0, 5);
        SummonAnimal(animals[animalIndex].animalPrefab, animals[animalIndex].name);
        animals[animalIndex].count++;

        // Randomly select a time to wait before spawning the next animal
        float delay = Random.Range(MIN_SPAWN_DELAY, MAX_SPAWN_DELAY);
        Invoke("SummonRandomAnimal", delay);
    }

    // Summon an animal of a specific type
    // Instantiates the animal and sets the animal's script
    void SummonAnimal(GameObject animalPrefab, string animalType)
    {
        GameObject animal = Instantiate(animalPrefab);
        CTAAnimalScript animalScript = animal.GetComponent<CTAAnimalScript>();
        animalScript.animalType = animalType;
        animalScript.leftBarrier = leftBarrier;
        animalScript.rightBarrier = rightBarrier;
    }

    // Stop spawning animals
    // To be called briefly before the end of the game, 
    void StopSpawning()
    {
        CancelInvoke("SummonRandomAnimal");
    }

    // Ends the game
    private void EndGame()
    {
        // Stop the game
        gameActive = false;

        // Display the final counted animals
        int finalCount = GetAnimal(countedAnimal).count;
        finalCountedAnimalsText.gameObject.SetActive(true);
        finalCountedAnimalsText.text = finalCount.ToString();

        foreach (Animal animal in animals) {
            Debug.Log(animal.name + " count: " + animal.count);
        }

        //Invoke("ReturnToLobby", 3);
        
        var scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
        scoreController.CTA_CalculatePlayerScores(GetComponent<CTAPlayerConroller>().counter, finalCount);
    }

    private void ReturnToLobby() {
        try {
            Loader.LoadNetwork(Loader.Scene.PreLobbyScene);
        } catch (Exception e) {
            Debug.Log("Error: " + e.Message);
        }
    }

    public bool GameActive() {
        return gameActive;
    }
}
