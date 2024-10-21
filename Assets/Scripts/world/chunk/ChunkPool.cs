using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkPool
{
    private Queue<GameObject> availableChunks = new Queue<GameObject>();
    private GameObject chunkPrefab; 
    private GameObject parent;

    public ChunkPool(GameObject chunkPrefabIn, int intialPoolSize, GameObject parentIn){
        chunkPrefab = chunkPrefabIn;
        parent = parentIn;
        for(int i = 0; i < intialPoolSize; i++){
            GameObject chunk = CreateNewChunk();
            chunk.SetActive(false);
        }
    }

    private GameObject CreateNewChunk(){
        GameObject chunk = GameObject.Instantiate(chunkPrefab);
        chunk.transform.parent = parent.transform;
        return chunk;
    }

    public GameObject GetChunk(){
        if(availableChunks.Count > 0){
            GameObject chunk = availableChunks.Dequeue();
            chunk.SetActive(true);
            return chunk;
        }
        else{
            return CreateNewChunk();
        }
    }

    public void ReturnChunk(GameObject chunk){
        chunk.SetActive(false);
        availableChunks.Enqueue(chunk);
    }
}
