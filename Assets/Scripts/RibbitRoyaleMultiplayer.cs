using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RibbitRoyaleMultiplayer: NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;
    public static RibbitRoyaleMultiplayer Instance { get; private set;}

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnTFailedToJoinGame;

    private void Awake(){
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartHost(){
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
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

}
