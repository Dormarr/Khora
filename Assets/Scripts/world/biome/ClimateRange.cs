using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ClimateRange
{
    public float MinTemperature;
    public float MaxTemperature;
    public float MinPrecipitation;
    public float MaxPrecipitation;

    public ClimateRange(float minTemp, float maxTemp, float minPrec, float maxPrec)
    {
        MinTemperature = minTemp;
        MaxTemperature = maxTemp;
        MinPrecipitation = minPrec;
        MaxPrecipitation = maxPrec;
    }

    public bool Contains(float temperature, float precipitation)
    {
        return temperature >= MinTemperature && temperature <= MaxTemperature &&
               precipitation >= MinPrecipitation && precipitation <= MaxPrecipitation;
    }
}
