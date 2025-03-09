using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCtrl : NetworkBehaviour
{
    [SerializeField] private PlayerVisual playerVisual; // Reference to PlayerVisual    
    public RuntimeAnimatorController[] animators; // Green Blue Red Yellow
    public float movSpeed;
    private float speedx, speedy;
    Rigidbody2D rb;
    Animator animator;

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
        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        playerVisual.SetPlayerColor(RibbitRoyaleMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        animator = GetComponent<Animator>();

        SetColor();
    }

    void SetColor() {
        PlayerData playerData;
        try {
            // This will get a players data based off the clientId
            ulong clientId = GetComponent<NetworkObject>().OwnerClientId;
            playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
        } catch {
            Debug.Log("Failed to get player data from clientId");
            return;
        }
        
        if (playerData.colorId < 0 || playerData.colorId >= animators.Length) {
            Debug.LogError("Player color ID out of bounds");
            return;
        }

        animator.runtimeAnimatorController = animators[playerData.colorId];
    }

    void Update()
    {
        // Update animation / sprite direction
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        if (rb.linearVelocityX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (rb.linearVelocityX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (!IsOwner || !canMove)
        {
            return;
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = moveInput * movSpeed;
    }
}
