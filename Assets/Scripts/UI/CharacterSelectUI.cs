using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake(){
        mainMenuButton.onClick.AddListener(() =>{
            NetworkManager.Singleton.Shutdown();
            Loader.load(Loader.Scene.MainMenuScene);

        });
        readyButton.onClick.AddListener(() =>{
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start()
    {
       Lobby lobby = RibbitRoyaleLobby.Instance.GetLobby();

       lobbyNameText.text = "Lobby Name: " + lobby.Name;
       lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
    }
}
