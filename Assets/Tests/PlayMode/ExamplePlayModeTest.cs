using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class ExamplePlayModeTest
{
    [UnityTest]
    public IEnumerator GameObject_IsActive()
    {
        var obj = new GameObject();
        yield return null; // Waits for one frame
        Assert.IsTrue(obj.activeSelf); // Check if GameObject is active
    }
}
