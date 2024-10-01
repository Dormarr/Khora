using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Generator")]
    public PerlinGenerator temperatureGenerator;
    public PerlinGenerator precipitationGenerator;
    public PerlinGenerator elevationGenerator;
    public PerlinGenerator erosionGenerator;

    [Header("Map Config")]
    public int mapWidth;
    public int mapHeight;

    public int seed;

    public bool autoUpdate;

    private float temperature;
    private float precipitation;
    private float elevation;
    private float erosion;

    public void GenerateBiomeMap(){
        //generate perlin for each coordinate, for temp and precip.
        //Determine by map width.

        float[,] temperatureMap = new float[mapWidth, mapHeight];
        float[,] precipitationMap = new float[mapWidth, mapHeight];
        Biome[,] biomeMap = new Biome[mapWidth, mapHeight];

        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                Vector3Int coord = new Vector3Int(x, y, 0);
                biomeMap[x,y] = BiomeGenerator.GetBiome(temperatureGenerator.GenerateCoordinatePerlin(coord, seed), precipitationGenerator.GenerateCoordinatePerlin(coord, seed));
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawBiomeMap(biomeMap);

    }

        void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
    }
}
