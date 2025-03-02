using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCtrl : NetworkBehaviour
{
    [SerializeField] private PlayerVisual playerVisual; // Reference to PlayerVisual    
    public float movSpeed;
    private float speedx, speedy;
    private Rigidbody2D rb;

    InputAction moveAction;

    private bool canMove = true;

    void Start()
    {
        // Disable movement if in Character Select scene
        if (SceneManager.GetActiveScene().name == "CharacterSelectScene"){
            canMove = false;
        }
        PlayerInput input = GetComponent<PlayerInput>();
        moveAction = input.actions["Move"];

        rb = GetComponent<Rigidbody2D>();
        
        // This will get a players data based off the clientId 
        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        // Using the player data call the function SetPlayerColor, get the players color using the playerData
        playerVisual.SetPlayerColor(RibbitRoyaleMultiplayer.Instance.GetPlayerColor(playerData.colorId));


    }

    void Update()
    {
        if (!IsOwner || !canMove)
        {
            return;
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * movSpeed;
    }
}
