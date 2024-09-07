using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildBiome
{
    //need a feature list to add to and build.
    //Add the spawn stuff here too.

    //Need to consider the prioritisation of generation order. I.E. trees before grass etc.

    public static Biome CreateBiome(string name,
        FeatureSettings.Build featureSettings
        //SpawnSettings spawnSettings
    ){
        //funnel a feature list into this and construct as a biome.

        return new Biome.Build()
        .Name(name)
        .FeatureSettings(featureSettings.BuildFeatureSettings()).BuildBiome();
    }

    public static Biome CreatePlains(bool snowy, bool flower, bool trees){
        

        FeatureSettings.Build featureSettingsBuild = new FeatureSettings.Build();
        
        if(snowy){
            if(trees){
                if(flower){
                    //taiga
                    //coldish, pine trees, spattering of mushrooms and that's about it.
                    featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("pine_tree"));
                    featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("mushroom_tree"));
                    return CreateBiome("taiga", featureSettingsBuild);
                }
                //snowy forest
                return CreateBiome("snowyForest", featureSettingsBuild);
            }
            //tundra
            return CreateBiome("tundra", featureSettingsBuild);
        }

        if(flower){
            if(trees){
                //lush forest - boreal?
                featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("mushroom_tree"));
                featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("birch_tree"));
                featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("daisy"));
                return CreateBiome("borealForest", featureSettingsBuild);
            }
            //flower meadow
            featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("daisy"));
            return CreateBiome("flowerMeadow", featureSettingsBuild);
        }

        if(trees){
            //forest
            featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("oak_tree"));
            featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("birch_tree"));
            return CreateBiome("forest", featureSettingsBuild);
        }

        //true plains
        featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("grass"));
        return CreateBiome("plains", featureSettingsBuild);
    }

    public static Biome CreateSwamp(bool trees, bool wet){

        FeatureSettings.Build featureSettingsBuild = new FeatureSettings.Build();

        if(trees){
            if(wet){
                //swamp
                featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("willow_tree"));
                return CreateBiome("swamp", featureSettingsBuild);
            }
            //rainforest
            featureSettingsBuild.AddNaturalFeature(NaturalFeatureLookup("rubber_tree"));
            return CreateBiome("rainforest", featureSettingsBuild);
        }
        //wetlands
        return CreateBiome("wetlands", featureSettingsBuild);
    }

    public static Biome CreateDesert(bool cold, bool barren){

        FeatureSettings.Build featureSettingsBuild = new FeatureSettings.Build();

        if(cold){
            if(!barren){
                //glacial
                return CreateBiome("glacial", featureSettingsBuild);
            }
        }
        if(!barren){
            //shrubland
            return CreateBiome("shrubland", featureSettingsBuild);
        }
        
        //desert
        return CreateBiome("desert", featureSettingsBuild);

    }

    private static NaturalFeature NaturalFeatureLookup(string feature){
        return FeatureManager.GetNaturalFeature(feature);
    }
}
