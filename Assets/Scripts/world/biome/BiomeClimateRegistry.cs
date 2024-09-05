using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeClimateRegistry
{
    private static Dictionary<ClimateRange, Biome> biomeDictionary = new Dictionary<ClimateRange, Biome>();

    public static void RegisterBiome(ClimateRange range, Biome biome){
        biomeDictionary[range] = biome;
    }

    public static Biome GetBiome(float temperature, float precipitation){
        foreach(var entry in biomeDictionary){
            if(entry.Key.Contains(temperature, precipitation)){
                return entry.Value;
            }
        }
        //throw new Exception("No matching biome found.");
        Debug.Log("No matching biome found in BiomeRegistry.");
        return null;
    }
}
