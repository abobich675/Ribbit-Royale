using System;
using Unity.Netcode;
using UnityEngine;
using UI.Scoreboard;

public class SnakeCollider : MonoBehaviour
{
    private ScoreController scoreCon;
    void Awake()
    {
        //var thisCol = gameObject.GetComponent<BoxCollider2D>();
        try
        {
            scoreCon = GameObject.FindGameObjectWithTag("ScoreControllerGO").GetComponent<ScoreController>();
        }
        catch (Exception e)
        {
            Debug.Log("Could not create scoreCon variable: ERROR " + e);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided With: " + other.GetType());
        if (other.gameObject.CompareTag("Player"))
        {
            scoreCon.SetPlayerDNF(other.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }
}