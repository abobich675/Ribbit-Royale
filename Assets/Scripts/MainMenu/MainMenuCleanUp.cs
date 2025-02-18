using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    // This function will execute when the user is located on the main menu scene
    private void Awake(){
        // If the network manager is still running then destroy the gameObject
        if (NetworkManager.Singleton != null){
            Destroy(NetworkManager.Singleton.gameObject);
        }
        // If the ribbit royale multiplayer instance is still activated then destroy the gameObject
        if (RibbitRoyaleMultiplayer.Instance != null){
            Destroy(RibbitRoyaleMultiplayer.Instance.gameObject);
        }
        // If the ribbit royale lobby instance is still activated then destroy the gameObject
        if (RibbitRoyaleLobby.Instance != null){
            Destroy(RibbitRoyaleLobby.Instance.gameObject);
        }
    }
}
