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
    [SerializeField] public float noiseScale;
    [SerializeField] public int octaves;
    [Range(0,1)]
    [SerializeField] public float persistence;
    [SerializeField] public float lacunarity;
    [SerializeField] public Vector2 offset;

    private int chunkSize => Config.chunkSize;


    public float[,] GenerateChunkPerlin(Vector3Int chunkPosition, int seed)
    {
        return Noise.GenerateChunkNoiseMap(chunkPosition, chunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
    }

    public float[,] GenerateChunkPerlinWithBorder(Vector3Int chunkPosition, int seed)
    {
        return Noise.GenerateChunkNoiseMapWithBorder(chunkPosition, chunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
    }

    public float GenerateCoordinatePerlin(Vector3Int coordinate, int seed)
    {
        return Noise.GenerateCoordinateNoise(coordinate, seed,  noiseScale, octaves, persistence,lacunarity, offset);
    }

        void OnValidate()
    {
        if(octaves < 1)
        {
            octaves = 1;
        }
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
    }

}
