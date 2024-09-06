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
        biomeRegistry = registry;
        //biomeRegistry.Register("plains",         new Biome("plains", 0.5f, 0.5f));
        biomeRegistry.Register("plains",            new Biome.Build().Name("plains").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("desert",            new Biome.Build().Name("desert").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("borealForest",      new Biome.Build().Name("borealForest").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("forest",            new Biome.Build().Name("forest").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("glacial",           new Biome.Build().Name("glacial").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("rainforest",        new Biome.Build().Name("rainforest").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("shrubland",         new Biome.Build().Name("shrubland").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("snowyForest",       new Biome.Build().Name("snowyForest").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("swamp",             new Biome.Build().Name("swamp").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("tundra",            new Biome.Build().Name("tundra").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("wetlands",          new Biome.Build().Name("wetlands").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
        biomeRegistry.Register("taiga",             new Biome.Build().Name("taiga").FeatureSettings(new FeatureSettings.Build().Grass(0.6f).Trees(0.1f, 0.6f).Flowers(0.05f, 0.4f).BuildFeatureSettings()).BuildBiome());
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
    }

}
