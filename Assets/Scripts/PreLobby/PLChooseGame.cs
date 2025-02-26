using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor.SearchService;
#endif 
using System.Threading;
using Unity.VisualScripting;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class PLChooseGame : NetworkBehaviour
{

    [Serializable]
    public struct Game
    {
        public string name;
        public GameObject colliderObject;
        public Loader.Scene scene;
        public int count;
    }

    public List<Game> games;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ulong playerId = RibbitRoyaleMultiplayer.Instance.GetPlayerData().clientId;
        ulong ownerId = NetworkManager.Singleton.CurrentSessionOwner;
        bool isHost = ownerId == playerId;
        if (isHost)
        {
            TallyVotes();
        }
    }

    private void TallyVotes()
    {
        if (games.Count == 0)
            return;
        int totalVotes = 0;
        Game mostPopularGame = games[0];
        for (int i = 0; i < games.Count; i++)
        {   
            totalVotes += games[i].count;

            if (games[i].count > mostPopularGame.count)
                mostPopularGame = games[i];
        }

        int totalPlayers = RibbitRoyaleMultiplayer.Instance.GetPlayerCount();
        // Check if all players have voted
        if (totalVotes == totalPlayers)
        {
            Loader.LoadNetwork(mostPopularGame.scene);
            return;
        }
    }

    public void AddGame(Game game)
    {
        Debug.Log("Adding game: " + game.name);
        games.Add(game);
    }

    public void UpdateGameCount(string gameName, int count)
    {
        for (int i = 0; i < games.Count; i++)
        {
            if (games[i].name == gameName)
            {
                Game game = games[i];
                game.count = count;
                games[i] = game;
            }
        }
    }
}
