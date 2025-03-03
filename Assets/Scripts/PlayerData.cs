using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    // Create a variable to store the clientId when they join a lobby
    public ulong clientId;
    // Create a variable to store the colorId of the user when they join the lobby
    public int colorId;
    public bool finished;

    // Count The Animal Game
    public int countedAnimalIndex;
    public int currentCount;
    public int finalCount;
    public int playerScore;
    
    public int previousRoundPlayerScore;

    // This function will store player data based off of that players clientId
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && 
               colorId == other.colorId &&
               playerScore == other.playerScore && 
               previousRoundPlayerScore == other.previousRoundPlayerScore;
    }
    // This function will allow you to transfer the player data over a network
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref countedAnimalIndex);
        serializer.SerializeValue(ref currentCount);
        serializer.SerializeValue(ref finalCount);
        serializer.SerializeValue(ref playerScore);
        serializer.SerializeValue(ref previousRoundPlayerScore);
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }
    
}
