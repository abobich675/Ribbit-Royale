using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RibbitRoyaleMultiplayer: NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;
    public static RibbitRoyaleMultiplayer Instance { get; private set;}

    private void Awake(){
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartHost(){
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
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

    public void StartClient(){
        NetworkManager.Singleton.StartClient();

    }

}
