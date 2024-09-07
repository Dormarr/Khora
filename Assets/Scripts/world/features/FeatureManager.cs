using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FeatureManager
{
    private static Registry<NaturalFeature> naturalFeatureRegistry;

    public static void InitializeNaturalFeatures(Registry<NaturalFeature> registry){

        naturalFeatureRegistry = registry;
        //I need a way to adjust the frequency from other scripts.
        naturalFeatureRegistry.Register("oak_tree", new NaturalFeature.Build().Name("oak_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.3f).BuildNaturalFeature());
        naturalFeatureRegistry.Register("pine_tree", new NaturalFeature.Build().Name("pine_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.3f).BuildNaturalFeature());
        naturalFeatureRegistry.Register("birch_tree", new NaturalFeature.Build().Name("birch_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.3f).BuildNaturalFeature());
        naturalFeatureRegistry.Register("willow_tree", new NaturalFeature.Build().Name("willow_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.1f).BuildNaturalFeature());
        naturalFeatureRegistry.Register("mushroom_tree", new NaturalFeature.Build().Name("mushroom_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.02f).BuildNaturalFeature());
        
        naturalFeatureRegistry.Register("rubber_tree", new NaturalFeature.Build().Name("rubber_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.3f).BuildNaturalFeature());
        naturalFeatureRegistry.Register("rowan_tree", new NaturalFeature.Build().Name("rowan_tree").Type(NaturalFeatureEnum.TREE).Strength(0.8f).Frequency(0.3f).BuildNaturalFeature());
        
        naturalFeatureRegistry.Register("grass", new NaturalFeature.Build().Name("grass").Type(NaturalFeatureEnum.GRASS).Strength(0.1f).Frequency(0.6f).BuildNaturalFeature());
        naturalFeatureRegistry.Register("daisy", new NaturalFeature.Build().Name("daisy").Type(NaturalFeatureEnum.FLOWER).Strength(0.1f).Frequency(0.15f).BuildNaturalFeature());
    }

    public static NaturalFeature GetNaturalFeature(string id){
        //do I even need this? I'm just making sure, I'd like to streamline the process.
        return naturalFeatureRegistry.Get(id);
    }

}
