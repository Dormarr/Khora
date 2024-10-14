using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct TileColourJob : IJobParallelFor
{
    
    [ReadOnly] public NativeArray<float> temperatureMap;
    [ReadOnly] public NativeArray<float> precipitationMap;
    public NativeArray<Color> tileColours;
    public Texture2D colourMap;
    public int mapWidth;

    public void Execute(int index){
        int x = index % mapWidth;
        int y = index / mapWidth;

        float temperature = temperatureMap[index];
        float precipitation = precipitationMap[index];

        tileColours[index] = UVColourMap.GetColourFromUVMap(temperature, precipitation, colourMap);
    }

    


}
