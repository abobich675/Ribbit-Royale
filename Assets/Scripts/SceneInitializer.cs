using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : NetworkBehaviour
{
    public static SceneInitializer Instance { get; private set; }
    // A field to spawn in ta playerPrefab
    [SerializeField] private Transform PrelobbyPrefab;
    [SerializeField] private Transform TongueSwingPrefab;
    [SerializeField] private Transform SnakeChasePrefab;

    private void Awake(){
        Instance = this;
    }

    public override void OnNetworkSpawn(){
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }
    
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut){
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            switch (sceneName)
            {
                case "PreLobbyScene":
                    SpawnPlayer(PrelobbyPrefab, clientId, 1f);
                    break;
                case "TongueSwingGame":
                    SpawnPlayer(TongueSwingPrefab, clientId, 10f);
                    break;
                case "SnakeChaseGame":
                    SpawnPlayer(SnakeChasePrefab, clientId, 1f);
                    break;
            }
        }
    }

    private void OnClientConnected(ulong clientId){
        // Spawn a player for the newly connected client
        SpawnPlayer(PrelobbyPrefab, clientId);
    }

    private void SpawnPlayer(Transform prefab, ulong clientId, float randX = 0){
        Transform playerTransform = Instantiate(prefab);
        playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        // Randomize X pos by a little bit
        playerTransform.position += Vector3.right * UnityEngine.Random.Range(-randX, randX);
    }

    private void OnDestroy(){
        try
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        catch (Exception e)
        {
            Debug.Log("Could not destroy: " + e);
        }
    }
}