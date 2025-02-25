using UnityEngine;
using Unity.Netcode;

public class TSNetworkController : NetworkBehaviour
{
    [SerializeField] private Transform playerPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPlayer(ulong clientId){
        Transform playerTransform = Instantiate(playerPrefab);
        playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
