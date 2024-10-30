using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using static Noise;

/// <summary>
/// This is essentially a complete rewrite of the world engine script.
/// I've made it static to avoid unnecessary setup in scenes and allow for cleaner global access.
/// This processes the worlds perlin values and assigns biomes to tiles in the game.
/// </summary>


public static class WorldGenerator{
    public static string worldName;
    public static int worldSeed;
    public static string worldDate;

    public static void GetWorldSeed(string input){
        if(File.Exists(Utility.GetWorldSaveDataFilePath(input))){
            WorldSaveData wsd = Utility.LoadWorldSaveData(input);
            wsd.date = Utility.GetDateTimeString();
            worldSeed = wsd.seed;
        }else{
            SaveNewWorldSaveData($"{Utility.GenerateWorldName(worldName)}", worldSeed, Utility.GetDateTimeString());
        }
    }

    public static void SaveNewWorldSaveData(string name, int seed, string date){
        worldName = name;
        worldSeed = seed;
        worldDate = date;
        WorldSaveData wsd = new WorldSaveData.Build()
            .Seed(seed)
            .Name(name)
            .Date(date)
            .BuildWorldSaveData();
        Utility.SaveWorldSaveData(wsd);
    }

    public static void UpdateWorldSaveData(string name = null, string date = null){
        string updatedName = !string.IsNullOrEmpty(name)? name : worldName;
        string updatedDate = !string.IsNullOrEmpty(date)? date : worldDate;
        
        WorldSaveData wsd = new WorldSaveData.Build()
            .Seed(worldSeed)//this should never need updating beyond initialisation.
            .Name(updatedName)
            .Date(updatedDate)
            .BuildWorldSaveData();
        Utility.SaveWorldSaveData(wsd);
    }

    public static Biome GenerateBiomeforCoordinate(Vector3Int coordinate){
        float temperature = GenerateCoordinateNoiseFromSettings(coordinate, worldSeed, WorldSettings.Temperature);
        float precipitation = GenerateCoordinateNoiseFromSettings(coordinate, worldSeed, WorldSettings.Precipitation);

        return BiomeGenerator.GetBiome(temperature, precipitation);
    }
    public static Biome[,] GenerateBiomeForChunk(Vector3Int chunkPosition){
        float[,] temperatureMap = GenerateChunkNoiseMapFromSettings(chunkPosition, worldSeed, WorldSettings.Temperature);
        float[,] precipitationMap = GenerateChunkNoiseMapFromSettings(chunkPosition, worldSeed, WorldSettings.Precipitation);
        Biome[,] biomeMap = new Biome[temperatureMap.GetLength(0), precipitationMap.GetLength(1)];

        for(int x = 0; x < temperatureMap.GetLength(0); x++){
            for(int y = 0; y < precipitationMap.GetLength(1); y++){
                biomeMap[x,y] = BiomeGenerator.GetBiome(temperatureMap[x,y], precipitationMap[x,y]);
            }
        }

        return biomeMap;
    }

    // I will do the same for topology, but I want to set it up as a defined thing like biomes.

}
public static class BiomeGenerator
{
    public static Biome GetBiome(float temperature, float precipitation)
    {
        Biome biome = BiomeClimateRegistry.GetBiome(temperature, precipitation);
        if(biome != null){
            return biome;
        }
        
        throw new Exception("Big whoops, cannot find biome in registry.");
    }
}

public static class TopologyGenerator
{
    public static float GetTopology(float elevation, float erosion, float erosionStrength)
    {
        float erodedElevation = elevation - (erosion * erosionStrength);
        return Mathf.Clamp01(erodedElevation);
    }
}