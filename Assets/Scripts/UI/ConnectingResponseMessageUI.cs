using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingResponseMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake (){
        closeButton.onClick.AddListener(Hide);
    }
    private void Start(){
        RibbitRoyaleMultiplayer.Instance.OnTFailedToJoinGame += RibbitRoyaleMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void RibbitRoyaleMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if (messageText.text == ""){
            messageText.text = "Failed to connect";
        }
    }

    private void Show(){
        gameObject.SetActive(true);
    }
    private void Hide(){
        gameObject.SetActive(false);
    }

    private void OnDestroy(){
        RibbitRoyaleMultiplayer.Instance.OnTFailedToJoinGame -= RibbitRoyaleMultiplayer_OnFailedToJoinGame;
    }
}
