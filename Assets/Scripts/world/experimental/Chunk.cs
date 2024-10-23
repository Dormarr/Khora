using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk{
    private GameObject chunkGameObject;
    private Tilemap tilemap;

    public Tilemap Tilemap => tilemap;
    public Vector3Int Position { get; private set; }
    public GameObject ChunkGameObject { get; private set; }
    public ChunkStatus Status { get; private set; }

    public Chunk(Vector3Int position){
        Position = position;
        chunkGameObject = new GameObject($"Chunk_{position.x}_{position.y}");
        ChunkGameObject = chunkGameObject;

        // Set parent to grid.
        Transform parent = GameObject.Find("Grid").transform;

        if(parent != null){
            chunkGameObject.transform.parent = parent;
        }else{
            Debug.LogWarning("Chunk: Parent transform is null.");
        }

        tilemap = chunkGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = chunkGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingOrder = 1;
        tilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0);
    }

    public void SetTile(Vector3Int position, TileBase tile){
        tilemap.SetTile(position, tile);
    }

    public void Clear(){
        tilemap.ClearAllTiles();
    }

    public void Destroy(){
        if(ChunkGameObject != null){
            GameObject.Destroy(ChunkGameObject);
        }
    }
}