using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneLoadTest
{
    [UnityTest]
    public IEnumerator TestSnakeChaseSceneLoadTime()
    {
        float startTime = Time.realtimeSinceStartup;
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SnakeChaseScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        float loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"SnakeChaseScene Load Time: {loadTime} seconds");

        // Set an assertion for reasonable load time
        Assert.Less(loadTime, 5f, "Scene took too long to load!");
    }
}
