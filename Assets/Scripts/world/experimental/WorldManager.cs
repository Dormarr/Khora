using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TicketManager;

public static class WorldManager
{
    // Aiming to replace ChunkManager and ChunkLoader.
    // Should respond to tickets submitted to load/unload chunks.

    // Do I need to have the chunks cached here as before?

    // Make these a bit more dynamic down the line.
    private static readonly int _fullyLoadDistance = 1;
    private static readonly int _entityTickDistance = 3;
    private static readonly int _tileTickDistance = 4;
    private static readonly int _softLoadDistance = 5;
    private static readonly int _unloadDistance = 6;

    public static Dictionary<Vector3Int, Chunk> activeChunks = new Dictionary<Vector3Int, Chunk>();

    public static void HandleChunkTicket(ChunkTicket chunkTicket){
        
        switch (chunkTicket.Status)
        {
            case ChunkStatus.FullLoad:
                LoadChunk(chunkTicket.ChunkPosition);
                break;
            case ChunkStatus.EntityTick:
                TickEntitiesInChunk(chunkTicket.ChunkPosition);
                break;
            case ChunkStatus.TileTick:
                TickTilesInChunk(chunkTicket.ChunkPosition);
                break;
            case ChunkStatus.SoftLoad:
                SoftLoadChunk(chunkTicket.ChunkPosition);
                break;
            case ChunkStatus.Unload:
                UnloadChunk(chunkTicket.ChunkPosition);
                break;
            default:
                break;
        }
    }

    // Check active chunks, if null, then check save data for stuff you might need.


    private static void LoadChunk(Vector3Int chunkPosition, int testTileIndex = 0){
        Debug.Log($"WorldManager.LoadChunk: Called to load chunk {chunkPosition}");
        if(activeChunks.ContainsKey(chunkPosition)){
            //UnloadChunk(chunkPosition);
            return;
        }
        if(!activeChunks.ContainsKey(chunkPosition)){
            Chunk chunk = AddChunk(chunkPosition);
            if(chunk != null){
                // WorldRenderer.RenderTestChunk(chunk, testTileIndex);
                WorldRenderer.RenderChunk(chunk);
            }
            else{
                Debug.LogWarning($"WorldManager.LoadChunk: Chunk {chunkPosition} cannot be rendered as it is null.");
            }
        }else{
            Debug.Log($"WorldManager.LoadChunk: Chunk {chunkPosition} already exists.");
        }
    }
    private static void TickEntitiesInChunk(Vector3Int chunkPosition){
        // Will implement more specific functionality later. Right now, will just load chunks.
        UnloadChunk(chunkPosition);
        // LoadChunk(chunkPosition, 1);

        // Load with different gamerules in place.
    }
    private static void TickTilesInChunk(Vector3Int chunkPosition){
        // Will implement more specific functionality later. Right now, will just load chunks.
        UnloadChunk(chunkPosition);
        // LoadChunk(chunkPosition, 2);

        // Load with different gamerules in place.
    }
    private static void SoftLoadChunk(Vector3Int chunkPosition){
        // Will implement more specific functionality later. Right now, will just load chunks.
        UnloadChunk(chunkPosition);
        // LoadChunk(chunkPosition, 3);

        // Load with minimal sim and reduced rendering.
    }
    private static void UnloadChunk(Vector3Int chunkPosition)
    {
        if (activeChunks.ContainsKey(chunkPosition))
        {
            // Get the chunk from the cache.
            Chunk chunk = activeChunks[chunkPosition];
            // Destroy the associated GameObject.
            if (chunk != null && chunk.ChunkGameObject != null)
            {
                if (chunk.ChunkGameObject != null)
                {
                    GameObject.Destroy(chunk.ChunkGameObject);
                    Debug.Log($"WorldManager.UnloadChunk: Destroyed Chunk {chunkPosition}");
                }
                else
                {
                    Debug.LogWarning($"WorldManager.UnloadChunk: Chunk GameObject was already destroyed or not assigned: {chunkPosition}");
                }
            }
            else
            {
                Debug.LogWarning($"WorldManager.UnloadChunk: Could not destroy Chunk {chunkPosition}");
            }

            activeChunks.Remove(chunkPosition);
        }
        else
        {
            // Debug.LogWarning($"Attempted to unload chunk at {chunkPosition}, but it does not exist.");
        }
    }

    private static Chunk AddChunk(Vector3Int chunkPosition){
        Chunk newChunk = new Chunk(chunkPosition);
        activeChunks.Add(chunkPosition, newChunk);
        return newChunk;
    }

    public static void AssessAndSubmitTickets(Vector3Int playerChunkPosition){
        // Iterate through chunks in square around player chunk pos.

        for(int x = -_unloadDistance; x <= _unloadDistance; x++){
            for(int y = -_unloadDistance; y <= _unloadDistance; y++){
                Vector3Int chunkPosition = new Vector3Int(playerChunkPosition.x + x, playerChunkPosition.y + y, 0);
                int distance = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                int priority;

                ChunkStatus targetStatus;
                if(distance <= _fullyLoadDistance){
                    targetStatus = ChunkStatus.FullLoad;
                    priority = 1;
                }
                else if(distance <= _entityTickDistance){
                    targetStatus = ChunkStatus.EntityTick;
                    priority = 2;
                }
                else if(distance <= _tileTickDistance){
                    targetStatus = ChunkStatus.TileTick;
                    priority = 3;
                }
                else if(distance <= _softLoadDistance){
                    targetStatus = ChunkStatus.SoftLoad;
                    priority = 4;
                }
                else{
                    targetStatus = ChunkStatus.Unload;
                    priority = 5;
                }

                ChunkTicket ticket = new ChunkTicket(chunkPosition, targetStatus, priority);
                SubmitTicket(ticket);
            }
        }
    }

}