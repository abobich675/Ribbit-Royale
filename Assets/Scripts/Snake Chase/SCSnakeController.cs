using Unity.Netcode;
using System;
using System.Collections;
using System.Net;
using UI.Scoreboard;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SnakeController : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float moveSpeed = 5.0f;
    private Transform target;
    private Rigidbody2D rb;
    private bool timerOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(StartDelay());

        moveSpeed = 5.0f;
        FindPlayer();

    }

    IEnumerator StartDelay()
    {
        moveSpeed = 0.0f;
        yield return new WaitForSeconds(8f);
        timerOver = true;
        yield return null;

    }

    void Update()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // Move towards the player
        if (timerOver)
        {
            if (IsServer) // Ensure only the server moves the snake
            {
                MoveSnakeServerRpc(target.position.x);
            }
        }
    }

    [ServerRpc]
    void MoveSnakeServerRpc(float playerX)
    {
        Vector3 targetPosition = new Vector3(playerX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Destroy(collision.gameObject, 0.1f);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            NetworkObject playerNetworkObject = collision.gameObject.GetComponent<NetworkObject>();
            if (playerNetworkObject != null && playerNetworkObject.IsSpawned)
            {
                playerNetworkObject.Despawn();
            }

            FindPlayer();
        }
    }

    void FindPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            target = null; // No players left to chase
            return;
        }

        Transform closestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            // Ensure player has a NetworkObject and is still active
            NetworkObject netObj = player.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPlayer = player.transform;
                }
            }
        }

        target = closestPlayer;
    }
}
