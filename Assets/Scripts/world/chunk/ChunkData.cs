using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ChunkData
{
    public Vector3Int chunkPosition;
    public string name;
    //public Biome[,] biomeMap;
    
    public List<BiomeData> biomeMapList;
    ChunkData(string name, Vector3Int chunkPosition, List<BiomeData> biomeMapList){
        this.chunkPosition = chunkPosition;
        //this.biomeMap = biomeMap;
        this.biomeMapList = biomeMapList;
    }

    public class Build{
        
        private Vector3Int chunkPosition;
        private string name;
        //private Biome[,] biomeMap;
        private List<BiomeData> biomeMapList;

        public ChunkData.Build ChunkPosition(Vector3Int chunkPosition){
            this.chunkPosition = chunkPosition;
            return this;
        }

        // public ChunkData.Build BiomeMap(Biome[,] biomeMap){
        //     this.biomeMap = biomeMap;
        //     return this;
        // }

        public ChunkData.Build BiomeMapList(List<BiomeData> biomeMapList){
            this.biomeMapList = biomeMapList;
            return this;
        }

        public ChunkData.Build Name(string name){
            this.name = name;
            return this;
        }


        public ChunkData BuildChunkData(){
            if(this.name != null && this.chunkPosition != null && this.biomeMapList != null){
                return new ChunkData(this.name, this.chunkPosition, this.biomeMapList);
            }else{
                throw new Exception("Missing ChunkData parameter in build\n" + this);
            }
        }
    }


}

[System.Serializable]
public struct BiomeData{
    public int x;
    public int y;
    public string biomeName;

    public BiomeData(int x, int y, string biomeName){
        this.x = x;
        this.y = y;
        this.biomeName = biomeName;
    }
}
