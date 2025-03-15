using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct ScoreboardData  : INetworkSerializable, IEquatable<ScoreboardData>
{
    public ulong clientId;
    public FixedString128Bytes userName;
    public int score;

    public bool Equals(ScoreboardData other)
    {
        return clientId==other.clientId &&
            userName.Equals(other.userName) &&
            score==other.score;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref userName);
        serializer.SerializeValue(ref score);
    }
}
