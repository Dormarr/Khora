using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class WorldSaveData
{
    public int seed;
    public string name;
    public string date;


    WorldSaveData(int seed, string name, string date){
        this.seed = seed;
        this.name = name;
        this.date = date;
    }

    public class Build{
        private int seed;
        private string name;
        private string date;

        public WorldSaveData.Build Seed(int seed){
            this.seed = seed;
            return this;
        }

        public WorldSaveData.Build Name(string name){
            this.name = name;
            return this;
        }

        public WorldSaveData.Build Date(string date){
            this.date = date;
            return this;
        }

        public WorldSaveData BuildWorldSaveData(){
            if(this.seed != null && this.name != null && this.date != null){
                return new WorldSaveData(seed, name, date);
            }else{
                throw new Exception($"Missing WorldSaveData parameter\n {this}");
            }
        }
    }
}
