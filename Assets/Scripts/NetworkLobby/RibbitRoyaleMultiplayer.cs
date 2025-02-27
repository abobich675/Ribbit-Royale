using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RibbitRoyaleMultiplayer : NetworkBehaviour
{
    // Global variable which sets the max player amount to 4
    public const int MAX_PLAYER_AMOUNT = 4;
    // Create an singleton instance 
    public static RibbitRoyaleMultiplayer Instance { get; private set; }
    // Handle the event in which a user is trying to join a game
    public event EventHandler OnTryingToJoinGame;
    // Handle the event in which a user failed to join the game
    public event EventHandler OnTFailedToJoinGame;
    // Handle the event when the player data network list is changed
    public event EventHandler OnPlayerDataNetworkListChanged;
    // Create a network list for the player data
    private NetworkList<PlayerData> playerDataNetworkList;
    // List of colors the user can choose from
    [SerializeField] private List<Color> playerColorList;

    private void Awake(){
        // Setup the singleton pattern so that Instance can be accessed globally within the class 
        Instance = this;
        // Game object that the script is attached to will persist between the scenes
        DontDestroyOnLoad(gameObject);
        // Initialize a new NetworkList to store player data (clientId, colorId, score)
        playerDataNetworkList = new NetworkList<PlayerData>();
        // Subscribing to the event, triggers whenever the list changes 
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    // This function is used to invoke this event whenever the playerDataNetworkList changes.
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent){
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost(){
        // Set up a callback to handle the connection when approved
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        // Set up a callback to handle the connection when a client joins
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnnectedCallback;
        // Set up a callback to handle when a client disconnects
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        // Create a host instance using the network manager
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId){
        // Loop through the player data network list to find the player that is disconnecting
        for (int i = 0; i < playerDataNetworkList.Count; i++){
            // Set playerData to the playData that is disconnecting
            PlayerData playerData = playerDataNetworkList[i];
            // If the clientId is a match
            if(playerData.clientId == clientId){
                // Disconnected 
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnnectedCallback(ulong clientId){
        // Add new player to the network list
        playerDataNetworkList.Add(new PlayerData
        {
            // Set the clientId
            clientId = clientId,
            // Set the color 
            colorId = GetFirstUnusedColorId()

            //LEADERBOARD INTEGRATION
            // Set the players score 
        });
    }

    // Invokes the OnTFailedToJoinGame event when a client disconnects from the network
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId){
        OnTFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse){
        // The server is live
        if (IsServer)
        {
            // Set approval respone to true
            connectionApprovalResponse.Approved = true;
        }
        // The server that is trying to be connected to needs to be on the character select scene
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            // Set the approval response to false 
            connectionApprovalResponse.Approved = false;
            // Tell the user that the game has been started
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        // The server has reached full capacity
        if (NetworkManager.Singleton.ConnectedClients.Count >= MAX_PLAYER_AMOUNT)
        {
            // Set the approval response to false 
            connectionApprovalResponse.Approved = false;
            // Tell the user the game is full
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    // Initiates the client connection, triggers the join game event, and sets up the disconnect callback
    public void StartClient(){
        // Trigger the event to join game
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        // Register the disconnect callback
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    // Checks if a player exists at the given index in the player data list, indicating if they are connected
    public bool IsPlayerIndexConnected(int playerIndex){
        return playerIndex < playerDataNetworkList.Count;
    }

    // Retrieves the PlayerData for a player at the specified index in the player data list
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex){
        return playerDataNetworkList[playerIndex];
    }

    // Retrieves the color for a player based on the provided colorId
    public Color GetPlayerColor(int colorId){
        return playerColorList[colorId];
    }

    // Searches for the player data index based on the provided clientId and returns the index, or -1 if not found
    public int GetPlayerDataIndexFromClientId(ulong clientId){
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId) 
            {
                return i;
            }
        }
        return -1;
    }

    // Searches for and returns the PlayerData corresponding to the given clientId, or the default value if not found
    public PlayerData GetPlayerDataFromClientId(ulong clientId){
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    // Retrieves the PlayerData for the local client using the LocalClientId from the NetworkManager
    public PlayerData GetPlayerData(){
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    // Initiates a server RPC to change the player's color using the provided colorId
    public void ChangePlayerColor(int colorId){
        ChangePlayerColorServerRpc(colorId);
    }

    // Server-side RPC that changes the player's color if available and updates the player data
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default){
        if (!IsColorAvailable(colorId))
        {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId); 
        if (playerDataIndex == -1) return;

        PlayerData playerData = playerDataNetworkList[playerDataIndex]; 
        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    // Checks if the requested colorId is available by verifying its not already assigned to another player
    private bool IsColorAvailable(int colorId){
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                return false; // Already in use
            }
        }
        return true;
    }

    // Finds and returns the first unused colorId from the playerColorList, or -1 if no colors are available
    private int GetFirstUnusedColorId(){
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }
    
    public void SetPlayerFinished(bool finished){
        SetPlayerFinishedServerRpc(finished);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerFinishedServerRpc(bool finished, ServerRpcParams serverRpcParams = default){
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId); 
        if (playerDataIndex == -1) return;

        PlayerData playerData = playerDataNetworkList[playerDataIndex]; 
        playerData.finished = finished;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public int GetPlayerCount(){
        return playerDataNetworkList.Count;
    }
}
