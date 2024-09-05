using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkLoader : MonoBehaviour
{
    public ChunkManager chunkManager;
    public WorldEngine worldEngine;

    public Camera mainCamera;
    public GameObject grid;

    public Tile[] tiles; //I'd like to use scriptable objects instead of tiles.

    public Tile blankTile;

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
    private Vector2 playerPosition;
    private GameObject player;

    public void Init()
    {
        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform.position;
        playerChunkPosition = BiomeUtility.GetVariableChunkPosition(playerPosition);
        LoadChunksAroundPlayer();
    }
    
    void Update()
    {
        playerPosition = player.transform.position;
        Vector3Int newPlayerChunkPos = BiomeUtility.GetVariableChunkPosition(playerPosition);

        if(newPlayerChunkPos != playerChunkPosition)
        {
            playerChunkPosition = newPlayerChunkPos;
            LoadChunksAroundPlayer();
        }
    }

    void LoadChunksAroundPlayer()
    {
        List<Vector3Int> chunksToLoad = new List<Vector3Int>();

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

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

//This might be the source of the issue.
        if(chunkManager.chunkCache.ContainsKey(chunkPosition)){
            Debug.LogWarning($"Chunk at position {chunkPosition} already exists.");
            return;
        }

        ChunkData chunkData = chunkManager.LoadChunk(chunkPosition);

        if(chunkData == null){
            Debug.Log("Generating chunk from scratch.");
            chunkData = worldEngine.GenerateChunk(chunkPosition);
            if(chunkData != null){
                chunkManager.SaveChunk(chunkPosition, chunkData);
            }else{
                Debug.LogWarning($"Failed to generate chunk at {chunkPosition}");
            }
        }
        Biome[,] biomeMap = BiomeUtility.ListToBiomeArray(chunkData.biomeMapList, chunkSize, chunkSize);

        DrawBiomeMap(biomeMap, chunkTilemap, chunkPosition);

        chunkManager.AddChunk(chunkPosition, chunk);
    }

    void UnloadChunk(Vector3Int chunkPosition)
    {
        if(chunkManager.chunkCache.TryGetValue(chunkPosition, out GameObject chunk))
        {
            chunkManager.RemoveChunk(chunkPosition, chunk);
            Destroy(chunk);
        }
    }

    void DrawBiomeMap(Biome[,] biomeMap, Tilemap chunkTilemap, Vector3Int chunkPosition)
    {
        int width = biomeMap.GetLength(0);
        int height = biomeMap.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Biome tileBiome = biomeMap[x, y];
                TileBase selectedTile = BiomeUtility.GetTileFromBiome(tileBiome);
                if(selectedTile == null){
                    selectedTile = blankTile;
                }
                Vector3Int tilePosition = new Vector3Int(chunkPosition.x * width + x, chunkPosition.y * height + y, 0);

                chunkTilemap.SetTile(tilePosition, selectedTile);
            }
        }
    }
}
