using Unity.Netcode;
using UnityEngine;

public class PlayerCtrl : NetworkBehaviour
{
    public float movSpeed;
    private float speedx, speedy;
    private Rigidbody2D rb;
    private Camera playerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure only the local player has an active camera
        if (IsOwner)
        {
            playerCamera = Camera.main; // Get the main camera
            playerCamera.transform.SetParent(transform); // Attach it to this player
            playerCamera.transform.localPosition = new Vector3(0, 0, -10); // Adjust camera position
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        speedx = Input.GetAxisRaw("Horizontal") * movSpeed;
        speedy = Input.GetAxisRaw("Vertical") * movSpeed;

        rb.linearVelocity = new Vector2(speedx, speedy);
    }
}
