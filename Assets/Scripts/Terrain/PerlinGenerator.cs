using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//This script exists as a way to centralise perlin noise map generation, using the noise script.
//This is the central place to tinker with settings, tiles, and definitions.
//It should be attached to independent gameobjects representing different perlin layers.
//This should only ever be called by the world engine.

//Hopefully this makes ChunkLoader.DrawNoiseMap() redundant, leading to rework for dedicated environment rendering.


public class PerlinGenerator : MonoBehaviour
{
    public int seed; //this need to come from the seed generation script.

    public Tile[] tiles;

    [SerializeField] private int octaves;
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private float noiseScale;
    [SerializeField] private Vector2 offset;


    //I'm not sure what should be passed into this, because I'm not sure what needs to be generated.
    //It should probably be done chunk by chunk, right? Or does this exist just to return perlin noise based on coord data?
    //public void GenerateNoiseMap()
}
