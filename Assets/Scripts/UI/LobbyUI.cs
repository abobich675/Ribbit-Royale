using TMPro;
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

    private void Awake(){
        mainMenuButton.onClick.AddListener(() => {
            RibbitRoyaleLobby.Instance.LeaveLobby();
            Loader.load(Loader.Scene.MainMenuScene);
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
        
    }
}
