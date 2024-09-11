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

    public List<TileData> modificationCache;

    void Awake(){
        GlobalRegistry.Initialize();
        chunkCache.Clear();
        modificationCache.Clear();
    }

    void Start()
    {
        InitializeChunks();
    }

    void InitializeChunks()
    {
        //need to grab the seed from the world data file.

        chunkLoader.Init();
    }

    public void AddChunk(Vector3Int chunkPosition, GameObject chunk)
    {
        if(!chunkCache.ContainsKey(chunkPosition))
        {
            chunkCache[chunkPosition] = chunk;
        }
    }

    public void RemoveChunk(Vector3Int chunkPosition, GameObject chunk)
    {
        if(chunkCache.ContainsKey(chunkPosition))
        {
            chunkCache.Remove(chunkPosition);
        }
    }

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
        Vector3Int chunkPosition = BiomeUtility.GetVariableChunkPosition(Utility.GetMouseWorldPosition());

        if (chunkCache.ContainsKey(chunkPosition))
        {
            return chunkCache[chunkPosition];
        }

        UnityEngine.Debug.Log("Oopsie, No Chunk Position Found!");
        return null;

    }

    public ChunkData LoadChunk(Vector3Int chunkPosition){
        string filePath = ChunkSerializer.GetChunkFilePath(chunkPosition);        
        return ChunkSerializer.LoadChunk(filePath);
    }

    public void SaveChunk(Vector3Int chunkPosition, ChunkData chunkData){
        string filePath = ChunkSerializer.GetChunkFilePath(chunkPosition);
        //Debug.Log("Saved Chunk: " + chunkPosition);
        ChunkSerializer.SaveChunk(chunkData, filePath);
    }

    public void SaveModifications(){
        //gather chunk data and chunk position, then save chunk.

        Dictionary<Vector3Int, List<TileData>> chunkedModifications = new Dictionary<Vector3Int, List<TileData>>();

        foreach(TileData tile in modificationCache){
            Vector3Int chunkPosition = BiomeUtility.GetVariableChunkPosition(new Vector2(tile.x, tile.y));

            if(chunkedModifications.ContainsKey(chunkPosition)){
                chunkedModifications[chunkPosition].Add(tile);
            }else{
                chunkedModifications[chunkPosition] = new List<TileData> {tile};
            }
        }

        foreach(var entry in chunkedModifications){
            Vector3Int chunkPosition = entry.Key;
            List<TileData> tileData = entry.Value;

            ChunkData chunkData = new ChunkData.Build()
                .Name("chunkData")
                .ChunkPosition(chunkPosition)
                .TileDataList(tileData)
                .BuildChunkData();

            SaveChunk(chunkPosition, chunkData);
        }

    }
}
