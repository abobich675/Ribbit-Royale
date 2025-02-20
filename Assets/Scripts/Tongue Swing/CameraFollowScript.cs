using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public float followRate = 10;
    public float xOffset = 0;
    public float yOffset = 0;
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(target.position.x + xOffset, target.position.y + yOffset, -10);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = new Vector3(target.position.x + xOffset, target.position.y + yOffset, -10);
        transform.position += (targetPosition - transform.position) / followRate; // Move camera to the middle of the target and the camera
    }
}
