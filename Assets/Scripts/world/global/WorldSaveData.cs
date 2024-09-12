using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class WorldSaveData
{
    public int seed;
    public string name;


    WorldSaveData(int seed, string name){
        this.seed = seed;
        this.name = name;
    }

    public class Build{
        private int seed;
        private string name;

        public WorldSaveData.Build Seed(int seed){
            this.seed = seed;
            return this;
        }

        public WorldSaveData.Build Name(string name){
            this.name = name;
            return this;
        }

        public WorldSaveData BuildWorldSaveData(){
            if(this.seed != null && this.name != null){
                return new WorldSaveData(seed, name);
            }else{
                throw new Exception($"Missing WorldSaveData parameter\n {this}");
            }
        }
    }
}
