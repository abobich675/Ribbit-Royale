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
            RibbitRoyaleMultiplayer.Instance.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() =>{
            Debug.Log("ClIENT");
            RibbitRoyaleMultiplayer.Instance.StartClient();
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

}
