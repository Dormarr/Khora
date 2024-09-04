using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BiomeEditorWindow : EditorWindow
{
    private Biome newBiome;

    [MenuItem("Tools/Biome Editor")]
    public static void ShowWindow(){
        GetWindow<BiomeEditorWindow>("Biome Editor");
    }

    private void OnGUI(){
        GUILayout.Label("Create New Biome", EditorStyles.boldLabel);
        newBiome.biomeName = EditorGUILayout.TextField("Biome Name", newBiome.biomeName);
        newBiome.temperature = EditorGUILayout.FloatField("Temperature", newBiome.temperature);
        newBiome.precipitation = EditorGUILayout.FloatField("Precipitation", newBiome.precipitation);

        if(GUILayout.Button("Save Biome")){
            SaveBiome(newBiome);
        }
    }

    private static void SaveBiome(Biome newBiome){
        string directoryPath = "Assets/data/worldgen/biome/";
        if(!Directory.Exists(directoryPath)){
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, newBiome.biomeName + ".json");
        string json = JsonUtility.ToJson(newBiome, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Biome saved to " + filePath + " || From BiomeCreator");
    }

    //might need a method to cycle through the jsons and regenerate them based on new variables if/when I expand.
}
