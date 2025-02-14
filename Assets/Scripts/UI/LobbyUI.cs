using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;

    private void Awake(){
        mainMenuButton.onClick.AddListener(() => {
            Loader.load(Loader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(() => {
            RibbitRoyaleLobby.Instance.CreateLobby("LobbyName", false);
        });
        quickJoinButton.onClick.AddListener(() => {
            RibbitRoyaleLobby.Instance.QuickJoin();
        });
    }
}
