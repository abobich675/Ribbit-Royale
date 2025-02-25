using Unity.Netcode;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    const float GRAVITYSCALE = 2f;

    float Acceleration = 5;
    float FastFallMultiplier = 1.25f;
    float BounceSpeedIncrease = 3;
    float BounceHeight = 12.5f;

    public float maxSpeed;
    public float dampingFactor;

    public float swingingMovementBonus;

    public new Camera camera;
    public GameObject tongue;
    public RuntimeAnimatorController[] animators;

    GameObject connectedObject;
    bool isSwinging = false;



    Rigidbody2D rb;
    Animator animator;

    InputAction moveAction;
    InputAction attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        moveAction = input.actions["Move"];
        attackAction = input.actions["Attack"];

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        tongue = Instantiate(tongue);
        tongue.SetActive(false);

        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        DoInteraction();
        DoMovement();
        UpdateAnimator();
    }
    
    void SetColor() {
        PlayerData playerData;
        try {
            // This will get a players data based off the clientId 
            playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        } catch (Exception e) {
            Debug.Log("Failed to get player data from clientId");
            return;
        }
        
        if (playerData.colorId < 0 || playerData.colorId >= animators.Length) {
            Debug.LogError("Player color ID out of bounds");
            return;
        }

        animator.runtimeAnimatorController = animators[playerData.colorId];
        // Using the player data call the function SetPlayerColor, get the players color using the playerData
        // RibbitRoyaleMultiplayer.Instance.GetPlayerColor(playerData.colorId)
    }

    void DoInteraction()
    {
        // Add tongue if click started
        if (attackAction.triggered)
        {
            // Locate object to connect to
            Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
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
            if (transform.position.y < connectedObject.transform.position.y)
            {
                if (rb.linearVelocityX > 0)
                {
                    rb.AddForce(Vector2.right * swingingMovementBonus);
                }
                else if (rb.linearVelocityX < 0)
                {
                    rb.AddForce(Vector2.left * swingingMovementBonus);
                }

                if (rb.linearVelocityY > 0)
                {
                    rb.AddForce(Vector2.up * swingingMovementBonus);
                }
                else if (rb.linearVelocityY < 0)
                {
                    rb.AddForce(Vector2.down * swingingMovementBonus);
                }
            }
        }
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
        {
            Vector2 perpendicular = Vector2.Perpendicular(collision.GetContact(0).normal);

            // Project velocity onto vector parallel to the surface
            float dot = Vector2.Dot(perpendicular, rb.linearVelocity);
            Vector2 newVelocity = dot / math.square(perpendicular.magnitude) * perpendicular;

            newVelocity = new Vector2(newVelocity.x * BounceSpeedIncrease, newVelocity.y); // Increase x speed
            newVelocity += collision.GetContact(0).normal * BounceHeight; // Add bounce height

            rb.linearVelocity = newVelocity; // Update velocity
        }
    }

    void UpdateAnimator()
    {
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("isSwinging", isSwinging);
    }
}
