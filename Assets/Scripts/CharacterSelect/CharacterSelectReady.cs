using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    private Dictionary<ulong, bool> playerReadyDictionary;

    [SerializeField] private GameObject playerPrefab; // Reference to your player prefab

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        playerReadyDictionary[clientId] = true;

        bool allClientsReady = true;
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            ulong currentClientId = client.Key;
            if (!playerReadyDictionary.ContainsKey(currentClientId) || !playerReadyDictionary[currentClientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            RibbitRoyaleLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.PreLobbyScene);
        }
    }
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId){
        playerReadyDictionary [clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerReady(ulong clientId){
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }

   
}
