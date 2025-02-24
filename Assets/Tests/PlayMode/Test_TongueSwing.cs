using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TongueSwingTests
{
    float MAX_LOAD_TIME = 5f;

    [UnityTest]
    public IEnumerator TestTongueSwingSceneLoadTime()
    {
        float startTime = Time.realtimeSinceStartup;
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TongueSwingGame");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        float loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"TongueSwing Scene Load Time: {loadTime} seconds");

        // Set an assertion for reasonable load time
        Assert.Less(loadTime, MAX_LOAD_TIME, "Scene took too long to load!");
    }
}
