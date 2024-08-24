using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    //this is where I'll keep all the loaded/unloaded chunk info.
    //Cache memory for faster performance at the expense of memory, but chunks aren't that intensive so it's all good.
    public bool gate = false;

    [SerializeField] private ChunkLoader chunkLoader;

    public Dictionary<Vector3Int, GameObject> chunkCache = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        InitializeChunks();
    }

    void InitializeChunks()
    {
        //load chunks through chunkloader.
        //add each chunk to the chunkCache.

        chunkLoader.Init();
        if(chunkCache.Count > 0 )
        {
            gate = true;
        }

    }


    public void AddChunk(Vector3Int chunkPosition, GameObject chunk)
    {
        //add chunk based on chunkPosition to chunkCache.

        if(!chunkCache.ContainsKey(chunkPosition))
        {
            chunkCache[chunkPosition] = chunk;
        }


    }

    public void RemoveChunk(Vector3Int chunkPosition, GameObject chunk)
    {
        //remove chunk.

        if(chunkCache.ContainsKey(chunkPosition))
        {
            chunkCache.Remove(chunkPosition);
        }
    }

    //Add an IdentifyTile method next, so you can identify the tile based on coordinates? IDK, figure it out.

    public TileBase IdentifyTile(Vector3 position)
    {
        int chunkPositionX = Mathf.FloorToInt(position.x / Config.chunkSize);
        int chunkPositionY = Mathf.FloorToInt(position.y / Config.chunkSize);
        Vector3Int chunkPosition = new Vector3Int(chunkPositionX, chunkPositionY, 0);

        if (chunkCache.ContainsKey(chunkPosition))
        {
            GameObject chunk = chunkCache[chunkPosition];
            Tilemap chunkTilemap = chunk.GetComponent<Tilemap>();
            Vector3Int tilePos = chunkTilemap.WorldToCell(position);
            return chunkTilemap.GetTile(tilePos);
        }

        return null;
    }

    public GameObject GetChunkTilemap()
    {
        //Vector3Int chunkPosition = chunkLoader.GetMouseChunkPosition();
        Vector3Int chunkPosition = Utility.GetVariableChunkPosition(Utility.GetMouseWorldPosition());

        if (chunkCache.ContainsKey(chunkPosition))
        {
            return chunkCache[chunkPosition];
        }

        UnityEngine.Debug.Log("Oopsie, No Chunk Position Found!");
        return null;

    }



}
