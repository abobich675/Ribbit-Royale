using UnityEngine;

public class FrogController : MonoBehaviour
{
    public float moveSpeed = 5f;     // Speed for horizontal movement
    public float jumpForce = 7f;    // Force applied when jumping
    private bool isGrounded = true; // Tracks if the player is on the ground
    private Rigidbody2D rb;         // Reference to the Rigidbody2D component

    void Start()
    {
        // Initialize the Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player lands on a surface (ground detection)
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
