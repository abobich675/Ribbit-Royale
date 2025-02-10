using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    private void Awake(){
        mainMenuButton.onClick.AddListener(() =>{
            NetworkManager.Singleton.Shutdown();
            Loader.load(Loader.Scene.MainMenuScene);

        });
        readyButton.onClick.AddListener(() =>{
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
}
