using System;
using UnityEngine;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake() {
        if (RibbitRoyaleMultiplayer.Instance == null) {
            Debug.LogError("RibbitRoyaleMultiplayer.Instance is null! Character script is executing too early.");
            return;
        }

        Debug.Log("Character Awake");
        var button = GetComponent<Button>();
        if (button == null) {
            Debug.LogError("Button component is missing on " + gameObject.name);
            return;
        }

        button.onClick.AddListener(() => {
            RibbitRoyaleMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }
private void Start() {
    // Check if RibbitRoyaleMultiplayer.Instance is not null
    if (RibbitRoyaleMultiplayer.Instance == null) {
        Debug.LogError("RibbitRoyaleMultiplayer.Instance is null! The Character script is running too early.");
        return;
    }
    // Log the current GameObject this script is attached to
    Debug.Log("Character script attached to GameObject: " + gameObject.name);
    // Check if the image field is assigned in the Inspector
    if (image == null) {
        Debug.LogError("Image component is missing! Please assign an Image to the Character script on GameObject: " + gameObject.name);
        return;
    } else {
        Debug.Log("Image component found on GameObject: " + gameObject.name);
    }
    // Check if the selectedGameObject is assigned in the Inspector
    if (selectedGameObject == null) {
        Debug.LogError("Selected GameObject is missing! Please assign the selected GameObject on GameObject: " + gameObject.name);
        return;
    } else {
        Debug.Log("Selected GameObject found: " + selectedGameObject.name);
    }
    // Subscribe to the event
    RibbitRoyaleMultiplayer.Instance.onPlayerDataNetworkListChanged += RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged;
    // Set the player's color
    image.color = RibbitRoyaleMultiplayer.Instance.GetPlayerColor(colorId);
    // Update the selection state
    UpdateIsSelected();
}


    private void RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected(){
        if (RibbitRoyaleMultiplayer.Instance.GetPlayerData().colorId == colorId){
            selectedGameObject.SetActive(true);
        } else{
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy(){
        RibbitRoyaleMultiplayer.Instance.onPlayerDataNetworkListChanged -= RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged;

    }
}
