using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CTAPlayerConroller : MonoBehaviour
{
    public CTAMain mainScript;
    public TextMeshProUGUI counterText;


    InputAction attackAction;
    InputAction jumpAction;


    // Player's counter
    public int counter = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        attackAction = input.actions["Attack"];
        jumpAction = input.actions["Jump"];

        counterText.text = counter.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainScript.GameActive())
        {
            return;
        }
        
        if (attackAction.triggered || jumpAction.triggered)
        {
            counter++;

            PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
            RibbitRoyaleMultiplayer.Instance.SetCTAPlayerData(playerData.countedAnimalIndex, counter, playerData.finalCount);
            counterText.text = counter.ToString();
        }
    }
}
