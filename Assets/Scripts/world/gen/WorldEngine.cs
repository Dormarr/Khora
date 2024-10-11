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
            SaveNewWorldSaveData($"{GenerateWorldName()}", worldSeed, Utility.GetDateTimeString());
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

    public string GenerateWorldName(){
        
        //add in a system for what if the input name is already the name of a world.

        if(string.IsNullOrEmpty(worldName)){
            return WorldDataTransfer.worldName;
        }
        
        int highestNumber = 0;

        WorldSaveData[] wsds = Utility.GetWorldDataFiles();
        List<string> worldNames = new List<string>();

        foreach(WorldSaveData wsd in wsds){
            worldNames.Add(wsd.name);
        }

        Regex regex = new Regex(@"New_World_(\d+)");

        foreach(string worldName in worldNames){
            string fileName = Path.GetFileNameWithoutExtension(worldName);

            Match match = regex.Match(fileName);
            if(match.Success){
                int currentNumber = int.Parse(match.Groups[1].Value);

                if(currentNumber > highestNumber){
                    highestNumber = currentNumber;
                }
            }
        }

        string finalWorldName = $"New_World_{highestNumber + 1}";
        WorldDataTransfer.worldName = finalWorldName;
        return finalWorldName;
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
        temperature = temperatureGenerator.GenerateCoordinatePerlin(coordinate - offset, worldSeed);
        precipitation = precipitationGenerator.GenerateCoordinatePerlin(coordinate - offset, worldSeed);
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

public static class BiomeGenerator
{
    public static Biome GetBiome(float temperature, float precipitation)
    {
        Biome biome = BiomeClimateRegistry.GetBiome(temperature, precipitation);
        if(biome != null){
            return biome;
        }
        
        throw new Exception("Big whoops, cannot find biome in registry.");
    }
}

public static class TopologyGenerator
{
    public static float GetTopology(float elevation, float erosion, float erosionStrength)
    {
        float erodedElevation = elevation - (erosion * erosionStrength);
        return Mathf.Clamp01(erodedElevation);
    }
}
