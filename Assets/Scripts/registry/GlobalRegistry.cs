using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalRegistry
{
    public static CategoryRegistry categoryRegistry = new CategoryRegistry();

    public static void Initialize(){

        categoryRegistry.RegisterCategory<Feature>("naturalFeatures");
        Registry<Feature> featureRegistry = categoryRegistry.GetCategoryRegistry<Feature>("naturalFeatures");
        //Need to inisitalize the features prior to biomes.

        categoryRegistry.RegisterCategory<Biome>("biomes");
        Registry<Biome> biomeRegistry = categoryRegistry.GetCategoryRegistry<Biome>("biomes");
        BiomeManager.InitializeBiomes(biomeRegistry);

        Debug.Log("Initialized GlobalRegistry");
    }
}
