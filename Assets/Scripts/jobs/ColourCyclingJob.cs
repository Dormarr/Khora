using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct ColourCyclingJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<int> uvInts;
    [ReadOnly] public NativeArray<Color> inputColours;
    public NativeArray<Color> outputColours;

    public void Execute(int index){
        int uvInt = uvInts[index];
        outputColours[index] = (uvInt >= 0 && uvInt < inputColours.Length) ? inputColours[uvInt] : Color.red;
    }
}
