using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;

//This will combine all the generated perlin noise maps and generate a final world based on the return values for coords.
//Coords come in, passed over to perlinGenerator, a cluster of info comes back, is assessed and rendered.
//The rendering should probably be done elsewhere though, because that's a whole other task, especially with connected tiles.


public class WorldEngine : MonoBehaviour
{
    public float erosionStrength;
    public string worldName;
    public int worldSeed;
    public string worldDate;

    public PerlinGenerator temperatureGenerator;
    public PerlinGenerator precipitationGenerator;
    public PerlinGenerator elevationGenerator;
    public PerlinGenerator erosionGenerator;

    public float temperature;
    public float precipitation;
    public float elevation;
    public float erosion;
    

    void Awake()
    {
        worldName = WorldDataTransfer.worldName;
        worldSeed = WorldDataTransfer.worldSeed;

        //I'll deal with this later.
        GetWorldSeed(worldName);
    }

    public void GetWorldSeed(string input){
        if(File.Exists(Utility.GetWorldSaveDataFilePath(input))){
            WorldSaveData wsd = Utility.LoadWorldSaveData(input);
            wsd.date = Utility.GetDateTimeString();
            worldSeed = wsd.seed;
        }else{
            SaveNewWorldSaveData($"{Utility.GenerateWorldName(worldName)}", worldSeed, Utility.GetDateTimeString());
        }
    }

    public void SaveNewWorldSaveData(string name, int seed, string date){
        worldName = name;
        worldSeed = seed;
        worldDate = date;
        WorldSaveData wsd = new WorldSaveData.Build()
            .Seed(seed)
            .Name(name)
            .Date(date)
            .BuildWorldSaveData();
        Utility.SaveWorldSaveData(wsd);
    }

    public void UpdateWorldSaveData(string name = null, string date = null){
        string updatedName = !string.IsNullOrEmpty(name)? name : worldName;
        string updatedDate = !string.IsNullOrEmpty(date)? date : worldDate;
        
        WorldSaveData wsd = new WorldSaveData.Build()
            .Seed(worldSeed)//this should never need updating beyond initialisation.
            .Name(updatedName)
            .Date(updatedDate)
            .BuildWorldSaveData();
        Utility.SaveWorldSaveData(wsd);
    }

    public async Task<ChunkData> GenerateChunkAsync(Vector3Int chunkPosition){
        return await Task.Run(() => GenerateChunk(chunkPosition));
    }

    public ChunkData GenerateChunk(Vector3Int chunkPosition){
        return new ChunkData.Build()
        .Name($"chunk_{chunkPosition}")
        .ChunkPosition(chunkPosition)
        .BiomeMapList(BiomeUtility.ArrayToList(GenerateBiomeForChunk(chunkPosition)))
        .BuildChunkData();
    }

    public Biome GenerateBiomeForCoordinate(Vector3Int coordinate){
        Vector3Int offset = new Vector3Int(Config.chunkSize / 2, Config.chunkSize / 2, 0);
        Vector3Int localCoord = coordinate - offset;
        temperature = temperatureGenerator.GenerateCoordinatePerlin(localCoord, worldSeed);
        precipitation = precipitationGenerator.GenerateCoordinatePerlin(localCoord, worldSeed);
        return BiomeGenerator.GetBiome(temperature, precipitation);
    }

    public Biome[,] GenerateBiomeForChunk(Vector3Int chunkPosition)
    {
        float[,] temperatureMap = temperatureGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] precipitationMap = precipitationGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);
        Biome[,] biomeMap = new Biome[temperatureMap.GetLength(0), precipitationMap.GetLength(1)];

        for(int i = 0; i < temperatureMap.GetLength(0); i++){
            for(int j = 0; j < precipitationMap.GetLength(1); j++){
                biomeMap[i,j] = BiomeGenerator.GetBiome(temperatureMap[i,j], precipitationMap[i,j]);
            }
        }
        return biomeMap;
    }

    public float GenerateTopologyForCoordinate(Vector3Int coordinate)
    {
        elevation = elevationGenerator.GenerateCoordinatePerlin(coordinate, worldSeed);
        erosion = erosionGenerator.GenerateCoordinatePerlin(coordinate, worldSeed);

        return TopologyGenerator.GetTopology(elevation, erosion, erosionStrength);
    }

    public float[,] GenerateTopologyForChunk(Vector3Int chunkPosition)
    {
        float[,] elevationMap = elevationGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] erosionMap = erosionGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] topologyMap = new float[elevationMap.GetLength(0),erosionMap.GetLength(0)];

        for(int i = 0; i < elevationMap.GetLength(0);  i++){
            for(int j = 0; j < erosionMap.GetLength(0); j++){
                topologyMap[i, j] = TopologyGenerator.GetTopology(elevationMap[i, j], erosionMap[i, j], erosionStrength);
            }
        }

        return topologyMap;
    }
}