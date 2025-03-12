using Unity.Netcode;
using System;
using System.Collections;
using System.Net;
using UI.Scoreboard;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class FrogController : NetworkBehaviour
{
    public float moveSpeed = 5f;     // Speed for horizontal movement
    public float jumpForce = 7f;    // Force applied when jumping
    private bool isGrounded = true; // Tracks if the player is on the ground
    private Rigidbody2D rb;         // Reference to the Rigidbody2D component
    private bool timerOver = false;
    private ScoreController scoreController;

    InputAction moveAction;
    float moveHorizontal;

    Animator animator;
    public RuntimeAnimatorController[] animators; // Green Blue Red Yellow
    [SerializeField] private PlayerVisual playerVisual; // Reference to PlayerVisual    

    private bool isHost;

    //GameObject connectedObject;

    void Start()
    {
        // Initialize the Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 0f;

        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
        {
            moveAction = input.actions["Move"];
            moveAction.Enable(); // Ensure the action is enabled
        }
        else
        {
            Debug.LogError("PlayerInput component missing on: " + gameObject.name);
        }


        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        playerVisual.SetPlayerColor(RibbitRoyaleMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        animator = GetComponent<Animator>();

        SetColor();

        if (!IsOwner)
        {
            return;
        }

        if (GameObject.FindGameObjectWithTag("ScoreControllerGO"))
        {
            scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
            scoreController.InitializeSC();
            var infoPanelDuration = scoreController.GetPopupTimer(0);
            StartCoroutine(WaitForPopupDelay(infoPanelDuration));
        }
        moveSpeed = 5f;
    }

    void Update()
    {
        if (!timerOver || !IsOwner) return;

        // Read movement input from PlayerInput component
        Vector2 moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero; // Safely read input

        if (moveInput != Vector2.zero)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        // Update animation / sprite direction
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Handle jumping
        if (moveInput.y > 0.5f && isGrounded) // Using 'y' axis instead of KeyCode.W
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false; // Prevent multiple jumps mid-air
        }
    }

    void SetColor()
    {
        PlayerData playerData;
        try
        {
            // This will get a players data based off the clientId
            ulong clientId = GetComponent<NetworkObject>().OwnerClientId;
            playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
        }
        catch
        {
            Debug.Log("Failed to get player data from clientId");
            return;
        }

        if (playerData.colorId < 0 || playerData.colorId >= animators.Length)
        {
            Debug.LogError("Player color ID out of bounds");
            return;
        }

        animator.runtimeAnimatorController = animators[playerData.colorId];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player lands on a surface (ground detection)
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }

        Debug.Log(collision.gameObject.name);

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // When player hits the snake
        if (collider.gameObject.CompareTag("Snake"))
        {
            HandlePlayerCaught();
        }

        if (collider.gameObject.CompareTag("Finish"))
        {
            Finish();
        }
    }

    private void HandlePlayerCaught()
    {
        Debug.Log($"Player {OwnerClientId} hit by snake, setting score to DNF...");

        // Ensure ScoreManager is available
        if (ScoreManager.Instance != null)
        {
            foreach (var entry in ScoreManager.Instance.GetEntryList())
            {
                if (entry.GetPlayerName() == OwnerClientId)
                {
                    entry.SetScore(-1, "DNF"); // Mark player as DNF immediately
                    Debug.Log($"Player {OwnerClientId} marked as DNF on scoreboard.");
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("ScoreManager instance not found!");
        }

        // Prevent further movement and interactions
        moveSpeed = 0f;
        GetComponent<Collider2D>().enabled = false;

        // Remove player from the network
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }


        // Check if all players have finished
        if (ScoreManager.Instance != null)
        {
            var allFinished = true;
            foreach (var entry in ScoreManager.Instance.GetEntryList())
            {
                if (entry.GetScore() != -1) // If any player is still active, don't finish
                {
                    allFinished = false;
                    break;
                }
            }

            if (allFinished)
            {
                Debug.Log("All players have finished or are DNF. Ending game...");
                Finish(); // Call Finish() to end the game early
            }
        }
    }

    private void Finish()
    {
        Debug.Log("Player reached the finish");
        try
        {
            RibbitRoyaleMultiplayer.Instance.SetPlayerFinished(true);
            //GetComponent<Collider2D>().enabled = false;
            Debug.Log("RRM SetPlayerFinished Successfully...");
            //scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
            //scoreController.SetPlayerFinished();
            //RibbitRoyaleMultiplayer.Instance.SetPlayerFinished(true);
            Debug.Log("ScoreController SetPlayerFinished Successfully...");

            // Check if all players have finished
            bool allFinished = true;
            Debug.Log("Checking if players have finished:");
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                ulong currentClientId = client.Key;
                // Debug.Log(RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(currentClientId).finished);
                if (!RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(currentClientId).finished)
                {
                    allFinished = false;
                    break;
                }
            }
            if (allFinished)
            {
                //Loader.LoadNetwork(Loader.Scene.PreLobbyScene);
                //scoreController.CalculatePlayerScores();
                return;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Failed to get player data from clientId. Sending player back to prelobby: ERROR \" + e");
            Loader.Load(Loader.Scene.PreLobbyScene);
            return;
        }
    }

    private IEnumerator WaitForPopupDelay(float popupDelay)
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(popupDelay);
        timerOver = true;
        yield return null;
    }
}
