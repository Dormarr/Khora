using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BiomeCreator
{
    private static Biome createBiome(string name, float temperature, float precipitation){
        return new Biome.Build().Temperature(temperature).Precipitation(precipitation).BuildBiome();
    }

    private static Biome createTransition(){
        //this will be the transition between biomes.
        //By splitting the difference of the biome settings, we can create a gradual gradient between biomes.

        return null;
    }

    private static Biome createPlains(bool flower, bool snowy){//need a way to reference features.
        
        
        //build spawn settings.
        //add features to a featureBuilder class, which returns into create biomes.


        //I'd like to avoid if statements where possible, so maybe a switch statement?
        if(snowy){
            //this would depend on the temperature? So I guess I would have to pass that in too.
        }else{

            if(flower){
            //add flower features, with specific flowers for plains.
            }else{
            //just go with grass.
            return createBiome("plains", 0.5f, 0.5f);
            }
        }
    }
}
