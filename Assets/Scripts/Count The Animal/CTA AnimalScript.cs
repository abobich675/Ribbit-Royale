using UnityEngine;

public class CTAAnimalScript : MonoBehaviour
{
    public float VELOCITY = 1;
    public string animalType = null;
    public GameObject leftBarrier;
    public GameObject rightBarrier;

    Rigidbody2D rb;
    int spawnLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        SetSpawnLocation();
    }

    // Update is called once per frame
    void Update()
    {
        if (animalType == null) {
            return;
        }
    }

    // Chooses and sets a random spawn location for the animal
    // Chooses between the left and right side
    // Sets a random y position based on the animal type
    void SetSpawnLocation()
    {
        // Pick side to spawn on
        spawnLocation = Random.Range(0, 2); // 0 is left, 1 is right
        float yPos = 0;
        switch (animalType)
        {
            case "fly":
                yPos = Random.Range(1f, 4f);
                break;
            case "bird":
                yPos = Random.Range(1f, 4f);
                break;
            case "monkey":
                yPos = Random.Range(-2f, 2f);
                break;
            case "turtle":  
                yPos = Random.Range(-1f, -4f);
                break;
            case "snake":
                yPos = Random.Range(-1f, -4f);
                break;
        }

        // Set Position
        if (spawnLocation == 0)
        {
            transform.position = new Vector3(leftBarrier.transform.position.x, yPos, transform.position.z);
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
            transform.position = new Vector3(rightBarrier.transform.position.x, yPos, transform.position.z);
        }

        SetSpawnVelocity(spawnLocation);
    }

    // Sets the velocity of the animal
    // Moves across the screen with slight vertical movement
    // Vertical movement will tend towards the center of the screen
    void SetSpawnVelocity(int spawnLocation)
    {
        float yVel = 0;
        switch (animalType)
        {
            case "fly":
                yVel = Random.Range(-1f, 0.5f);
                break;
            case "bird":
                yVel = Random.Range(-1f, 0.5f);
                break;
            case "monkey":
                yVel = 0;
                break;
            case "turtle":  
                yVel = Random.Range(-0.5f, 1f);
                break;
            case "snake":
                yVel = Random.Range(-0.5f, 1f);
                break;
        }

        // Set Velocity
        if (spawnLocation == 0)
        {
            rb.linearVelocity = new Vector2(VELOCITY, yVel);
        }
        else
        {
            rb.linearVelocity = new Vector2(-VELOCITY, yVel);
        }
    }
    
    // Detects collision with the barriers
    // Destroys the animal if it collides with the proper barrier
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.linearVelocityX < 0 && collision.gameObject == leftBarrier)
        {
            Destroy(gameObject);
        }
        else if (rb.linearVelocityX > 0 && collision.gameObject == rightBarrier)
        {
            Destroy(gameObject);
        }
    }
}
