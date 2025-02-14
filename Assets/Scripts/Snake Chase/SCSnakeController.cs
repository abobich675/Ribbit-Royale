using UnityEngine;

public class SnakeController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player;
    public float moveSpeed = 3f;

    void Update()
    {
        // Move towards the player
        Vector3 targetPosition = new Vector3(player.position.x - 2, player.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
