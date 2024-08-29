using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BiomeData", menuName = "Biome/Biomedata", order = 1)]
public class BiomeData : ScriptableObject
{
    public Biome biomeName;
    public TileBase tile; //expand once dual grid system in place.

    public Color biomeColour; //temp for visualization

    public float minTemperature;
    public float maxTemperature;
    public float minHumidity;
    public float maxHumidity;

    public float erosionModifier = 1f; //use later for 3rd wave terrain gen.
}
