using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    float speed = 5;

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
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        rb.linearVelocityX = moveInput.x * speed;

        if (interactAction.triggered)
        {
            Debug.Log("Interact");
            // Vector3 mousePos = Input.mousePosition;
            // {
            //     Debug.Log(mousePos.x);
            //     Debug.Log(mousePos.y);
            // }
        }
    }
}
