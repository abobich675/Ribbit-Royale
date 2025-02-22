using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    // Create fields for assigning UI buttons 
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    
    private void Awake(){
        // When the main menu button is clicked
        mainMenuButton.onClick.AddListener(() =>{
            // Call the leave lobby function which removes the user from the lobby 
            RibbitRoyaleLobby.Instance.LeaveLobby();
            // Shutdown the network connection
            NetworkManager.Singleton.Shutdown();
            // Load the main menu scene
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        // When the ready button is clicked
        readyButton.onClick.AddListener(() =>{
            // Call the set player ready functiion
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start(){
        // Get lobby information when the character select scene starts 
       Lobby lobby = RibbitRoyaleLobby.Instance.GetLobby();
        // Set the lobby name text to the lobby.name
       lobbyNameText.text = "Lobby Name: " + lobby.Name;
       // Set the lobby code text to the lobby.code 
       lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
