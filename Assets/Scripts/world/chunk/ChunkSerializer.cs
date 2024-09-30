using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public static class ChunkSerializer
{
    public static void SaveChunk(ChunkData chunkData, string filePath){
        string json = JsonUtility.ToJson(chunkData);
        File.WriteAllText(filePath, json);
    }

    public static ChunkData LoadChunk(string filePath){
        if(File.Exists(filePath)){
            string json = File.ReadAllText(filePath);
            //Debug.Log($"Loaded chunk: {filePath}");
            return JsonUtility.FromJson<ChunkData>(json);
        }
        return null;
    }

    public static string GetChunkFilePath(Vector3Int chunkPosition){
        //return $"Assets/saves/world/chunks/chunk_{chunkPosition.x}_{chunkPosition.y}.json";
        string directory = $"Assets/saves/{WorldDataTransfer.worldName}/chunks/";
        if(!Directory.Exists(directory)){
            Directory.CreateDirectory(directory);
        }
        return $"{directory}/chunk_{chunkPosition.x}_{chunkPosition.y}.json";
    }
}

