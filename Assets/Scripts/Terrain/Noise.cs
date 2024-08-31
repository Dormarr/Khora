using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        //Still used for editor preview tilemap for testing perlin settings.

        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-(mapWidth/2f)) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-(mapHeight/2f)) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }

        }

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateChunkNoiseMap(Vector3Int chunkPosition, int chunkSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[chunkSize, chunkSize];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        float maxPossibleHeight = -1f;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        int chunkPosX = chunkPosition.x * chunkSize;
        int chunkPosY = chunkPosition.y * chunkSize;

        float halfChunkSize = chunkSize / 2f;
        

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                float globalX = (x + chunkPosition.x * chunkSize - halfChunkSize) / scale;
                float globalY = (y + chunkPosition.y * chunkSize - halfChunkSize) / scale;

                //float worldX = (chunkPosition.x * chunkWidth + x) / scale;
                //float worldY = (chunkPosition.y * chunkHeight + y) / scale;

                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = globalX * frequency + octaveOffsets[i].x;
                    float sampleY = globalY * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise((float)sampleX, (float)sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }

        }

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                float normalizedHeight = (noiseMap[x,y] + maxPossibleHeight) / (2f * maxPossibleHeight);
                
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                noiseMap[x,y] = Mathf.Clamp01(normalizedHeight);
            }
        }

        return noiseMap;
    }

    public static float GenerateCoordinateNoise(Vector3Int coordinate, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        System.Random prng = new System.Random(seed); //implement seed.
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        float maxPossibleHeight = -1f;

        // Generate octave offsets based on the seed
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue, minNoiseHeight = float.MaxValue;

        amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        float globalX = coordinate.x / scale;
        float globalY = coordinate.y / scale;

        // Loop through each octave
        for (int i = 0; i < octaves; i++)
        {
            float sampleX = globalX * frequency + octaveOffsets[i].x;
            float sampleY = globalY * frequency + octaveOffsets[i].y;

            float perlinValue = Mathf.PerlinNoise((float)sampleX, (float)sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Track max and min noise height for normalization
        if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
        else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

        // Normalize noise value
        float normalizedHeight = (noiseHeight + maxPossibleHeight) / (2f * maxPossibleHeight);
        float noiseValue = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseHeight);
        noiseValue = Mathf.Clamp01(normalizedHeight);

        return noiseValue;
    }
}
