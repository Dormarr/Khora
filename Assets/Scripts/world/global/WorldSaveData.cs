using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class WorldSaveData
{
    public int seed;


    WorldSaveData(int seed){
        this.seed = seed;
    }

    public class Build{
        private int seed;

        public WorldSaveData.Build Seed(int seed){
            this.seed = seed;
            return this;
        }

        public WorldSaveData BuildWorldSaveData(){
            if(this.seed != null){
                return new WorldSaveData(seed);
            }else{
                throw new Exception($"Missing WorldSaveData parameter\n {this}");
            }
        }
    }
}
