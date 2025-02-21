using NUnit.Framework;
using UnityEngine;

public class ExampleEditModeTest
{
    [Test]
    public void GameObject_HasTransform()
    {
        var obj = new GameObject();
        Assert.IsNotNull(obj.transform); // Check if GameObject has a Transform component
    }
}
