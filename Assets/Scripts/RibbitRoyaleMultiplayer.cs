using System;
using Unity.Netcode;
using UnityEngine;

public class RibbitRoyaleMultiplayer: NetworkBehaviour
{
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
        connectionApprovalResponse.Approved = true;
        connectionApprovalResponse.CreatePlayerObject = true;
    }

    public void StartClient(){
        NetworkManager.Singleton.StartClient();

    }

}
