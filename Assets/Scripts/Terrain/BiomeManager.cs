using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum Biome
{
    Tundra,
    BorealForest,
    Glacial,
    Shrubland,
    Forest,
    Swamp,
    SandDesert,
    Grassland,
    Rainforest,
    SnowyForest
}

public class BiomeManager : MonoBehaviour
{
    public List<BiomeData> biomes;

    private Dictionary<Biome, BiomeData> biomeDictionary;

    private void Awake()
    {
        biomeDictionary = new Dictionary<Biome, BiomeData>();

        foreach (var biome in biomes)
        {
            biomeDictionary[biome.biomeName] = biome;
        }
    }

    public TileBase GetTileFromBiome(Biome biomeType)
    {
        if(biomeDictionary.TryGetValue(biomeType, out BiomeData biomeData))
        {
            return biomeData.tile;
        }
        return null;//handle cases where no tile is found.
    }

    public Color GetColourFromBiome(Biome biomeType)
    {
        if( biomeDictionary.TryGetValue(biomeType, out BiomeData biomeData))
        {
            return biomeData.biomeColour;
        }
        return Color.clear;
    }

    //use other functions to return data from within the chosen biome.
}
