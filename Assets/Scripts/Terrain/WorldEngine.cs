using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will combine all the generated perlin noise maps and generate a final world based on the return values for coords.
//Coords come in, passed over to perlinGenerator, a cluster of info comes back, is assessed and rendered.
//The rendering should probably be done elsewhere though, because that's a whole other task, especially with connected tiles.


public class WorldEngine : MonoBehaviour
{
    //this will probably need it's own cache, or assign a cache to the chunk it generate to?
    //Assign by coordinate values, and attach to the chunk gameobject to be drawn on later by other stuff.
    //Maybe create a scriptable object that contains the 4 values, and give each chunk their 32x32 share to store.

    //Need to grab chunkPosition.

    public float erosionStrength;
    public int worldSeed;

    public PerlinGenerator tempGen;
    public PerlinGenerator precipGen;
    public PerlinGenerator elevGen;
    public PerlinGenerator erosGen;

    public float temperature;
    public float precipitation;
    public float elevation;
    public float erosion;
    
    void Awake()
    {
        worldSeed = Utility.GenerateWorldSeedFromString("temp");
    }

    public Biome GenerateBiomeForCoordinate(Vector3Int coordinate)
    {
        Vector3Int offset = new Vector3Int(Config.chunkSize / 2, Config.chunkSize / 2, 0);

        temperature = tempGen.GenerateCoordinatePerlin(coordinate - offset, worldSeed);
        precipitation = precipGen.GenerateCoordinatePerlin(coordinate - offset, worldSeed);

        return BiomeGenerator.GetBiome(temperature, precipitation);
    }

    public Biome[,] GenerateBiomeForChunk(Vector3Int chunkPosition)
    {
        float[,] temperatureMap = tempGen.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] precipitationMap = precipGen.GenerateChunkPerlin(chunkPosition, worldSeed);


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
        elevation = elevGen.GenerateCoordinatePerlin(coordinate, worldSeed);
        erosion = erosGen.GenerateCoordinatePerlin(coordinate, worldSeed);

        return TopologyGenerator.GetTopology(elevation, erosion, erosionStrength);
    }

    public float[,] GenerateTopologyForChunk(Vector3Int chunkPosition)
    {
        float[,] elevationMap = elevGen.GenerateChunkPerlin(chunkPosition, worldSeed);
        float[,] erosionMap = erosGen.GenerateChunkPerlin(chunkPosition, worldSeed);

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
    private static Biome[,] biomeTable;

    static BiomeGenerator()
    {

        //I'd like to configure the enums to have temp and precip tags to generate the table with more scalability.


        biomeTable = new Biome[,]
        {
            //Columns | Precipitation (low to high)
            //Rows | Temperature (low to high)

            {Biome.Tundra, Biome.SnowyForest, Biome.Glacial },
            {Biome.Shrubland, Biome.Forest, Biome.Swamp },
            {Biome.SandDesert, Biome.Grassland, Biome.Rainforest }
        };
    }

    public static Biome GetBiome(float temperature, float precipitation)
    {
        int tempIndex = Mathf.Clamp((int)(temperature * biomeTable.GetLength(0)), 0, biomeTable.GetLength(0) - 1);
        int precipIndex = Mathf.Clamp((int)(precipitation * biomeTable.GetLength(1)), 0, biomeTable.GetLength(1) - 1);

        return biomeTable[tempIndex, precipIndex];
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
