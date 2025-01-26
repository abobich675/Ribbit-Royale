using Unity.Netcode;
using UnityEngine;

public class PlayerCtrl : NetworkBehaviour
{
    public float movSpeed;
    float speedx, speedy;
    Rigidbody2D rb; // Correct type is Rigidbody2D for 2D physics

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Correctly get the Rigidbody2D component
    }

    // Update is called once per frame
    void Update()
    {
        speedx = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedy = Input.GetAxisRaw("Vertical") * movSpeed;

        // Use linearVelocity or velocity for Rigidbody2D
        rb.linearVelocity = new Vector2(speedx, speedy);
    }
}
