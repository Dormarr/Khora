using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildBiome
{
    //need a feature list to add to and build.
    //Add the spawn stuff here too.
    public static Biome CreateBiome(){
        //funnel a feature list into this and construct as a biome.
    }

    public static Biome CreatePlains(bool snowy, bool flower, bool trees){
        
        //add grass
        
        if(snowy){
            //tundra
            if(trees){
                //snowy forest
            }
        }

        if(flower){
            if(trees){
                //lush forest - boreal?
            }else{
                //flower meadow
            }
        }

        if(trees){
            //forest
        }
    }

    public static Biome CreateSwamp(bool trees, bool wet){
        if(trees){
            if(wet){
                //swamp
            }else{
                //rainforest
            }
        }else{
            //wetlands
        }
    }

    public static Biome CreateDesert(bool cold, bool barren){
        if(cold){
            if(!barren){
                //glacial
            }
        }else{
            if(barren){
                //desert
            }else{
                //shrubland
            }
        }
    }
}
