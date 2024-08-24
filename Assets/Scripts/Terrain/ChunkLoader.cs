using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkLoader : MonoBehaviour
{
    public ChunkManager chunkManager;

    public Camera camera;
    public GameObject grid;

    public Tile[] tiles; //I'd like to use scriptable objects instead of tiles.

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    private int chunkSize => Config.chunkSize;
    public float padding;

    private Vector3Int playerChunkPosition;
    private Dictionary<Vector3Int, GameObject> loadedChunks = new Dictionary<Vector3Int, GameObject>();

    private GameObject player;

    public void Init()
    {
        player = GameObject.FindWithTag("Player");
        playerChunkPosition = GetPlayerChunkPosition();
        LoadChunksAroundPlayer();
    }
    
    void Update()
    {
        Vector3Int newPlayerChunkPos = GetPlayerChunkPosition();

        if(newPlayerChunkPos != playerChunkPosition)
        {
            playerChunkPosition = newPlayerChunkPos;
            LoadChunksAroundPlayer();
        }
    }

    public Vector3Int GetPlayerChunkPosition()
    {
        Vector3 playerPosition = player.transform.position;
        return new Vector3Int(Mathf.FloorToInt(playerPosition.x / chunkSize), Mathf.FloorToInt(playerPosition.y / chunkSize), 0);
    }

    public Vector3Int GetMouseChunkPosition(Vector2 mousePos)
    {
        return new Vector3Int(Mathf.FloorToInt(mousePos.x / chunkSize), Mathf.FloorToInt(mousePos.y / chunkSize), 0);
    }

    void LoadChunksAroundPlayer()
    {
        HashSet<Vector3Int> chunksToLoad = new HashSet<Vector3Int>();

        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));

        // Calculate chunk coordinates covered by the camera view with padding
        int minX = Mathf.FloorToInt((bottomLeft.x - padding) / chunkSize);
        int maxX = Mathf.FloorToInt((topRight.x + padding) / chunkSize);
        int minY = Mathf.FloorToInt((bottomLeft.y - padding) / chunkSize);
        int maxY = Mathf.FloorToInt((topRight.y + padding) / chunkSize);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int chunkPosition = new Vector3Int(x, y, 0);

                if (!chunkManager.chunkCache.ContainsKey(chunkPosition))
                {
                    LoadChunk(chunkPosition);
                }

                chunksToLoad.Add(chunkPosition);
            }
        }



        List<Vector3Int> chunksToUnload = new List<Vector3Int>();

        foreach(var chunk in chunkManager.chunkCache.Keys)
        {
            if (!chunksToLoad.Contains(chunk))
            {
                chunksToUnload.Add(chunk);
            }
        }

        foreach(var chunk in chunksToUnload)
        {
            UnloadChunk(chunk);
        }
    }

    void LoadChunk(Vector3Int chunkPosition)
    {
        GameObject chunk = new GameObject("Chunk_" + chunkPosition);
        chunk.transform.parent = grid.transform;

        Tilemap chunkTilemap = chunk.AddComponent<Tilemap>();
        TilemapRenderer chunkRenderer = chunk.AddComponent<TilemapRenderer>();


        chunkTilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0);

        float[,] noiseMap = Noise.GenerateChunkNoiseMap(chunkPosition, chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        DrawNoiseMap(noiseMap, chunkTilemap, chunkPosition);

        //loadedChunks.Add(chunkPosition, chunk);
        chunkManager.AddChunk(chunkPosition, chunk);
    }

    void UnloadChunk(Vector3Int chunkPosition)
    {
        if(chunkManager.chunkCache.TryGetValue(chunkPosition, out GameObject chunk))
        {
            Destroy(chunk);//will this screw up saving? Is this gonna bite me in the arse??
            //loadedChunks.Remove(chunkPosition);
            chunkManager.RemoveChunk(chunkPosition, chunk);
        }
    }


    void DrawNoiseMap(float[,] noiseMap, Tilemap chunkTilemap, Vector3Int chunkPosition)
    {

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Tile selectedTile = SelectTile(noiseMap[x, y]);
                //Vector3Int tilePosition = new Vector3Int(chunkPosition.x * chunkSize + x - (chunkSize/2), chunkPosition.y * chunkSize + y - (chunkSize / 2), 0);
                Vector3Int tilePosition = new Vector3Int(chunkPosition.x * width + x, chunkPosition.y * height + y, 0);
                
                chunkTilemap.SetTile(tilePosition, selectedTile);
            }
        }
    }

    Tile SelectTile(float perlinValue)
    {
        int i = Mathf.RoundToInt(perlinValue * (tiles.Length - 1));
        int tileIndex = Mathf.Clamp(i, 0, tiles.Length - 1);
        return tiles[tileIndex];
    }

}
