using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CountTheAnimalTests
{
    float MAX_LOAD_TIME = 5f;
    
    [UnityTest]
    public IEnumerator TestCountTheAnimalSceneLoadTime()
    {
        float startTime = Time.realtimeSinceStartup;
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CountTheAnimalGame");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        float loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"CountTheAnimal Scene Load Time: {loadTime} seconds");

        // Set an assertion for reasonable load time
        Assert.Less(loadTime, MAX_LOAD_TIME, "Scene took too long to load!");
    }
}
