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

        biomeRegistry = registry;
        biomeRegistry.Register("plains",        new Biome("plains", 0.5f, 0.5f));
        biomeRegistry.Register("desert",        new Biome("desert", 1.0f, 0.1f));
        biomeRegistry.Register("borealForest",  new Biome("borealForest", 0.2f, 0.1f));
        biomeRegistry.Register("forest",        new Biome("forest", 0.65f, 0.4f));
        biomeRegistry.Register("glacial",       new Biome("glacial", 0.1f, 0.8f));
        biomeRegistry.Register("rainforest",    new Biome("rainforest", 0.9f, 0.9f));
        biomeRegistry.Register("shrubland",     new Biome("shrubland", 0.7f, 0.1f));
        biomeRegistry.Register("snowyForest",   new Biome("snowyForest", 0.1f, 0.4f));
        biomeRegistry.Register("swamp",         new Biome("swamp", 0.65f, 0.8f));
        biomeRegistry.Register("tundra",        new Biome("tundra", 0.1f, 0.1f));
        biomeRegistry.Register("wetlands",      new Biome("wetlands", 0.3f, 0.8f));
        biomeRegistry.Register("taiga",         new Biome("taiga", 0.3f, 0.5f));
        InitializeClimateBiomes();
    }


    public static void InitializeClimateBiomes(){

        if(biomeRegistry == null){
            Debug.Log("Biome registry is not initialiszed.");
            return;
        }
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
    }

}
