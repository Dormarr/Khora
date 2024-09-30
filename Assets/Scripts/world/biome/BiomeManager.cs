using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor;

public static class BiomeManager
{
    private static Registry<Biome> biomeRegistry;

    public static void InitializeBiomes(Registry<Biome> registry){

        //Temperature and Precipitation variables are redundant here I think.
        //Also the biomes should be built using the buildBiomes script once it's done.
        biomeRegistry = registry;

        biomeRegistry.Register("plains",            BuildBiome.CreatePlains(false, false, false));
        biomeRegistry.Register("desert",            BuildBiome.CreateDesert(false, true));
        biomeRegistry.Register("borealForest",      BuildBiome.CreatePlains(false, true, true));
        biomeRegistry.Register("forest",            BuildBiome.CreatePlains(false, false, true));
        biomeRegistry.Register("glacial",           BuildBiome.CreateDesert(true, false));
        biomeRegistry.Register("rainforest",        BuildBiome.CreateSwamp(true, false));
        biomeRegistry.Register("shrubland",         BuildBiome.CreateDesert(false, false));
        biomeRegistry.Register("snowyForest",       BuildBiome.CreatePlains(true, false, true));
        biomeRegistry.Register("swamp",             BuildBiome.CreateSwamp(true, true));
        biomeRegistry.Register("tundra",            BuildBiome.CreatePlains(true, false, false));
        biomeRegistry.Register("wetlands",          BuildBiome.CreateSwamp(false, false));
        biomeRegistry.Register("taiga",             BuildBiome.CreatePlains(true, true, true));
        //UNUSED
        biomeRegistry.Register("flowerMeadow",      BuildBiome.CreatePlains(false, true, false));
        InitializeClimateBiomes();
    }


    public static void InitializeClimateBiomes(){

        if(biomeRegistry == null){
            Debug.Log("Biome registry is not initialiszed.");
            return;
        }
        //This is where the temperature ranges are set as opposed to the initialized biomes above.
        //I'd like to mathematically calculate the proper ranges based on weightings and biome count.

        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.8f,1.0f,0.3f,0.7f), biomeRegistry.Get("plains"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.8f,1.0f,0.0f,0.3f), biomeRegistry.Get("desert"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.3f,0.5f,0.0f,0.3f), biomeRegistry.Get("borealForest"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.5f,0.8f,0.3f,0.7f), biomeRegistry.Get("forest"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.0f,0.3f,0.7f,1.0f), biomeRegistry.Get("glacial"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.8f,1.0f,0.7f,1.0f), biomeRegistry.Get("rainforest"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.5f,0.8f,0.0f,0.3f), biomeRegistry.Get("shrubland"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.0f,0.3f,0.3f,0.7f), biomeRegistry.Get("snowyForest"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.5f,0.8f,0.7f,1.0f), biomeRegistry.Get("swamp"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.0f,0.3f,0.0f,0.3f), biomeRegistry.Get("tundra"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.3f,0.5f,0.7f,1.0f), biomeRegistry.Get("wetlands"));
        BiomeClimateRegistry.RegisterBiome(new ClimateRange(0.3f,0.5f,0.3f,0.7f), biomeRegistry.Get("taiga"));
        
        Debug.Log("Successfully registered BiomeClimateRegistry()");

        Gates.biomeClimateRegistryGate = Gate.Closed;
    }
}
