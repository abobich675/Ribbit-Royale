using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene {
        MainMenuScene,
        PreLobbyScene,
        NetworkLobbyScene,
        CharacterSelectScene
    }
    public static int targetSceneIndex;

    public static void load(Scene targetScene){
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(),LoadSceneMode.Single);
    }
}
