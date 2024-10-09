using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;

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

    async void LoadChunk(Vector3Int chunkPosition)
    {
        //Generation tilemap.
        GameObject chunk = new GameObject($"Chunk_{chunkPosition}");
        chunk.transform.parent = grid.transform;

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

        ChunkData chunkData = await worldEngine.GenerateChunkAsync(chunkPosition);
        ChunkData modChunkData = chunkManager.LoadChunk(chunkPosition);

        //Could be an issue where chunkData isn't finished before the biomeMap is populated.
        //Need to figure out how to wait for the data to be done before proceeding without messing up async.

        Biome[,] biomeMap = BiomeUtility.ListToArray(chunkData.biomeMapList, chunkSize, chunkSize);

        DrawBiomeMap(biomeMap, chunkTilemap, chunkPosition);
        if(modChunkData != null){
            DrawModificationMap(modChunkData.tileDataList, chunkChildTilemap, chunkPosition);
        }

        chunkManager.AddChunk(chunkPosition, chunk);
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

    void DrawModificationMap(List<TileData> tileDataList, Tilemap chunkTilemap, Vector3Int chunkPosition){
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
        int width = biomeMap.GetLength(0);
        int height = biomeMap.GetLength(1);

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){

                GradientTile selectedTile = BiomeUtility.GetGradientTileByName("grass");

                Vector3Int tilePosition = new Vector3Int(chunkPosition.x * width + x, chunkPosition.y * height + y, 0);

                chunkTilemap.SetTile(tilePosition, selectedTile);
            }
        }


        float[,] temperatureMap = worldEngine.temperatureGenerator.GenerateChunkPerlin(chunkPosition, WorldDataTransfer.worldSeed);
        float[,] precipitationMap = worldEngine.precipitationGenerator.GenerateChunkPerlin(chunkPosition, WorldDataTransfer.worldSeed);
        RenderTileColours(temperatureMap, precipitationMap, chunkTilemap, chunkPosition);
    }

    public void RenderTileColours(float[,] temperatureMap, float[,] precipitationMap, Tilemap chunkTilemap, Vector3Int chunkPosition){

        // Values set to 1 and -1 so as to leave a border of data which avoids out-of-bounds issues.
        for(int x = 1; x < temperatureMap.GetLength(0) -1; x++){
            for(int y = 1; y < temperatureMap.GetLength(1) -1; y++){
                Color[] tiles = GetTileNeighbourColours(temperatureMap, precipitationMap, x, y);

                Color[] colours = Utility.Get8WayGradient(tiles);

                //Find a better way to do this.
                GradientTile selectedTile = BiomeUtility.GetGradientTileByName("grass");

                GradientTile tile = ScriptableObject.CreateInstance<GradientTile>();
                Vector3Int tilePos = new Vector3Int(chunkPosition.x * Config.chunkSize + x, chunkPosition.y * Config.chunkSize + y, 0);
                tile.Initialize(tilePos, colours, selectedTile.mainTexture);

                chunkTilemap.SetTile(tilePos, tile);
                // chunkTilemap.RefreshAllTiles();
            }
        }
    }

    public Color[] GetTileNeighbourColours(float[,] temperatureMap, float[,] precipitationMap, int x, int y){

        Color[] tileColours = new Color[9];
        
        int index = 0;

        // [0,1,2]
        // [3,4,5]
        // [6,7,8]

        //Gets the colours in rows from bottom left.
        for(int yOffset = 1; yOffset >= -1; yOffset--){
            for(int xOffset = -1; xOffset <= 1; xOffset++){
                int xCoord = x + xOffset;
                int yCoord = y + yOffset;

                // Safety check to ensure we don't access out-of-bounds
                if (xCoord >= 0 && xCoord < temperatureMap.GetLength(0) && yCoord >= 0 && yCoord < precipitationMap.GetLength(1))
                {
                    // Get the colour based on the temperature and precipitation map values at this tile
                    tileColours[index] = uVColourMap.GetColourFromUVMap(temperatureMap[xCoord, yCoord], precipitationMap[xCoord, yCoord]);
                }
                else
                {
                    // Backup colour.
                    tileColours[index] = Color.white;
                }

                index++;
            }
        }

        return tileColours;
    }

}
