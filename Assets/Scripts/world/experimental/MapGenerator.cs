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

        float[,] temperatureMap = new float[mapWidth, mapHeight];
        float[,] precipitationMap = new float[mapWidth, mapHeight];
        Biome[,] biomeMap = new Biome[mapWidth, mapHeight];

        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                Vector3Int coord = new Vector3Int(x, y, 0);

                temperatureMap[x,y] = temperatureGenerator.GenerateCoordinatePerlin(coord, seed);
                precipitationMap[x,y] = precipitationGenerator.GenerateCoordinatePerlin(coord, seed);

                biomeMap[x,y] = BiomeGenerator.GetBiome(
                    temperatureMap[x,y], 
                    precipitationMap[x,y]);
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawBiomeMap(biomeMap);
        //display.RenderTileTint(temperatureMap, precipitationMap);
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
