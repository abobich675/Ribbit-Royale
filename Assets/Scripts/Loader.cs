using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    // List the scenes that can be loaded (Needs to match the order in Unitys build profile)
    public enum Scene {
        MainMenuScene,
        PreLobbyScene,
        NetworkLobbyScene,
        CharacterSelectScene,
        TongueSwingGame,
        CountTheAnimalGame,
        SnakeChaseGame
    }
    // Varaible for the index of the target scene
    public static int targetSceneIndex;
    // This function calls the scene manager function called LoadScene and passes the targetScene argument and converts to a string
    public static void Load(Scene targetScene){
        SceneManager.LoadScene(targetScene.ToString());
    }
    // This function attaches the network mangaer singleton to the scene manager to load the scene over a netwrok
    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(),LoadSceneMode.Single);
    }
}
