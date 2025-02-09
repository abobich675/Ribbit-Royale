using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Awake(){
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();

        });
    }

}
