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
    public float moveSpeed = 4.0f;
    private Transform target;
    private Rigidbody2D rb;
    private bool timerOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(StartDelay());

        moveSpeed = 4.0f;
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
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + 3, target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Destroy(collision.gameObject, 0.1f);
        }
    }

    void FindPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        try
        {
            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().IsOwner)
                {
                    target = player.transform;
                    return;
                }
            }
        }
        catch
        {
            if (players.Length > 0)
                target = players[0].transform;
        }
    }
}
