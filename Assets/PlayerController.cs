using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const float GRAVITYSCALE = 1;

    float Speed = 5;
    float FastFallMultiplier = 1.25f;

    public Camera camera;



    Rigidbody2D rb;

    InputAction moveAction;
    InputAction interactAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        moveAction = input.actions["Move"];
        interactAction = input.actions["Interact"];

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement();
        DoInteraction();

        void DoMovement()
        {
            // Movement
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            rb.linearVelocityX = moveInput.x * Speed;

            // Fast Fall
            if (moveInput.y < 0)
            {
                Debug.Log("Fast Fall");
                rb.gravityScale = GRAVITYSCALE * FastFallMultiplier;
            }
            else
            {
                rb.gravityScale = GRAVITYSCALE;
            }
        }

        void DoInteraction()
        {
            if (interactAction.triggered)
            {
                Debug.Log("Interact Triggered");

                Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

                foreach (RaycastHit2D hit in hits)
                {
                    Debug.Log(hit.transform.name);
                }
            }
        }
    }
}
