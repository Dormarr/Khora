using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Tilemap tileMap;
    public Tile[] tiles;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        tileMap.ClearAllTiles();

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);


        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Tile selectedTile = SelectTile(noiseMap[x,y]);
                tileMap.SetTile(new Vector3Int(x - (width / 2), y - (height / 2), 0), selectedTile);
                
            }
        }
    }


    Tile SelectTile(float perlinValue)
    {
        int i = Mathf.RoundToInt(perlinValue * tiles.Length);
        int tileIndex = Mathf.Clamp(i, 0, tiles.Length-1);
        return tiles[tileIndex];
    }

}
