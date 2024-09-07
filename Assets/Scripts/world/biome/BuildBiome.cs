using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildBiome
{
    //need a feature list to add to and build.
    //Add the spawn stuff here too.
    public static Biome CreateBiome(
        FeatureSettings featureSettings
        //SpawnSettings spawnSettings
    ){
        //funnel a feature list into this and construct as a biome.

        return new Biome.Build()
        .FeatureSettings(featureSettings).BuildBiome();
    }

    public static Biome CreatePlains(bool snowy, bool flower, bool trees){
        
        //add grass
        
        if(snowy){
            if(trees){
                //snowy forest
                return null;
            }
            //tundra
            return null;
        }

        if(flower){
            if(trees){
                //lush forest - boreal?
                return null;
            }
            //flower meadow
            return null;
        }

        if(trees){
            //forest
            return null;
        }

        return null;//handle nothing.
    }

    public static Biome CreateSwamp(bool trees, bool wet){
        if(trees){
            if(wet){
                //swamp
                return null;
            }
            //rainforest
            return null;
        }
        //wetlands
        return null;
    }

    public static Biome CreateDesert(bool cold, bool barren){
        if(cold){
            if(!barren){
                //glacial
                return null;
            }
        }
        if(barren){
            //desert
            return null;
        }
        
        return null;//handle nothing

    }
}
