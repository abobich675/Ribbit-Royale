using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Unity.Netcode;
using Unity.Services.Relay;

#if (UNITY_EDITOR)
public class CharacterSelectSceneTests
{
    float MAX_LOAD_TIME = 10f;

    [UnityTest]
    public IEnumerator TestCharacterSelectSceneLoadAndRelayConnection()
    {
        float startTime = Time.realtimeSinceStartup;

        // Load the Character Select Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CharacterSelectScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Allow time for initialization

        float loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"Character Select Scene Load Time: {loadTime} seconds");

        // Assert that scene loaded within reasonable time
        Assert.Less(loadTime, MAX_LOAD_TIME, "Scene took too long to load!");

        // Check if NetworkManager is active and running
        Assert.IsNotNull(NetworkManager.Singleton, "NetworkManager is missing!");
        Assert.IsTrue(NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost, 
            "Player is not connected to the relay!");

        // Verify Relay Connection (if using Unity Relay)
        Assert.IsTrue(Unity.Services.Relay.RelayService.Instance != null, "Relay Service is not initialized!");

        Debug.Log("Player is connected to the relay and scene loaded successfully.");
    }
}
#endif
