using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake(){
        createGameButton.onClick.AddListener(() =>{
            RibbitRoyaleMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);

        });
        joinGameButton.onClick.AddListener(() =>{
            RibbitRoyaleMultiplayer.Instance.StartClient();

        });
    }



}
