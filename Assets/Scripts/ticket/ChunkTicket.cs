using System;
using UnityEngine;

public enum ChunkStatus{
    FullLoad,
    EntityTick,
    TileTick,
    SoftLoad,
    Unload
}
public class ChunkTicket : Ticket
{
    public Vector3Int ChunkPosition { get; private set; }
    public ChunkStatus Status { get; private set; }

    public ChunkTicket(Vector3Int chunkPosition, ChunkStatus status, int priority, float targetExectuionTime = 0f)
        : base(priority, ActionType.Load, targetExectuionTime)
    {
        ChunkPosition = chunkPosition;
        Status = status;
    }
    

    public override void Execute(){
        WorldManager.HandleChunkTicket(this);   
    }
}
