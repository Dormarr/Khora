using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This is passed into the json files and is subsequently what determines how biomes are generated.


[System.Serializable]
public class Biome
{
    public string biomeName;//ensure consistency across the board.
    public float temperature;
    public float precipitation;

    //public SpawnSettings spawnSettings;
    //public GenerationSettings generationSettings;
    //public EffectSettings effectSettings;

    Biome(float temperature, float precipitation){
        this.temperature = temperature;
        this.precipitation = precipitation;
    }

    public class Build
    {
        //something
        private float temperature;
        private float precipitation; //use as chance of rain.
        //private float downfall;
        //Add some other stuff for weather and summise as cohesive weather settings.
        //private SpawnSettings spawnSettings;
        //private GenerationSettings generationSettings; //add features to this.
        //private EffectSettings effectSettings; //to make adjustments to fundamental tile data after biome generation.

        public Biome.Build Temperature(float temperature){
            this.temperature = temperature;
            return this;
        }

        public Biome.Build Precipitation(float precipitation){
            this.precipitation = precipitation;
            return this;
        }

        // public Biome.Build SpawnSettings(SpawnSettings spawnSettings){
        //     this.spawnSettings = spawnSettings;
        //     return this;
        // }

        public Biome BuildBiome(){
            if(this.temperature != null && this.precipitation != null){
                return new Biome(this.temperature, this.precipitation);
            }else{
                throw new Exception("Missing Biome parameters in builder\n" + this);
            }
        }



    }

}
