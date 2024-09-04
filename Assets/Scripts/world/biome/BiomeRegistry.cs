using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeRegistry : Registry<Biome>
{
    public static RegistryKey<Biome> PLAINS = register("plains");

    private static RegistryKey<Biome> register(string name){
        Debug.Log($"Registered {name}");
        return RegistryKey<Biome>.of(RegistryKeys.BIOME, Identifier.ofOriginal(name));
    }
}