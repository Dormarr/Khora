using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GlobalRegistry
{
    public static CategoryRegistry categoryRegistry = new CategoryRegistry();

    public static void Initialize(){
        //Consider order of initialization.
        InitializeRegistry<NaturalFeature>("naturalFeatures", FeatureManager.InitializeNaturalFeatures);
        InitializeRegistry<Biome>("biomes", BiomeManager.InitializeBiomes);

        Debug.Log("Initialized GlobalRegistry");
    }

    private static void InitializeRegistry<T>(string categoryKey, Action<Registry<T>> initializeAction){
        categoryRegistry.RegisterCategory<T>(categoryKey);
        Registry<T> registry = categoryRegistry.GetCategoryRegistry<T>(categoryKey);
        initializeAction(registry);
    }
}
