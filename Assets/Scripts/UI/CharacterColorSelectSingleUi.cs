using System;
using UnityEngine;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake(){
        GetComponent<Button>().onClick.AddListener(() =>{
            RibbitRoyaleMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }


    private void Start(){
        RibbitRoyaleMultiplayer.Instance.onPlayerDataNetworkListChanged += RibbitRoyaleMultiplayer_OnPlayerDataNetworkListChanged;
        image.color = RibbitRoyaleMultiplayer.Instance.GetPlayerColor(colorId);
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
}
