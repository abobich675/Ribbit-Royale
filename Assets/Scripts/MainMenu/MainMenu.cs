using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Loader.load(Loader.Scene.NetworkLobbyScene);
        
    }
    public void DoExitGame(){
        Application.Quit();
    }

}
