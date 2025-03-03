using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLobbyManager : NetworkBehaviour
{
    public static PreLobbyManager Instance { get; private set; }

    // This is a list of the different states the game can be in
    private enum State{
        CountDownToStart,
        GamePlaying,
        GameOver
    }
    // A field to spawn in ta playerPrefab
    [SerializeField] private Transform PrelobbyPrefab;
    [SerializeField] private Transform TongueSwingPrefab;
    // The state of the game is synced over a network
    private NetworkVariable<State> state = new NetworkVariable<State>(State.GamePlaying);
    // Implement an option when the user pauses the game
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    
    private float waitingToTransition = 30f;

    private void Awake(){
        Instance = this;
    }

    private void Update(){
        if (!IsServer) return; // Only the server should update the state

        switch (state.Value)
        {
            case State.GamePlaying:
                waitingToTransition -= Time.deltaTime;
                if (waitingToTransition < 0f)
                {
                    state.Value = State.GameOver;
                }
                break;
        }
    }

    public bool IsGamePlaying(){
        return state.Value == State.GamePlaying;
    }

    public override void OnNetworkSpawn(){
        if (IsServer)
        {
            // Add this to remove the event first
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            // Add this to remove the event for the client connection
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    // TODO: This function is being called an additional time for each time the prelobby scene is loaded
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut){
        Debug.Log("Connected Clients: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("OnLoad: " + clientId);
            Debug.Log("OnLoad: " + RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(clientId));

            switch (sceneName)
            {
                case "PreLobbyScene":
                    SpawnPlayer(PrelobbyPrefab, clientId);
                    break;
                case "TongueSwingGame":
                    SpawnPlayer(TongueSwingPrefab, clientId);
                    break;
            }


        }
    }

    private void OnClientConnected(ulong clientId){
        // Spawn a player for the newly connected client
        SpawnPlayer(PrelobbyPrefab, clientId);
    }

    private void SpawnPlayer(Transform prefab, ulong clientId){
        Transform playerTransform = Instantiate(prefab);
        playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
