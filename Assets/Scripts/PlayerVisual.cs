using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

  
    public void SetPlayerColor(Color color){
        spriteRenderer.color = color;
    }
    
}
