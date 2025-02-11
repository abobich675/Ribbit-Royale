using UnityEngine;
using UnityEngine.Analytics;

public class CTAAnimalScript : MonoBehaviour
{

    public string animalType = null;
    public GameObject leftBarrier;
    public GameObject rightBarrier;
    int spawnLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnLocation = Random.Range(0, 2); // 0 is left, 1 is right
    }

    // Update is called once per frame
    void Update()
    {
        if (animalType == null) {
            return;
        }
    }
}
