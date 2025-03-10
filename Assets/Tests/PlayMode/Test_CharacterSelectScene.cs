using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

#if UNITY_EDITOR
public class CharacterSelectSceneTests
{
    [UnityTest]
    public IEnumerator TestCharacterSelectSceneLoadsCorrectly()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CharacterSelectScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); 

        Assert.AreEqual("CharacterSelectScene", SceneManager.GetActiveScene().name, "Scene did not load correctly!");
    }
}
#endif
