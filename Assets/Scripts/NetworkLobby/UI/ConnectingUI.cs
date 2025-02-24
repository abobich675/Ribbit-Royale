using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start(){
        RibbitRoyaleMultiplayer.Instance.OnTryingToJoinGame += RibbitRoyaleMultiplayer_OnTryingToJoinGame;
        RibbitRoyaleMultiplayer.Instance.OnTFailedToJoinGame += PreLobbyManager_OnFailedToJoinGame;
        Hide();

    }

    private void PreLobbyManager_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void RibbitRoyaleMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Show();
        
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void OnDestroy(){
        RibbitRoyaleMultiplayer.Instance.OnTryingToJoinGame -= RibbitRoyaleMultiplayer_OnTryingToJoinGame;
        RibbitRoyaleMultiplayer.Instance.OnTFailedToJoinGame -= PreLobbyManager_OnFailedToJoinGame;
    }

}
