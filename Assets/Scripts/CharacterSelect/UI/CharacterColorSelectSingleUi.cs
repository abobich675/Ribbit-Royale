using System;
using UnityEngine;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    // This will control the color of the button based of the color ids in the RibbitRoyaleMultiplayer color list field
    [SerializeField] private int colorId;
    // The image (color) of the button chnages based off the color off the colorId
    [SerializeField] private Image image;
    // This will display the button with a checkmark if the button is selected
    [SerializeField] private GameObject selectedGameObject;

    // Awake function for the different color options (buttons)
    private void Awake() {
        // This will get the button component of the prefab CharacterColorSelectUI
        var button = GetComponent<Button>();
        // When the button is cliecked
        button.onClick.AddListener(() => {
            // Call the instance for the ChangePlayerColor function passing in the colorId of the button that has been pressed to set the character to the color of the button
            RibbitRoyaleMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }
private void Start() {
    // Subscribe to the event
    RibbitRoyaleMultiplayer.Instance.onPlayerDataNetworkListChanged += RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged;
    // Set the player's color
    image.color = RibbitRoyaleMultiplayer.Instance.GetPlayerColor(colorId);
    // Update the selection state
    UpdateIsSelected();
}


    private void RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e){
        UpdateIsSelected();
    }

    // Function is used to update the imgae of the button based on when it is pressed (selected) 
    private void UpdateIsSelected(){
        // Create an instance of the RibbitRoyaleMultiplayer and call function to get the colorId
        //If the colorId matches the colorId of the button then set image to the selectedGameObject
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
