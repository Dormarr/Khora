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

    public static Color[] LerpBetweenColours(Color colourA, Color colourB, int steps){
        //return an array the size of steps, consisting of colours equal between colourA and colourB.

        return null;
    }

    public static Color[] Get4WayGradient(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight){
        Color[] colours = new Color[16];

        for(int i = 0; i < 4; i++){
            float xLerp = i / 3.0f;
            Color leftLerp = Color.Lerp(topLeft, bottomLeft, xLerp);
            Color rightLerp = Color.Lerp(topRight, bottomRight, xLerp);

            for(int j = 0; j < 4; j++){
                float yLerp = j / 3.0f;
                colours[i * 4 + j] = Color.Lerp(leftLerp, rightLerp, yLerp);
            }
        }

        return colours;
    }

    public static Color[] Get8WayGradient(Color[] neighbourColors)
    {
        // Color array structure
        // [ 0, 1, 2 ]
        // [ 3, 4, 5 ]
        // [ 6, 7, 8 ]
        
        Color topLeft = neighbourColors[0];
        Color top = neighbourColors[1];
        Color topRight = neighbourColors[2];
        Color left = neighbourColors[3];
        Color center = neighbourColors[4]; // The tile's own color
        Color right = neighbourColors[5];
        Color bottomLeft = neighbourColors[6];
        Color bottom = neighbourColors[7];
        Color bottomRight = neighbourColors[8];

        // We will create a 4x4 grid to smoothly blend colors across the tile
        Color[] gradientColours = new Color[16];


        // [ 0,  1,  2,  3, ]
        // [ 4,  5,  6,  7, ]
        // [ 8,  9,  10, 11,]
        // [ 12, 13, 14, 15 ]

        // Corners
        gradientColours[0]  =   Color.Lerp(top, left, 0.5f);
        gradientColours[3]  =   Color.Lerp(top, right, 0.5f);
        gradientColours[15] =   Color.Lerp(bottom, right, 0.5f);
        gradientColours[12] =   Color.Lerp(bottom, left, 0.5f);

        //Edges
        gradientColours[1]  =   Color.Lerp(center, top, 0.4f);
        gradientColours[2]  =   Color.Lerp(center, top, 0.4f);
        gradientColours[4]  =   Color.Lerp(center, left, 0.4f);
        gradientColours[8]  =   Color.Lerp(center, left, 0.4f);
        gradientColours[7]  =   Color.Lerp(center, right, 0.6f);
        gradientColours[11] =   Color.Lerp(center, right, 0.6f);
        gradientColours[13] =   Color.Lerp(center, bottom, 0.6f);
        gradientColours[14] =   Color.Lerp(center, bottom, 0.6f);
        
        //Center
        gradientColours[5]  =   ApplyNoiseToColour(Color.Lerp(center, gradientColours[1], 0.5f), 0.025f);
        gradientColours[10] =   ApplyNoiseToColour(Color.Lerp(center, gradientColours[14], 0.5f), 0.025f);
        gradientColours[9]  =   ApplyNoiseToColour(center, 0.02f);
        gradientColours[6]  =   ApplyNoiseToColour(center, 0.02f);

        return gradientColours;
    }

    public static Color ApplyRandomHueShift(Color colour){
        float hue, saturation, brightness;
        Color.RGBToHSV(colour, out hue, out saturation, out brightness);

        // Id'd rather this wasn't random, but it's inconsequential so whatever. I'll revisit it.
        hue += UnityEngine.Random.Range(-0.02f, 0.02f);
        hue = Mathf.Repeat(hue, 1.0f);

        return Color.HSVToRGB(hue, saturation, brightness);
    }

    public static Color ApplyNoiseToColour(Color color, float noiseIntensity)
    {
        // Id'd rather this wasn't random, but it's inconsequential so whatever. I'll revisit it.
        float noise = UnityEngine.Random.Range(-1.0f, 1.0f) * noiseIntensity;

        // Convert the colour to HSV to modify brightness (Value)
        float hue, saturation, brightness;
        Color.RGBToHSV(color, out hue, out saturation, out brightness);

        // Apply noise to brightness
        brightness += noise;
        brightness = Mathf.Clamp(brightness, 0.0f, 1.0f); // Ensure brightness is within valid range

        // Return the colour with applied noise
        return Color.HSVToRGB(hue, saturation, brightness);
    }
}
