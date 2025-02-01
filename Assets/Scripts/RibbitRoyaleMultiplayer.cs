using System;
using System.Diagnostics.Tracing;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RibbitRoyaleMultiplayer: NetworkBehaviour
{

    private const int MAX_PLAYER_AMOUNT = 4;
    public static RibbitRoyaleMultiplayer Instance { get; private set;}

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnTFailedToJoinGame;
    public event EventHandler onPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake(){
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += playerDataNetworkList_OnListChanged;
    }

    private void playerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        onPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost(){
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnnectedCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientConnnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData{
            clientId = clientId,
        });
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        OnTFailedToJoinGame?.Invoke(this,EventArgs.Empty);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if(IsServer){
            connectionApprovalResponse.Approved = true;
        }
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()){
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClients.Count >=  MAX_PLAYER_AMOUNT){
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;

    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this,EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();

    }


    public bool IsPlayerIndexConnected(int playerIndex){
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex){
        return playerDataNetworkList[playerIndex];
    }

}
