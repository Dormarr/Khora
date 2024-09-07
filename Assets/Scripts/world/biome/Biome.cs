using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This is passed into the json files and is subsequently what determines how biomes are generated.


[System.Serializable]
public class Biome
{
    public string Name {get; private set;}
    public bool Occupied {get;private set;}

    public FeatureSettings FeatureSettings {get; private set;}

    //public SpawnSettings spawnSettings;
    //public GenerationSettings generationSettings; //this should include species of tree.
    //public EffectSettings effectSettings;

    public Biome(string name, FeatureSettings featureSettings, bool occupied){
        this.Name = name;
        this.FeatureSettings = featureSettings;
        this.Occupied = occupied;
    }

    public class Build
    {
        private string name;//reshuffle to be enum.
        public bool occupied;
        private FeatureSettings featureSettings;//this contains all the trees, grass, flowers, and stones.
        //private SpawnSettings spawnSettings;//this will be the config for what entities can spawn, how many, and how often.

        public Biome.Build Name(string name){
            this.name = name;
            return this;
        }

        //this exists so that, if a tree generates on the tile, it will be marked as occupied and won't be regenerated on.
        //Not sure if this actually belongs here or not though, since this will be determined after feature settings are done.
        public Biome.Build Occupied(bool occupied){
            this.occupied = occupied;
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
                return new Biome(this.name, this.featureSettings, this.occupied);
            }else{
                throw new Exception("Missing Biome parameters in builder\n" + this);
            }
        }



    }

}
