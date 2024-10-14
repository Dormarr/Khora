using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public struct NeighbourColoursJob : IJob
{
    [ReadOnly] public NativeArray<float> temperatureMap;
    [ReadOnly] public NativeArray<float> precipitationMap;
    [WriteOnly] public NativeArray<Color> tileColours;
    public int xCoord;
    public int yCoord;
    public int mapSize;
    public Texture2D colourMap;

    public void Execute(){
        int index = 0;

        for(int yOffset = 1; yOffset >= -1; yOffset--){
            for(int xOffset = -1; xOffset <= 1; xOffset++){
                int neighbourX = xCoord + xOffset;
                int neighbourY = yCoord + yOffset;

                if(neighbourX >= 0 && neighbourX < mapSize && neighbourY >= 0 && neighbourY < mapSize){
                    float temp = temperatureMap[neighbourX + neighbourY * mapSize];
                    float precip = precipitationMap[neighbourX + neighbourY * mapSize];

                    tileColours[index] = UVColourMap.GetColourFromUVMap(temp, precip, colourMap);
                }
                else{
                    tileColours[index] = Color.white;
                }

                index++;
            }
        }
    }
}
