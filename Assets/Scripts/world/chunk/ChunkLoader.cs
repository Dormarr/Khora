using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Profiling;

public class ChunkLoader : MonoBehaviour
{
    public ChunkManager chunkManager;
    public WorldEngine worldEngine;
    public UVColourMap uVColourMap;

    public Camera mainCamera;
    public GameObject grid;
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
    [SerializeField] private GameObject player;

    public void Initialize()
    {
        seed = WorldDataTransfer.worldSeed;
        UnityMainThreadDispatcher.Instance();

        playerPosition = player.transform.position;
        playerChunkPosition = BiomeUtility.GetVariableChunkPosition(playerPosition);
        
        //Should I wait for the registries to be done?
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

        //add in the extra padding for the generated chunks.

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

        //Add in another round of chunks to load without rendering.

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

    async void LoadChunk(Vector3Int chunkPosition, bool softLoad = false)
    {
        //Create game object.
        GameObject chunk = new GameObject($"Chunk_{chunkPosition}");
        chunk.isStatic = true;
        chunk.transform.parent = grid.transform;

        //Generation tilemap.
        Tilemap chunkTilemap = chunk.AddComponent<Tilemap>();
        TilemapRenderer chunkRenderer = chunk.AddComponent<TilemapRenderer>();

        //Modification tilemap.
        GameObject chunkChild = new GameObject($"Modified_{chunkPosition}");
        chunkChild.transform.parent = chunk.transform;

        Tilemap chunkChildTilemap = chunkChild.AddComponent<Tilemap>();
        TilemapRenderer chunkChildRenderer = chunkChild.AddComponent<TilemapRenderer>();
        chunkChildRenderer.sortingOrder = 1;

        chunkTilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0);

        if(chunkManager.chunkCache.ContainsKey(chunkPosition)){
            Debug.LogWarning($"Chunk at position {chunkPosition} already exists.");
            return;
        }

        ChunkData chunkData = await Task.Run(() => worldEngine.GenerateChunkAsync(chunkPosition));

        // If the chunkData generation failed or returned null, abort.
        if (chunkData == null)
        {
            Debug.LogError($"Failed to generate chunk data for chunk at position {chunkPosition}.");
            return;
        }

        // Load modification data synchronously (assuming this is quick).
        ChunkData modChunkData = chunkManager.LoadChunk(chunkPosition);

        // Soft loaded chunks exist to supply data to the rendering process.
        // They exist computationally only (unrendered), but otherwise function as main chunks.
        if(softLoad){
            Debug.Log($"Chunk {chunkPosition} was Loaded Softly.");
            chunkManager.AddSoftChunk(chunkPosition, chunk);
            return;
        }

        // Add the chunk to the cache.
        chunkManager.AddChunk(chunkPosition, chunk);

        // Render the chunk.
        await Task.Run(async () =>
        {
            await RenderChunk(chunkData, modChunkData, chunkPosition, chunkTilemap, chunkChildTilemap);
        });

        Debug.Log($"Chunk at {chunkPosition} successfully loaded.");
    }

    async Task RenderChunk(ChunkData chunkData, ChunkData modChunkData, Vector3Int chunkPosition, Tilemap chunkTilemap, Tilemap chunkChildTilemap)
    {
        // Begin profiling the entire chunk rendering process
        Debug.Log("RenderChunk: Processing Started.");

        // Extract biomeMap from the chunkData and precompute the array
        Biome[,] biomeMap = BiomeUtility.ListToArray(chunkData.biomeMapList, chunkSize, chunkSize);

        // Render the biome map onto the chunk's tilemap
        await Task.Run(() => DrawBiomeMap(biomeMap, chunkTilemap, chunkPosition));

        // If there is modification data, apply it to the child tilemap
        if (modChunkData != null)
        {
            await Task.Run(() => DrawModificationMap(modChunkData.tileDataList, chunkChildTilemap, chunkPosition));
        }

        // End profiling for the entire chunk rendering process
        Debug.Log("RenderChunk: Processing Finished.");
    }

    void UnloadChunk(Vector3Int chunkPosition)
    {
        //rejig to unload only once a certain distance away.
        //Maybe unrender but don't have to completely unload it.

        if(chunkManager.chunkCache.TryGetValue(chunkPosition, out GameObject chunk))
        {
            chunkManager.SaveModifications();
            chunkManager.RemoveChunk(chunkPosition, chunk);
            Destroy(chunk);
        }
    }

    async void DrawModificationMap(List<TileData> tileDataList, Tilemap chunkTilemap, Vector3Int chunkPosition){
        if(tileDataList == null){
            Debug.LogError("Unable to DrawModificationMap as tileDataList was null.");
            return;
        }

        await Task.Run(() =>
        {
            for(int i = 0; i < tileDataList.Count; i++){
                TileData tileData = tileDataList[i];

                int x = tileData.x;
                int y = tileData.y;
                
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                chunkTilemap.SetTile(tilePosition, blankTile);
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                });
            }
        });

    }
    async Task DrawBiomeMap(Biome[,] biomeMap, Tilemap chunkTilemap, Vector3Int chunkPosition)
    {
        int width = biomeMap.GetLength(0);
        int height = biomeMap.GetLength(1);

        // Get the selected tile only once from the main thread
        GradientTile selectedTile = await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
        {
            return BiomeUtility.GetGradientTileByName("grass");
        });

        // Create a list of tasks for setting tiles
        List<Task> tileTasks = new List<Task>();


        int localX = chunkPosition.x * width;
        int localY = chunkPosition.y * height;

        // Process the biome map asynchronously and only set tiles on the main thread
        await Task.Run(() =>
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3Int tilePosition = new Vector3Int(localX + x, localY + y, 0);

                    if(tilePosition.x > localX + width || tilePosition.x < localX || tilePosition.y > localY + height || tilePosition.y < localY){
                        continue;
                    }

                    // Only enqueue tile setting on the main thread when necessary
                    tileTasks.Add(UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
                    {
                        chunkTilemap.SetTile(tilePosition, selectedTile);
                        return Task.CompletedTask;
                    }));
                }
            }
            // Generate temperature and precipitation maps asynchronously

            // Call RenderTileColours with temperature and precipitation maps asynchronously

        });

        float[,] temperatureMap = worldEngine.temperatureGenerator.GenerateChunkPerlinWithBorder(chunkPosition, WorldDataTransfer.worldSeed);
        float[,] precipitationMap = worldEngine.precipitationGenerator.GenerateChunkPerlinWithBorder(chunkPosition, WorldDataTransfer.worldSeed);




        await RenderTileColours(temperatureMap, precipitationMap, chunkTilemap, chunkPosition);
    }


    async Task RenderTileColours(float[,] temperatureMap, float[,] precipitationMap, Tilemap chunkTilemap, Vector3Int chunkPosition)
    {
        Debug.Log("RenderTileColours: Processing Started.");

        int width = Config.chunkSize;
        int height = Config.chunkSize;

        int chunkOffsetX = chunkPosition.x * Config.chunkSize;
        int chunkOffsetY = chunkPosition.y * Config.chunkSize;

        // Find a better way to do this.
        GradientTile selectedTile = await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
        {
            return BiomeUtility.GetGradientTileByName("grass");
        });

        List<Task<(Vector3Int pos, GradientTile tile)>> tileTasks = new List<Task<(Vector3Int pos, GradientTile tile)>>();

        Debug.Log("RenderTileColours: About to start the loop.");
        
        // Use a regular for loop to handle async tasks properly.
        for (int x = 0; x < width; x++)         // THE BORDER CODE LIVED HERE!!!
        {
            for (int y = 0; y < height; y++)
            {
                int localX = x;
                int localY = y;

                // Offload the task of getting neighbor colors and creating tiles to background threads.
                var tileTask = Task.Run(async () =>
                {
                    // Fetch neighbour colors without dispatcher (background thread).
                    //This can't grab from other chunks, so need to work around that.
                    Color[] neighbourColours = await Task.Run(() => GetTileNeighbourColoursAsync(temperatureMap, precipitationMap, localX, localY));
                    
                    // Perform the gradient calculation in the background.
                    Color[] colours = await Utility.Get8WayGradientAsync(neighbourColours);

                    // Create the tile on the main thread.
                    GradientTile tile = await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
                    {
                        var newTile = ScriptableObject.CreateInstance<GradientTile>();
                        Vector3Int tilePos = new Vector3Int(chunkOffsetX + localX, chunkOffsetY + localY, 0);
                        newTile.Initialize(tilePos, colours, selectedTile.mainTexture);
                        return newTile;
                    });

                    // Return tile data for setting in Tilemap.
                    return (new Vector3Int(chunkOffsetX + localX, chunkOffsetY + localY, 0), tile);
                });

                tileTasks.Add(tileTask);
            }
        }

        // Wait for all tasks to complete.
        var tiles = await Task.WhenAll(tileTasks);

        // Set tiles on the main thread.
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            foreach (var (tilePos, tile) in tiles)
            {
                chunkTilemap.SetTile(tilePos, tile);
            }
        });

        Debug.Log("RenderTileColours: Completed.");
    }

    public async Task<Color[]> GetTileNeighbourColoursAsync(float[,] temperatureMap, float[,] precipitationMap, int x, int y)
    {

        //If I generate noise maps for the chunk with a 1 tile border, then offset the x and y values by 1 inwards...
        //I can get rid of the border colour issue.

        var dispatcher = UnityMainThreadDispatcher.Instance();

        Color[] tileColours = new Color[9];
        int index = 0;

        // [0,1,2]
        // [3,4,5]
        // [6,7,8]

        // Run the tile colour fetching on the main thread
        // Iterate over neighboring tiles to collect colors
        for (int yOffset = 1; yOffset >= -1; yOffset--)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                int xCoord = x + xOffset + 1;
                int yCoord = y + yOffset + 1;

                // Ensure we don't go out of bounds
                if (xCoord >= 0 && xCoord < temperatureMap.GetLength(0) && yCoord >= 0 && yCoord < precipitationMap.GetLength(1))
                {
                    // Call GetColourFromUVMap on the main thread
                    int currentIndex = index;  // Capture the index for the closure

                    // Ensure this is finished before returning and rendering.
                    tileColours[currentIndex] = await dispatcher.EnqueueAsync(() =>
                    {
                        return uVColourMap.GetColourFromUVMap(temperatureMap[xCoord, yCoord], precipitationMap[xCoord, yCoord]);
                    });
                }
                else
                {
                    // Assign a backup color for out-of-bounds tiles
                    tileColours[index] = Color.white;
                }

                index++;
            }
        }

        return tileColours;
    }

}
