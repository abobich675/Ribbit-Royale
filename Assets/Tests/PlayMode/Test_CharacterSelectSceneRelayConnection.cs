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
    float MAX_LOAD_TIME = 5f;

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

        
    }
}
#endif
