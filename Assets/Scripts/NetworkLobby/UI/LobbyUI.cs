using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;

    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake(){
        mainMenuButton.onClick.AddListener(() => {
            RibbitRoyaleLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(() => {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() => {
            RibbitRoyaleLobby.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() => {
            RibbitRoyaleLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });
        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        RibbitRoyaleLobby.Instance.OnLobbyListChanged += RibbitRoyaleLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());   
    }

    private void RibbitRoyaleLobby_OnLobbyListChanged(object sender, RibbitRoyaleLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList){
        foreach (Transform child in lobbyContainer){
            if (child == lobbyTemplate)continue;
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbyList){
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSIngleUI>().SetLobby(lobby);
        }
 
    }

    private void OnDestroy()
    {
        RibbitRoyaleLobby.Instance.OnLobbyListChanged -= RibbitRoyaleLobby_OnLobbyListChanged;

    }
}
