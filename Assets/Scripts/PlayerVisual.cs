using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    // Create a field for altering the sprites color 
    [SerializeField] private SpriteRenderer spriteRenderer;
    // This function will set the color of the player based on the color being passed in 
    public void SetPlayerColor(Color color){
        spriteRenderer.color = color;
    }
}
