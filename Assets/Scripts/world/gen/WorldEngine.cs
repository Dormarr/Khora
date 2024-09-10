using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This will combine all the generated perlin noise maps and generate a final world based on the return values for coords.
//Coords come in, passed over to perlinGenerator, a cluster of info comes back, is assessed and rendered.
//The rendering should probably be done elsewhere though, because that's a whole other task, especially with connected tiles.


public class WorldEngine : MonoBehaviour
{
    public float erosionStrength;
    public int worldSeed;

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
        //check to see if any world data file exists, if so, pull seed from there.
        worldSeed = Seed.GenerateSeed();
    }

    public ChunkData GenerateChunk(Vector3Int chunkPosition){
        return new ChunkData.Build()
        .Name($"chunk_{chunkPosition}")
        .ChunkPosition(chunkPosition)
        .BiomeMapList(BiomeUtility.ArrayToList(GenerateBiomeForChunk(chunkPosition)))//this shouldn't be done.
        .BuildChunkData();
    }

    public ChunkData LoadChunkDataFromSave(Vector3Int chunkPosition){
        //use the chunkSerializer to grab tileData to regenerate saved modifications to the terrain.
        //if null, return empty chunkData.

        //Is this the right way to do it? Won't loaded chunks work anyway?
        return null;
    }

    public Biome GenerateBiomeForCoordinate(Vector3Int coordinate)
    {
        //this should be adjust to account for coordinates previously generated.

        Vector3Int offset = new Vector3Int(Config.chunkSize / 2, Config.chunkSize / 2, 0);

        temperature = temperatureGenerator.GenerateCoordinatePerlin(coordinate - offset, worldSeed);
        precipitation = precipitationGenerator.GenerateCoordinatePerlin(coordinate - offset, worldSeed);

        return BiomeGenerator.GetBiome(temperature, precipitation);
        //time to implement a save feature to cache the biomes and optimise the gameplay.
    }

    public Biome[,] GenerateBiomeForChunk(Vector3Int chunkPosition)
    {
        //this should be adjusted to account for chunks previous generated.

        float[,] temperatureMap = temperatureGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] precipitationMap = precipitationGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);

        //redefine to use a range for precipitation and temperature rather than a table.


        Biome[,] biomeMap = new Biome[temperatureMap.GetLength(0), precipitationMap.GetLength(1)];

        for(int i = 0; i < temperatureMap.GetLength(0); i++)
        {
            for(int j = 0; j < precipitationMap.GetLength(1); j++)
            {
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

        for(int i = 0; i < elevationMap.GetLength(0);  i++)
        {
            for(int j = 0; j < erosionMap.GetLength(0); j++)
            {
                topologyMap[i, j] = TopologyGenerator.GetTopology(elevationMap[i, j], erosionMap[i, j], erosionStrength);
            }
        }

        return topologyMap;
    }
}

public static class BiomeGenerator
{
    //private static BiomeSearcher biomeSearcher;
    static BiomeGenerator()
    {  
        //biomeSearcher = new BiomeSearcher(categoryRegistry);
    }

    public static Biome GetBiome(float temperature, float precipitation)
    {
        Biome biome = BiomeClimateRegistry.GetBiome(temperature, precipitation);
        if(biome != null){
            Debug.Log($"Biome returned: {biome.Name}");
            return biome;
        }
        //biome = biomeSearcher.SearchBiome("biomes", temperature, precipitation);
        
        throw new Exception("Big whoops, unable to get biome in WorldEngine.GetBiome()");

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
