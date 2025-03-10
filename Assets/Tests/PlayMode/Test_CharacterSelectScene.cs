using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Correct namespace for TextMeshPro
using UnityEngine.TestTools;
using System.Collections;

public class TestLobbyCreateUI : MonoBehaviour
{
    private LobbyCreateUI lobbyCreateUI;
    private GameObject lobbyUIObject;
    
    [SetUp]
    public void SetUp()
    {
        // Create a GameObject to hold the UI
        lobbyUIObject = new GameObject("LobbyCreateUI");

        // Attach the LobbyCreateUI component
        lobbyCreateUI = lobbyUIObject.AddComponent<LobbyCreateUI>();

        // Create UI elements
        lobbyCreateUI.GetType().GetField("createPublicButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(lobbyCreateUI, CreateUIButton("PublicButton"));
        
        lobbyCreateUI.GetType().GetField("lobbyNameInputField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(lobbyCreateUI, CreateInputField("TestLobby"));
    }

    private Button CreateUIButton(string name)
    {
        GameObject buttonObj = new GameObject(name);
        Button button = buttonObj.AddComponent<Button>();
        return button;
    }

    private TMP_InputField CreateInputField(string text)
    {
        GameObject inputObj = new GameObject("InputField");
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.text = text;
        return inputField;
    }



    [TearDown]
    public void TearDown()
    {
        Object.Destroy(lobbyUIObject);
    }
}
