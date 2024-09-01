using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is passed into the json files and is subsequently what determines how biomes are generated.


[System.Serializable]
public class Biome
{
    public string biomeName;
    public float minTemperature;
    public float maxTemperature;
    public float minPrecipitation;
    public float maxPrecipitation;

    //public SpawnSettings

}
