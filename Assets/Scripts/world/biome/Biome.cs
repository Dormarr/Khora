using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This is passed into the json files and is subsequently what determines how biomes are generated.


[System.Serializable]
public class Biome
{
    public string Name {get; private set;}

    public FeatureSettings FeatureSettings {get; private set;}

    //public SpawnSettings spawnSettings;
    //public GenerationSettings generationSettings; //this should include species of tree.
    //public EffectSettings effectSettings;

    public Biome(string name, FeatureSettings featureSettings){
        this.Name = name;
        this.FeatureSettings = featureSettings;
    }

    public class Build
    {
        private string name;
        private FeatureSettings featureSettings;
        //private float downfall;
        //Add some other stuff for weather and summise as cohesive weather settings.
        //private SpawnSettings spawnSettings;
        //private GenerationSettings generationSettings; //add features to this.
        //private EffectSettings effectSettings; //to make adjustments to fundamental tile data after biome generation.

        public Biome.Build Name(string name){
            this.name = name;
            return this;
        }

        public Biome.Build FeatureSettings(FeatureSettings featureSettings){
            this.featureSettings = featureSettings;
            return this;
        }

        // public Biome.Build SpawnSettings(SpawnSettings spawnSettings){
        //     this.spawnSettings = spawnSettings;
        //     return this;
        // }

        public Biome BuildBiome(){
            if(this.name != null &&  this.featureSettings != null){
                return new Biome(this.name, this.featureSettings);
            }else{
                throw new Exception("Missing Biome parameters in builder\n" + this);
            }
        }



    }

}
