using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BiomeSearcher
{
    private readonly CategoryRegistry _categoryRegistry;

    public BiomeSearcher(CategoryRegistry categoryRegistry){
        _categoryRegistry = categoryRegistry;
    }

    public Biome SearchBiome(string categoryKey, float temperature, float precipitation){
        var biomeRegistry = _categoryRegistry.GetCategoryRegistry<Biome>(categoryKey);


        //this method SUCKS.
        foreach(var biome in biomeRegistry.GetAllValues()){
            if(biome.Matches(temperature, precipitation)){
                return biome;
            }
        }

        throw new Exception($"No matching biome found.");
    }
}
