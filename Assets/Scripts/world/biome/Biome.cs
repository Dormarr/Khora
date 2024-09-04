using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This is passed into the json files and is subsequently what determines how biomes are generated.


[System.Serializable]
public class Biome
{
    public string Name {get; private set;}
    public float Temperature {get; private set;}
    public float Precipitation {get; private set;}
    //add tile with input for texture file path;

    //public SpawnSettings spawnSettings;
    //public GenerationSettings generationSettings;
    //public EffectSettings effectSettings;

    public Biome(string name, float temperature, float precipitation){
        this.Name = name;
        this.Temperature = temperature;
        this.Precipitation = precipitation;
    }

    public bool Matches(float temperature, float precipitation){
        float matchRange = 0.35f;
        return Mathf.Abs(Temperature - temperature) < matchRange && Mathf.Abs(Precipitation - precipitation) < matchRange;
    }

    public class Build
    {
        private string name;
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
                return new Biome(this.name, this.temperature, this.precipitation);
            }else{
                throw new Exception("Missing Biome parameters in builder\n" + this);
            }
        }



    }

}
