using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistryManager
{
    public static readonly Registry<Biome> BiomeRegistry = new Registry<Biome>();

    public static void RegisterBiome(string id, Biome biome){
        BiomeRegistry.Register(id, biome);
    }

    public static Biome GetBiome(string id){
        return BiomeRegistry.Get(id);
    }
}
