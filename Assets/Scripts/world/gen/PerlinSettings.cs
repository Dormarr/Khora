using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinSettings
{
    public int Size { get; private set; }
    public float Scale { get; private set; }
    public int Octaves { get; private set; }
    public float Persistence { get; private set; }
    public float Lacunarity { get; private set; }
    public Vector2 Offset { get; private set; }

    public PerlinSettings(int size, float scale, int octaves, float persistence, float lacunarity, Vector2 offset){
        Size = size;
        Scale = scale;
        Octaves = octaves;
        Persistence = persistence;
        Lacunarity = lacunarity;
        Offset = offset;

    }
}
