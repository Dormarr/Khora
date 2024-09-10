using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ChunkData
{
    public Vector3Int chunkPosition;
    public string name;    
    public List<BiomeData> biomeMapList; //this shouldn't really need doing.
    public List<TileData> tileDataList;


    ChunkData(string name, Vector3Int chunkPosition, List<BiomeData> biomeMapList, List<TileData> tileDataList){
        this.chunkPosition = chunkPosition;
        //this.biomeMap = biomeMap;
        this.biomeMapList = biomeMapList;
        this.tileDataList = tileDataList;
    }

    public class Build{
        
        private Vector3Int chunkPosition;
        private string name;
        //private Biome[,] biomeMap;
        private List<BiomeData> biomeMapList;
        private List<TileData> tileDataList;

        public ChunkData.Build Name(string name){
            this.name = name;
            return this;
        }
        public ChunkData.Build ChunkPosition(Vector3Int chunkPosition){
            this.chunkPosition = chunkPosition;
            return this;
        }

        public ChunkData.Build TileDataList(List<TileData> tileDataList){
            this.tileDataList = tileDataList;
            return this;
        }

        public ChunkData.Build BiomeMapList(List<BiomeData> biomeMapList){
            this.biomeMapList = biomeMapList;
            return this;
        }



        public ChunkData BuildChunkData(){
            if(this.name != null && this.chunkPosition != null){
                return new ChunkData(this.name, this.chunkPosition, this.biomeMapList, this.tileDataList);
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

    //Add more as the sim grows, but beware of clogging save data.

    public BiomeData(int x, int y, string biomeName){
        this.x = x;
        this.y = y;
        this.biomeName = biomeName;
    }
}

[System.Serializable]
public struct TileData{
    public int x;
    public int y;
    public string tileName;
    //public bool occupied;

    //Add more as the sim grows.

    public TileData(int x, int y, string tileName){
        this.x = x;
        this.y = y;
        this.tileName = tileName;
    }
}
