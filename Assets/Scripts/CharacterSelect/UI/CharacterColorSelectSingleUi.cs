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
        GetComponent<Button>().onClick.AddListener(() => {
            RibbitRoyaleMultiplayer.Instance.ChangePlayerColor(colorId);
            UpdateIsSelected();
        });
    }
private void Start() {
    // Subscribe to the event
    RibbitRoyaleMultiplayer.Instance.OnPlayerDataNetworkListChanged += RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged;
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
        Debug.Log($"Checking selection for colorId {colorId}, Current Player Color ID: {RibbitRoyaleMultiplayer.Instance.GetPlayerData().colorId}");
        if (RibbitRoyaleMultiplayer.Instance.GetPlayerData().colorId == colorId){
            selectedGameObject.SetActive(true);
        } else{
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy(){
        RibbitRoyaleMultiplayer.Instance.OnPlayerDataNetworkListChanged -= RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged;

    }
}
