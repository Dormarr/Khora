using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GlobalRegistry
{
    public static CategoryRegistry categoryRegistry = new CategoryRegistry();

    public static void Bootstrap(){

        switch(Gates.registryGate){
            case Gate.Open:
                Initialize();
                Gates.registryGate = Gate.Closed;
                break;
            case Gate.Ajar:
                Bootstrap();
                break;
            case Gate.Closed:
                return;
        }
    }

    public static void Initialize(){
        //Consider order of initialization.
        Gates.registryGate = Gate.Ajar;

        InitializeRegistry<NaturalFeature>("naturalFeatures", FeatureManager.InitializeNaturalFeatures);
        InitializeRegistry<Biome>("biomes", BiomeManager.InitializeBiomes);
        InitializeRegistry<int[]>("textures", TextureManager.InitializeTextureInts);


        Debug.Log("Initialized GlobalRegistry");
    }

    private static void InitializeRegistry<T>(string categoryKey, Action<Registry<T>> initializeAction){
        categoryRegistry.RegisterCategory<T>(categoryKey);
        Registry<T> registry = categoryRegistry.GetCategoryRegistry<T>(categoryKey);
        initializeAction(registry);
    }
}
