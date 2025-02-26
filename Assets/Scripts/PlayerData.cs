using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    // Create a variable to store the clientId when they join a lobby
    public ulong clientId;
    // Create a variable to store the colorId of the user when they join the lobby
    public int colorId;

    public Vector3 playerPos;
    public Quaternion playerRot;

    // This function will store player data based off of that players clientId
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId;
    }
    // This function will allow you to transfer the player data over a network
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
    }
}
