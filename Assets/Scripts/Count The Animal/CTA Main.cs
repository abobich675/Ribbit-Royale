using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;

public class CTAMain : NetworkBehaviour
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

    struct Counter
    {
        public ulong clientId;
        public TextMeshProUGUI text;
    }

    // Prefab for player counts
    public GameObject playerCountPrefab;
    List<Counter> playerCounters = new List<Counter>();
    public Sprite[] playerSprites; // Green Blue Red Yellow

    // Array of animals
    public Animal[] animals;


    bool gameActive;

    private bool isHost;
    private int finalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreatePlayerCounters();
        
        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        ulong playerId = playerData.clientId;
        ulong ownerId = NetworkManager.Singleton.CurrentSessionOwner;
        isHost = playerId == ownerId;
        if (!isHost)
        {
            gameActive = true;
            finalCountedAnimalsText.text = "";
            finalCountedAnimalsText.gameObject.SetActive(false);
            
            PlayerData hostData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(ownerId);
            ChooseAnimalToCount(hostData.countedAnimalIndex);

            Invoke("EndGame", GAME_LENGTH);
            return;
        }

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
        UpdatePlayerCounters();
    }

    private void CreatePlayerCounters()
    {
        // Create a counter for each player
        int playerIndex = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            ulong currentClientId = client.Key;
            PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(currentClientId);
            
            GameObject counterObject = Instantiate(playerCountPrefab, GameObject.Find("Canvas").transform);
            // Adjust the height based on how many players have already been added
            float counterHeight = counterObject.GetComponent<RectTransform>().rect.height;
            Vector3 positionAdjustment = new Vector3(0, counterHeight * playerIndex, 0);
            counterObject.transform.position += positionAdjustment;

            playerIndex++;

            // Set the player's color
            Image counterImage = counterObject.GetComponentInChildren<Image>();
            Sprite playerSprite = playerSprites[playerData.colorId];
            counterImage.sprite = playerSprite;

            Counter counter = new Counter();
            counter.clientId = currentClientId;
            counter.text = counterObject.GetComponentInChildren<TextMeshProUGUI>();
            playerCounters.Add(counter);
        }
    }

    private void UpdatePlayerCounters()
    {
        foreach (Counter counter in playerCounters)
        {
            PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(counter.clientId);
            counter.text.text = playerData.currentCount.ToString();
        }
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
        
        RibbitRoyaleMultiplayer.Instance.SetCTAPlayerData(animalIndex, 0, 0);
    }

    void ChooseAnimalToCount(int animalIndex)
    {
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

        finalCount = GetAnimal(countedAnimal).count;
        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        
        RibbitRoyaleMultiplayer.Instance.SetCTAPlayerData(playerData.countedAnimalIndex, playerData.currentCount, finalCount);
    }

    // Ends the game
    private void EndGame()
    {
        // Stop the game
        gameActive = false;

        if (!isHost)
        {
            ulong ownerId = NetworkManager.Singleton.CurrentSessionOwner;
            PlayerData hostData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(ownerId);
            finalCount = hostData.finalCount;

            Debug.Log("CountedAnimalIndex: " + hostData.countedAnimalIndex);
            Debug.Log("Final Count: " + finalCount);
        }

        
        finalCountedAnimalsText.gameObject.SetActive(true);
        finalCountedAnimalsText.text = finalCount.ToString();

        // foreach (Animal animal in animals) {
        //     Debug.Log(animal.name + " count: " + animal.count);
        // }

        Invoke("ReturnToLobby", 3);
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