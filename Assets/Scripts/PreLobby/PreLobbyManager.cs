using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLobbyManager : NetworkBehaviour
{
    public static PreLobbyManager Instance { get; private set; }

    private enum State
    {
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.GamePlaying);
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private float waitingToTransition = 30f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
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
        Debug.Log(state.Value);
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Spawn a player for the newly connected client
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        Transform playerTransform = Instantiate(playerPrefab);
        playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
