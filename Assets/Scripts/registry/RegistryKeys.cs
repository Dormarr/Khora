using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class RegistryKeys
{
    //list of the categories, like biome, item, etc.

    public static Identifier ROOT = Identifier.ofOriginal("root");
    public static RegistryKey<Registry<Biome>> BIOME = of<Biome>("worldgen/biome");
    //public static RegistryKey<Registry<Feature>> FEATURE = of<Feature>("worldgen/feature");//for ecodeco
    //public static RegistryKey<Registry<LootTable>> LOOT_TABLE = of<LootTable>("loot_table");

    private static RegistryKey<Registry<T>> of<T>(string id){
        return RegistryKey<T>.ofRegistry(Identifier.ofOriginal(id));
    }
}
