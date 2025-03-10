using Unity.Netcode;
using System;
using System.Collections;
using System.Net;
using UI.Scoreboard;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    const float GRAVITYSCALE = 2f;

    float Acceleration = 30;
    float FastFallMultiplier = 1.25f;
    float BounceSpeedIncrease = 1;
    float BounceHeight = 15f;
    private ScoreController scoreController;
    private bool timerOver = false;

    public float maxSpeed;
    public float dampingFactor;

    public float swingingMovementBonus;
    public GameObject tongue;
    public RuntimeAnimatorController[] animators; // Green Blue Red Yellow

    GameObject connectedObject;
    bool isSwinging = false;
    Vector2 swingBoost = Vector2.zero;



    Rigidbody2D rb;
    Animator animator;

    InputAction moveAction;
    InputAction attackAction;

    private bool isHost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        tongue = Instantiate(tongue);
        tongue.SetActive(false);

        SetColor();
        
        if (!IsOwner)
            return;

        PlayerInput input = GetComponent<PlayerInput>();
        moveAction = input.actions["Move"];
        attackAction = input.actions["Attack"];
        
        // Will find the DoNotDestroy ScoreController object and initialize tongue swing score setup.

        try
        {
            scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
            scoreController.InitializeTS();
            var infoPanelDuration = scoreController.GetPopupTimer(0);
            StartCoroutine(WaitForPopupDelay(infoPanelDuration));
        }
        catch (Exception e)
        {
            Debug.Log("Could not set scoreController: ERROR " + e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner && timerOver)
        {
            DoInteraction();
        }
        UpdateAnimator();
    }

    // Fixed update for physics calculations
    void FixedUpdate()
    {
        if (IsOwner && timerOver)
        {
            DoForces();
        }
        
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

    void DoInteraction()
    {
        // Add tongue if click started
        if (attackAction.triggered)
        {
            // Locate object to connect to
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

            int bestHitIndex = -1;
            foreach (RaycastHit2D hit in hits)
            {
                // If this is the first hit
                if (bestHitIndex == -1) {
                    bestHitIndex = Array.IndexOf(hits, hit);
                }
                // If the hit object is closer to the player than the current best hit
                else if (Vector2.Distance(hit.transform.position, transform.position) < Vector2.Distance(hits[bestHitIndex].transform.position, transform.position))
                {
                    bestHitIndex = Array.IndexOf(hits, hit);
                }
            }
            if (bestHitIndex != -1)
            {
                connectedObject = hits[bestHitIndex].transform.gameObject;
                tongue.SetActive(true);

                GetComponent<SpringJoint2D>().enabled = true;
                GetComponent<SpringJoint2D>().connectedBody = connectedObject.GetComponent<Rigidbody2D>();

                isSwinging = true;
            }
        }

        // Remove tongue if click released
        if (attackAction.WasReleasedThisFrame())
        {
            tongue.SetActive(false);
            connectedObject = null;
            GetComponent<SpringJoint2D>().enabled = false;
            isSwinging = false;
        }

        // Update tongue position
        if (tongue.activeSelf) {
            Vector2 direction = connectedObject.transform.position - transform.position;
            float tongueWidth = direction.magnitude;
            direction.Normalize();

            tongue.transform.localScale = new Vector3(tongueWidth, 0.2f, 1);
            tongue.transform.position = transform.position + (Vector3)direction * tongueWidth / 2;
            tongue.transform.position = new Vector3(tongue.transform.position.x, tongue.transform.position.y, 2);
            tongue.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            // Update player rotation
            Vector2 playerToConnected = connectedObject.transform.position - transform.position;
            float angle = Mathf.Atan2(playerToConnected.y, playerToConnected.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, targetRotation, Time.deltaTime * 10);

            // Add swinging movement bonus
            // Increases speed of the player rotating around the node
            swingBoost = Vector2.zero;
            if (transform.position.y < connectedObject.transform.position.y) // under the object
            {
                if (rb.linearVelocityX > 0)
                {
                    swingBoost += Vector2.right;
                }
                else if (rb.linearVelocityX < 0)
                {
                    swingBoost += Vector2.left;
                }

                if (rb.linearVelocityY > 0)
                {
                    swingBoost += Vector2.up;
                }
                else if (rb.linearVelocityY < 0)
                {
                    swingBoost += Vector2.down;
                }
            }
        }
    }

    void DoForces()
    {
        if (isSwinging)
            rb.AddForce(swingBoost * swingingMovementBonus);
        DoMovement();
    }

    void DoMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        if (isSwinging)
        {
            // Movement
            if (rb.linearVelocityX < maxSpeed && moveInput.x > 0 ||
                rb.linearVelocityX > -maxSpeed && moveInput.x < 0)
            {
                rb.AddForce(new Vector2(moveInput.x * Acceleration / 2, 0));
            }

            // % Damping
            Vector2 counterForce = new Vector2(-rb.linearVelocityX / dampingFactor / 2, 0);
            rb.AddForce(counterForce);
        }
        else
        {
            // Movement
            if (rb.linearVelocityX < maxSpeed && moveInput.x > 0 ||
                rb.linearVelocityX > -maxSpeed && moveInput.x < 0)
            {
                rb.AddForce(new Vector2(moveInput.x * Acceleration, 0));
            }

            // Stop player if speed is approaching 0
            if (Mathf.Abs(moveInput.x) <= 0.1 && Mathf.Abs(rb.linearVelocityX) < 0.1)
                rb.linearVelocityX = 0;
            
            // % Damping
            Vector2 counterForce = new Vector2(-rb.linearVelocityX / dampingFactor, 0);
            rb.AddForce(counterForce);

            // Update player rotation
            if (rb.linearVelocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, targetRotation, Time.deltaTime * 10);
            }

            // Fast Fall
            if (moveInput.y < 0)
            {
                // Debug.Log("Fast Fall");
                rb.gravityScale = GRAVITYSCALE * FastFallMultiplier;
            }
            else
            {
                rb.gravityScale = GRAVITYSCALE;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // When player hits the ground
        if (collision.gameObject.GetComponent<PlatformEffector2D>() != null && !isSwinging)
            Bounce(collision);
    }

    private void Bounce(Collision2D collision)
    {
        Vector2 perpendicular = Vector2.Perpendicular(collision.GetContact(0).normal);

        // Project velocity onto vector parallel to the surface
        float dot = Vector2.Dot(perpendicular, rb.linearVelocity);
        Vector2 newVelocity = dot / math.square(perpendicular.magnitude) * perpendicular;

        newVelocity = new Vector2(newVelocity.x * BounceSpeedIncrease, newVelocity.y); // Increase x speed
        newVelocity += collision.GetContact(0).normal * BounceHeight; // Add bounce height

        rb.linearVelocity = newVelocity; // Update velocity
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // When player reaches the finish
        if (collider.gameObject.CompareTag("Finish"))
        {
            //Debug.Log("Collider: " + GameObject.GetComponent<NetworkObject>().OwnerClientId);
            Finish();
        }
    }

    private void Finish()
    {
        Debug.Log("Player reached the finish");
        try
        {
            Debug.Log("SetPlayerFinished() for clientId: " + RibbitRoyaleMultiplayer.Instance.GetPlayerData().clientId);
            RibbitRoyaleMultiplayer.Instance.SetPlayerFinished(true);
            scoreController = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
            //scoreController.SetPlayerFinished();

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
                scoreController.CalculatePlayerScores();
                return;
            }
        } catch (Exception e)
        {
            Debug.Log("Failed to get player data from clientId. Sending player back to prelobby: ERROR " + e);
            Loader.Load(Loader.Scene.PreLobbyScene);
            return;
        }
    }
    

    void UpdateAnimator()
    {
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("isSwinging", isSwinging);
    }

    private IEnumerator WaitForPopupDelay(float popupDelay)
    {
        SetPlayerActivity(false);
        yield return new WaitForSeconds(popupDelay);
        timerOver = true;
        SetPlayerActivity(true);
        yield return null;
    }

    private void SetPlayerActivity(bool active)
    {
        GetComponent<Collider2D>().enabled = active;
        rb.gravityScale = active ? GRAVITYSCALE : 0;
    }
    
}
