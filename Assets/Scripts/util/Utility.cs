using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.Security.Cryptography;
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

    public static void SaveWorldSaveData(WorldSaveData worldSaveData){
        string filePath = GetWorldSaveDataFilePath();
        string json = JsonUtility.ToJson(worldSaveData);
        File.WriteAllText(filePath, json);
        Debug.Log($"Saved '{filePath}'");
    }

    public static WorldSaveData LoadWorldSaveData(){
        string filePath = GetWorldSaveDataFilePath();
        if(File.Exists(filePath)){
            string json = File.ReadAllText(filePath);
            Debug.Log($"Loaded chunk: {filePath}");
            return JsonUtility.FromJson<WorldSaveData>(json);
        }
        return null;
    }

    public static string GetWorldSaveDataFilePath(){
        return "Assets/saves/world/data/WorldSaveData.Json";
    }


}
