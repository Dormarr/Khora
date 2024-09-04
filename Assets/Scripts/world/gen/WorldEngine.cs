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
        worldSeed = Seed.GenerateSeed();
    }

    public BiomeEnum GenerateBiomeForCoordinate(Vector3Int coordinate)
    {
        //this should be adjust to account for coordinates previously generated.

        Vector3Int offset = new Vector3Int(Config.chunkSize / 2, Config.chunkSize / 2, 0);

        temperature = temperatureGenerator.GenerateCoordinatePerlin(coordinate - offset, worldSeed);
        precipitation = precipitationGenerator.GenerateCoordinatePerlin(coordinate - offset, worldSeed);

        return BiomeGenerator.GetBiome(temperature, precipitation);
    }

    public BiomeEnum[,] GenerateBiomeForChunk(Vector3Int chunkPosition)
    {
        //this should be adjusted to account for chunks previous generated.

        float[,] temperatureMap = temperatureGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] precipitationMap = precipitationGenerator.GenerateChunkPerlin(chunkPosition, worldSeed);

        //redefine to use a range for precipitation and temperature rather than a table.


        BiomeEnum[,] biomeMap = new BiomeEnum[temperatureMap.GetLength(0), precipitationMap.GetLength(1)];

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
    private static BiomeEnum[,] biomeTable;

    static BiomeGenerator()
    {

        //I'd like to configure the enums to have temp and precip tags to generate the table with more scalability.


        biomeTable = new BiomeEnum[,]
        {
            //Columns | Precipitation (low to high)
            //Rows | Temperature (low to high)

            {BiomeEnum.Tundra, BiomeEnum.SnowyForest, BiomeEnum.Glacial },
            {BiomeEnum.Shrubland, BiomeEnum.Forest, BiomeEnum.Swamp },
            {BiomeEnum.SandDesert, BiomeEnum.Grassland, BiomeEnum.Rainforest }
        };
    }

    public static BiomeEnum GetBiome(float temperature, float precipitation)
    {
        //this need to be done by registryLookup and returned as a new class that contains the appropriate info for each tile.
        //Biome[] biomes = RegistryKeys.BIOME.GetRegistry().GetAllValues();

        //IEnumerable<Biome> biomes = RegistryKeys.BIOME.GetRegistry.GetAllValues();
        Identifier identifier = RegistryKeys.BIOME.GetValue();
        // string idName = BiomeRegistry.PLAINS.GetValue().GetName();
        // string idPath = BiomeRegistry.PLAINS.GetValue().GetPath();

        // Debug.Log($"Biome Reg: {identifier.GetPath()}{identifier.GetName()}, also registered {idName}:{idPath}");

        int tempIndex = Mathf.Clamp((int)(temperature * biomeTable.GetLength(0)), 0, biomeTable.GetLength(0) - 1);
        int precipIndex = Mathf.Clamp((int)(precipitation * biomeTable.GetLength(1)), 0, biomeTable.GetLength(1) - 1);

        return biomeTable[tempIndex, precipIndex];
    }

    //need a method that determines which biome to pull from the registry.
}

public static class TopologyGenerator
{
    public static float GetTopology(float elevation, float erosion, float erosionStrength)
    {
        float erodedElevation = elevation - (erosion * erosionStrength);

        return Mathf.Clamp01(erodedElevation);
    }
}
