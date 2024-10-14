using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class UVColourMap
{
    public static Color GetColourFromUVMap(float temperature, float precipitation, Texture2D colourMap){
        temperature = Mathf.Clamp01(temperature);
        precipitation = Mathf.Clamp01(precipitation);

        Texture2D tex = colourMap;
        int texSize = 16;

        int u = Mathf.FloorToInt(temperature * texSize);
        int v = Mathf.FloorToInt(precipitation * texSize);

        Color pixelColour = tex.GetPixel(u, v);
        return pixelColour;
    }

    public static void ApplyColourToTile(float temperature, float precipitation, Tilemap tilemap, Vector3Int tilePos, Texture2D colourMap){

        //use this colour value to determine the central colour for the tile to work outwards from.
        //The gradient and complimentary shades should start here.

        tilemap.SetTileFlags(tilePos, TileFlags.None);
        Color tintColour = GetColourFromUVMap(temperature, precipitation, colourMap);
    }
}
