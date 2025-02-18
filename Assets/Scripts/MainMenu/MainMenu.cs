using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // When the user clicks the Play button the user will be taken to the network lobby scene
    public void PlayGame(){
        Loader.Load(Loader.Scene.NetworkLobbyScene);
        
    }

    // When the user cliks the Quit button the users game window will be closed
    public void DoExitGame(){
        Application.Quit();
    }

}
