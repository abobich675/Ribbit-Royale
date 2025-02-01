using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake(){
        if (NetworkManager.Singleton != null){
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (RibbitRoyaleMultiplayer.Instance != null){
            Destroy(RibbitRoyaleMultiplayer.Instance.gameObject);
        }
    }




}
