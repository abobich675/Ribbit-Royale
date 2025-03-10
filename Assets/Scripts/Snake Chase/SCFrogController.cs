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
    //Animator animator;
    //public RuntimeAnimatorController[] animators; // Green Blue Red Yellow

    //InputAction moveAction;

    //private bool isHost;

    //GameObject connectedObject;

    void Start()
    {
        // Initialize the Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 0f;

        //PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerData();
        //playerVisual.SetPlayerColor(RibbitRoyaleMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        //animator = GetComponent<Animator>();

        SetColor();

        if (!IsOwner)
            return;

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
        if (timerOver)
        {
            // Handle horizontal movement
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

            // Handle jumping
            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false; // Prevent multiple jumps mid-air
            }
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

        //if (playerData.colorId < 0 || playerData.colorId >= animators.Length)
        //{
        //    Debug.LogError("Player color ID out of bounds");
        //    return;
        //}

        //animator.runtimeAnimatorController = animators[playerData.colorId];
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
        // When player reaches the finish
        if (collider.gameObject.CompareTag("Snake"))
        {
            Finish();
        }

        if (collider.gameObject.CompareTag("Finish"))
        {
            Finish();
        }
    }

    private void Finish()
    {
        Debug.Log("Player reached the finish");
        try
        {
            RibbitRoyaleMultiplayer.Instance.SetPlayerFinished(true);
            GetComponent<Collider2D>().enabled = false;
            Debug.Log("RRM SetPlayerFinished Successfully...");
            //scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
            //scoreController.SetPlayerFinished();
            RibbitRoyaleMultiplayer.Instance.SetPlayerFinished(true);
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
        catch
        {
            Debug.Log("Failed to get player data from clientId. Sending player back to prelobby");
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
