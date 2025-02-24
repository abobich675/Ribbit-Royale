using Unity.Netcode;
using UnityEngine;

public class PlayerCtrl : NetworkBehaviour
{
    [SerializeField] private PlayerVisual playerVisual; // Reference to PlayerVisual    
    public float movSpeed;
    private float speedx, speedy;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // This will get a players data based off the clientId 
        PlayerData playerData = RibbitRoyaleMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        // Using the player data call the function SetPlayerColor, get the players color using the playerData
        playerVisual.SetPlayerColor(RibbitRoyaleMultiplayer.Instance.GetPlayerColor(playerData.colorId));


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
