using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor;

public class BiomeManager
{
    private Registry<Biome> biomeRegistry;
    public BiomeClimateRegistry biomeClimateRegistry;

    public void InitializeBiomes(Registry<Biome> registry){

        biomeRegistry = registry;
        biomeRegistry.Register("plains", new Biome("Plains", 0.5f, 0.5f));
        biomeRegistry.Register("desert", new Biome("Desert", 1.0f, 0.1f));
        biomeRegistry.Register("borealForest", new Biome("BorealForest", 0.2f, 0.1f));
        biomeRegistry.Register("forest", new Biome("Forest", 0.65f, 0.4f));
        biomeRegistry.Register("glacial", new Biome("Glacial", 0.1f, 0.8f));
        biomeRegistry.Register("rainforest", new Biome("Rainforest", 0.9f, 0.9f));
        biomeRegistry.Register("shrubland", new Biome("Shrubland", 0.7f, 0.1f));
        biomeRegistry.Register("snowyForest", new Biome("SnowyForest", 0.1f, 0.4f));
        biomeRegistry.Register("swamp", new Biome("Swamp", 0.65f, 0.8f));
        biomeRegistry.Register("tundra", new Biome("Tundra", 0.1f, 0.1f));
        InitializeClimateBiomes();
    }


    public void InitializeClimateBiomes(){
        biomeClimateRegistry = new BiomeClimateRegistry();

        //I'd like to mathematically calculate the proper ranges based on weightings and biome count.
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.4f,0.7f,0.4f,0.7f), biomeRegistry.Get("plains"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.8f,1.0f,0.0f,0.1f), biomeRegistry.Get("desert"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.3f,0.5f,0.0f,0.3f), biomeRegistry.Get("borealForest"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.6f,0.8f,0.3f,0.6f), biomeRegistry.Get("forest"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.0f,0.1f,0.8f,1.0f), biomeRegistry.Get("glacial"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.8f,1.0f,0.8f,1.0f), biomeRegistry.Get("rainforest"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.6f,0.8f,0.0f,0.2f), biomeRegistry.Get("shrubland"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.0f,0.2f,0.2f,0.4f), biomeRegistry.Get("snowyForest"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.5f,0.7f,0.7f,0.8f), biomeRegistry.Get("swamp"));
        biomeClimateRegistry.RegisterBiome(new ClimateRange(0.0f,0.2f,0.0f,0.2f), biomeRegistry.Get("tundra"));
    }

}
