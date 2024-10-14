using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct ReplaceColourJob : IJob
{

    // No idea.
    public NativeArray<Color> csvColourData;
    public NativeArray<Color> inputColours;
    public NativeArray<Color> pixelInput;

    public void Execute(){
        for(int i = 0; i < csvColourData.Length; i++){
            for(int j = 0; j < inputColours.Length; j++){
                Debug.Log($"CSVColourData Length: {csvColourData.Length}, InputColour Length: {inputColours.Length}.");
                if(CompareColour(csvColourData[i], inputColours[j])){
                    pixelInput[i] = inputColours[j];
                    Debug.Log("Successfully Replaced Colour.");
                    break;
                }
            }
            
        }
    }

    bool CompareColour(Color colorA, Color colorB)
    {
        Debug.Log("Comparing Colours.");
        float distance = Mathf.Sqrt(
            Mathf.Pow(colorA.r - colorB.r, 2) + 
            Mathf.Pow(colorA.g - colorB.g, 2) + 
            Mathf.Pow(colorA.b - colorB.b, 2) +
            Mathf.Pow(colorA.a - colorB.a, 2)
        );

        return distance < 0.0001f;
    }
}
