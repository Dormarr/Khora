using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private ChunkLoader chunkLoader;

    public Dictionary<Vector3Int, GameObject> chunkCache = new Dictionary<Vector3Int, GameObject>();

    public List<TileData> modificationCache;

    void Awake(){
        //redo to get rid of awake entirely.
        chunkCache.Clear();
        modificationCache.Clear();

        GlobalRegistry.Bootstrap();
    }

    void Start(){
        chunkLoader.Initialize();
    }

    public void AddChunk(Vector3Int chunkPosition, GameObject chunk)
    {
        if(!chunkCache.ContainsKey(chunkPosition)){
            chunkCache[chunkPosition] = chunk;
        }
    }

    public void RemoveChunk(Vector3Int chunkPosition, GameObject chunk)
    {
        if(chunkCache.ContainsKey(chunkPosition)){
            chunkCache.Remove(chunkPosition);
        }
    }

    public TileBase IdentifyTile(Vector3 position)
    {
        int chunkPositionX = Mathf.FloorToInt((float)position.x / Config.chunkSize);
        int chunkPositionY = Mathf.FloorToInt((float)position.y / Config.chunkSize);
        Vector3Int chunkPosition = new Vector3Int(chunkPositionX, chunkPositionY, 0);

        if (chunkCache.ContainsKey(chunkPosition)){
            GameObject chunk = chunkCache[chunkPosition];
            Tilemap childChunkTilemap = chunk.GetComponentInChildren<Tilemap>();
            Vector3Int tilePos = childChunkTilemap.WorldToCell(position);
            TileBase tile = childChunkTilemap.GetTile(tilePos);
            if(tile == null){
                Tilemap chunkTilemap = chunk.GetComponent<Tilemap>();
                tilePos = chunkTilemap.WorldToCell(position);
                tile = chunkTilemap.GetTile(tilePos);
            }
            return tile;
        }

        return null;
    }

    public GameObject GetChunkGameObject()
    {
        Vector3Int chunkPosition = BiomeUtility.GetVariableChunkPosition(Utility.GetMouseWorldPosition());
        if (chunkCache.ContainsKey(chunkPosition)){
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

        //load and add data on top, save as the same file, overwrite with changes.

        ChunkSerializer.SaveChunk(chunkData, filePath);
    }

    public void SaveModifications(){
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
