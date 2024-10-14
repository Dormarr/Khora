using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor;

public static class Utility
{
    //I'll separate these into more appropriate scripts at some other point.
    public static InputAction mousePosition;

    //Get raw mouse input.
    public static Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        return mousePosition;
    }

    //Get mouse position in relation to the tilemap/world.
    public static Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        return mouseWorldPos;
    }

    public static Vector3Int GetChunkPosition(Vector3 position){
        int chunkPositionX = Mathf.FloorToInt((float)position.x / Config.chunkSize);
        int chunkPositionY = Mathf.FloorToInt((float)position.y / Config.chunkSize);
        Vector3Int chunkPosition = new Vector3Int(chunkPositionX, chunkPositionY, 0);
        return chunkPosition;
    }

    public static void SaveWorldSaveData(WorldSaveData worldSaveData){
        string filePath = GetWorldSaveDataFilePath(worldSaveData.name);


        string directory = $"Assets/saves/{worldSaveData.name}/data";
        if(!Directory.Exists(filePath)){
            Directory.CreateDirectory(directory);
            Debug.Log($"Created World Save File Path: {filePath}");
        }

        string json = JsonUtility.ToJson(worldSaveData);
        File.WriteAllText(filePath, json);
        Debug.Log($"Saved '{filePath}'");

        //Need to rejig along side the menu scripts to ensure loading and saving works with naming for multiple worlds.
    }

    public static WorldSaveData LoadWorldSaveData(string fileName){
        string filePath = GetWorldSaveDataFilePath(fileName);
        if(File.Exists(filePath)){
            string json = File.ReadAllText(filePath);
            Debug.Log($"Loaded chunk: {filePath}");
            return JsonUtility.FromJson<WorldSaveData>(json);
        }
        return null;
    }

    public static string GetWorldSaveDataFilePath(string fileName){
        return $"Assets/saves/{fileName}/data/{fileName}.json";
    }

    public static WorldSaveData[] GetWorldDataFiles(){
        string[] worldDirectories = Directory.GetDirectories("Assets/saves/");

        List<WorldSaveData> worldSaveDataList = new List<WorldSaveData>();

        foreach(string worldDir in worldDirectories){
            string dataFolder = Path.Combine(worldDir, "data");

            string worldName = Path.GetFileName(worldDir);
            string worldSaveFilePath = Path.Combine(dataFolder, $"{worldName}.json");

            //string worldSaveFilePath = $"Assets/saves/{worldName}/data/{worldName}.json";

            if(File.Exists(worldSaveFilePath)){
                WorldSaveData wsd = LoadWorldSaveData(worldName);
                if(wsd != null){
                    worldSaveDataList.Add(wsd);
                }
                else{
                    Debug.LogError($"Failed to load WorldSaveData from {worldSaveFilePath}");
                }
            }
            else{
                Debug.LogWarning($"World save file not found: {worldSaveFilePath}");
            }
        }

        return worldSaveDataList.ToArray();
    }

    public static string GetDateTimeString(){
        return System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
