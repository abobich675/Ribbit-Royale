using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField]private Button startHostButton; 
    [SerializeField]private Button startClientButton;

    private void Awake(){
        startHostButton.onClick.AddListener(() =>{
            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() =>{
            Debug.Log("ClIENT");
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

}
