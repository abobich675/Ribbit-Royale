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
            SetupCamera();
        }
    }

    void SetupCamera()
    {
        // Create a new camera for this player
        playerCamera = new GameObject("Player Camera").AddComponent<Camera>();

        // Attach the camera to the player
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = new Vector3(0, 0, -10); // Offset the camera behind the player

        // Make sure this camera is active and rendering only for this player
        playerCamera.gameObject.tag = "MainCamera";
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
