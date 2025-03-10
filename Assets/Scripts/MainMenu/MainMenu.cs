using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject OptionsMenu;
    [SerializeField] private GameObject Menu;
    public void PlayGame()
    {
        Loader.Load(Loader.Scene.NetworkLobbyScene);
    }

    public void OpenOptionsMenu()
    {
        OptionsMenu.SetActive(true);  // Show Options Menu and disable main menu
        Menu.SetActive(false);
    }

    public void CloseOptionsMenu()
    {
        OptionsMenu.SetActive(false); // Hide Options Menu and show main menu 
        Menu.SetActive(true);
    }

    public void DoExitGame()
    {
        Application.Quit();
    }
}
