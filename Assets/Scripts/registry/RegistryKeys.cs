using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RegistryKeys
{
    //list of the categories, like biome, item, etc.

    public static Identifier ROOT = Identifier.of("root");
    public static RegistryKey<Registry<Biome>> BIOME = of<Biome>("worldgen/biome");

    private static RegistryKey<Registry<T>> of<T>(string id){
        return RegistryKey<T>.ofRegistry(Identifier.ofOriginal(id));
    }
}
