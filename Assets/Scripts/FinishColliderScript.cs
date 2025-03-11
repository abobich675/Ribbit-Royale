using Unity.Netcode;
using UnityEngine;

public class FinishColliderScript : MonoBehaviour
{
    private ScoreController scoreCon;
    void Awake()
    {
        //var thisCol = gameObject.GetComponent<BoxCollider2D>();
        scoreCon = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided With: " + other.GetType());
        if (other.gameObject.CompareTag("Player"))
        {
            scoreCon.DidFinish(other.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }
}