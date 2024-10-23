using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Profiling;
using Unity.Collections;
using Unity.Jobs;

public class ChunkLoader : MonoBehaviour
{
    public ChunkManager chunkManager;
    public WorldEngine worldEngine;
    public GradientTile tempTile;

    public Camera mainCamera;
    public GameObject grid;
    public Tile blankTile;
    public float noiseScale;
    public int seed;

    private int chunkSize => Config.chunkSize;
    public float padding;

    private Vector3Int playerChunkPosition;
    private Vector2 playerPosition;
    [SerializeField] private GameObject player;

    LinkedList<Vector3Int> chunkLRU = new LinkedList<Vector3Int>();
    Dictionary<Vector3Int, GameObject> activeChunks = new Dictionary<Vector3Int, GameObject>();
    int maxActiveChunks => Config.maxChunks;

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
        List<Vector3Int> softChunksToLoad = new List<Vector3Int>();

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
                chunksToLoad.Add(chunkPosition);
            }
        }

        int softLoadPadding = 1;
        int softMinX = minX - softLoadPadding;
        int softMaxX = maxX + softLoadPadding;
        int softMinY = minY - softLoadPadding;
        int softMaxY = maxY + softLoadPadding;

        for(int x = softMinX; x <= softMaxX; x++)
        {
            for(int y = softMinY; y <= softMaxY; y++)
            {
                Vector3Int chunkPosition = new Vector3Int(x, y, 0);
                if(!chunksToLoad.Contains(chunkPosition))
                {
                    softChunksToLoad.Add(chunkPosition);
                }
            }
        }

        List<Vector3Int> chunksToUnload = new List<Vector3Int>();

        foreach(var chunk in chunkManager.chunkCache.Keys)
        {
            if (!chunksToLoad.Contains(chunk) && !softChunksToLoad.Contains(chunk))
            {
                chunksToUnload.Add(chunk);
            }
        }

        // StartCoroutine(LoadChunksAroundPlayerCoroutine(chunksToLoad, softChunksToLoad, chunksToUnload));

        foreach(var chunkPosition in chunksToLoad)
        {
            if (!chunkManager.chunkCache.ContainsKey(chunkPosition))
            {
                LoadChunk(chunkPosition);  // This can still be async
            }
            else{
                chunkLRU.Remove(chunkPosition);
                chunkLRU.AddFirst(chunkPosition);
            }
        }

        foreach(var chunkPosition in softChunksToLoad){
            if(!chunkManager.chunkCache.ContainsKey(chunkPosition))
            {
                LoadChunk(chunkPosition, softLoad: true);
                chunkLRU.Remove(chunkPosition);
                chunkLRU.AddFirst(chunkPosition);
            }
        }
    }

    // Spread out chunk loading across multiple frames
    IEnumerator LoadChunksAroundPlayerCoroutine(List<Vector3Int> chunksToLoad, List<Vector3Int> softChunksToLoad, List<Vector3Int> chunksToUnload)
    {
        // Loop through the chunks, adding a yield statement to spread the workload
        foreach(var chunkPosition in chunksToLoad)
        {
            if (!chunkManager.chunkCache.ContainsKey(chunkPosition))
            {
                LoadChunk(chunkPosition);  // This can still be async
                yield return null;  // Spread the workload across frames
            }
            else{
                chunkLRU.Remove(chunkPosition);
                chunkLRU.AddFirst(chunkPosition);
            }
        }

        foreach(var chunkPosition in softChunksToLoad){
            if(!chunkManager.chunkCache.ContainsKey(chunkPosition))
            {
                LoadChunk(chunkPosition, softLoad: true);
                yield return null;
                chunkLRU.Remove(chunkPosition);
                chunkLRU.AddFirst(chunkPosition);
            }
        }
    }


    async void LoadChunk(Vector3Int chunkPosition, bool softLoad = false)
    {
        if(softLoad){
            Debug.Log("SoftLoad is currently not in use.");
            return;
        }

        GameObject chunk = chunkManager.chunkPool.GetChunk();
        // chunk.transform.parent = grid.transform;
        string softLoadString = softLoad ? "_softLoad" : "";
        chunk.name = $"Chunk_{chunkPosition}{softLoadString}";

        Tilemap chunkTilemap = chunk.GetComponent<Tilemap>();
        GameObject child = chunk.transform.Find("Modifications").gameObject;
        Tilemap chunkChildTilemap = child.GetComponent<Tilemap>();

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

        if(activeChunks.ContainsKey(chunkPosition)){
            chunkLRU.Remove(chunkPosition);
            chunkLRU.AddFirst(chunkPosition);
            return;
        }

        ChunkData modChunkData = chunkManager.LoadChunk(chunkPosition);

        if(softLoad){
            Debug.Log($"Chunk {chunkPosition} was Loaded Softly.");
            //chunkManager.AddSoftChunk(chunkPosition, chunk);
            return;
        }

        // Add the chunk to the cache.
        activeChunks.Add(chunkPosition, chunk);
        chunkManager.AddChunk(chunkPosition, chunk);

        // Render the chunk.
        RenderChunk(chunkData, modChunkData, chunkPosition, chunkTilemap, chunkChildTilemap);

        chunkLRU.AddFirst(chunkPosition);

        if(chunkLRU.Count > maxActiveChunks){
            Vector3Int chunkToEvict = chunkLRU.Last.Value;
            chunkLRU.RemoveLast();
            UnloadChunk(chunkToEvict);
        }

        Debug.Log($"Chunk at {chunkPosition} Successfully Loaded.");
    }

    void UnloadChunk(Vector3Int chunkPosition)
    {
        //rejig to unload only once a certain distance away.
        //Maybe unrender but don't have to completely unload it.

        if(!activeChunks.TryGetValue(chunkPosition, out GameObject chunkGO)){
            return;
        }

        if(chunkManager.chunkCache.TryGetValue(chunkPosition, out GameObject chunk))
        {
            chunkManager.SaveModifications();
            chunkManager.RemoveChunk(chunkPosition, chunk);
            activeChunks.Remove(chunkPosition);
            chunkManager.chunkPool.ReturnChunk(chunk);
            // Destroy(chunk);
        }
    }

    void RenderChunk(ChunkData chunkData, ChunkData modChunkData, Vector3Int chunkPosition, Tilemap chunkTilemap, Tilemap chunkChildTilemap)
    {
        // Begin profiling the entire chunk rendering process
        Debug.Log("RenderChunk: Processing Started.");

        // Extract biomeMap from the chunkData and precompute the array
        Biome[,] biomeMap = BiomeUtility.ListToArray(chunkData.biomeMapList, chunkSize, chunkSize);

        // Render the biome map onto the chunk's tilemap
        // DrawBiomeMap(biomeMap, chunkTilemap, chunkPosition);
        RenderTileColours(chunkTilemap, chunkPosition);

        // If there is modification data, apply it to the child tilemap
        if (modChunkData != null)
        {
            DrawModificationMap(modChunkData.tileDataList, chunkChildTilemap, chunkPosition);
        }

        // End profiling for the entire chunk rendering process
        Debug.Log("RenderChunk: Processing Finished.");
    }

    async void DrawModificationMap(List<TileData> tileDataList, Tilemap chunkTilemap, Vector3Int chunkPosition){
        if(tileDataList == null){
            Debug.LogError("Unable to DrawModificationMap as tileDataList was null.");
            return;
        }

        for(int i = 0; i < tileDataList.Count; i++){
            TileData tileData = tileDataList[i];

            int x = tileData.x;
            int y = tileData.y;
            
            Vector3Int tilePosition = new Vector3Int(x, y, 0);

            chunkTilemap.SetTile(tilePosition, blankTile);
        }
    }
    void DrawBiomeMap(Biome[,] biomeMap, Tilemap chunkTilemap, Vector3Int chunkPosition)
    {
        //DEPRECATED

        int width = biomeMap.GetLength(0);
        int height = biomeMap.GetLength(1);

        GradientTile selectedTile = BiomeUtility.GetGradientTileByName("grass");


        int localX = chunkPosition.x * width;
        int localY = chunkPosition.y * height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePosition = new Vector3Int(localX + x, localY + y, 0);

                if(tilePosition.x > localX + width || tilePosition.x < localX || tilePosition.y > localY + height || tilePosition.y < localY){
                    continue;
                }

                chunkTilemap.SetTile(tilePosition, selectedTile);
            }
        }

        RenderTileColours(chunkTilemap, chunkPosition);
    }

    void RenderTileColours(Tilemap chunkTilemap, Vector3Int chunkPosition)
    {

        float[,] temperatureMap = worldEngine.temperatureGenerator.GenerateChunkPerlinWithBorder(chunkPosition, WorldDataTransfer.worldSeed);
        float[,] precipitationMap = worldEngine.precipitationGenerator.GenerateChunkPerlinWithBorder(chunkPosition, WorldDataTransfer.worldSeed);

        Debug.Log("RenderTileColours: Processing Started.");

        int width = Config.chunkSize;
        int height = Config.chunkSize;

        int chunkOffsetX = chunkPosition.x * Config.chunkSize;
        int chunkOffsetY = chunkPosition.y * Config.chunkSize;

        // Find a better way to do this.
        GradientTile selectedTile = BiomeUtility.GetGradientTileByName("grass");
        // GradientTile selectedTile = tempTile;
        Sprite tileSprite = selectedTile.mainTexture;

        List<(Vector3Int pos, GradientTile tile)> tiles = new List<(Vector3Int pos, GradientTile tile)>();

        Debug.Log("RenderTileColours: About to start the loop.");
        
        // Use a regular for loop to handle async tasks properly.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Color[] neighbourColours = GetTileNeighbourColours(temperatureMap, precipitationMap, x, y, selectedTile.colourMap);
                
                Color[] colours = TextureUtility.Get8WayGradient(neighbourColours);
                
                
                GradientTile tile = ScriptableObject.CreateInstance<GradientTile>();
                
                Vector3Int tilePos = new Vector3Int(chunkOffsetX + x, chunkOffsetY + y, 0);
                // tile.csvFile = selectedTile.csvFile;
                tile.Initialize(tilePos, colours, tileSprite);

                // Return tile data for setting in Tilemap.
                tiles.Add((tilePos, tile));
            }
        }

        foreach (var (tilePos, tile) in tiles)
        {
            chunkTilemap.SetTile(tilePos, tile);
            
        }
        Debug.Log("RenderTileColours: Completed.");
    }

    private Color[] GetTileNeighbourColours(float[,] temperatureMap, float[,] precipitationMap, int x, int y, Texture2D colourMap){

        Color[] tileColours = new Color[9];
        int mapSize = temperatureMap.GetLength(0);

        int index = 0;

        for(int yOffset = 1; yOffset >= -1; yOffset--){
            for(int xOffset = -1; xOffset <= 1; xOffset++){

                int neighbourX = x + xOffset + 1;
                int neighbourY = y + yOffset + 1;

                if(neighbourX >= 0 && neighbourX < mapSize && neighbourY >= 0 && neighbourY < mapSize){

                    float temp = temperatureMap[neighbourX, neighbourY];
                    float precip = precipitationMap[neighbourX, neighbourY];

                    tileColours[index] = UVColourMap.GetColourFromUVMap(temp, precip, colourMap);
                }
                else{
                    tileColours[index] = Color.white;
                }

                index++;
            }
        }

        return tileColours;
    }
}
