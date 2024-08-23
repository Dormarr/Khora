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

        //set tiles according to proportional decimal to tile.
        
        //perlin value * tile.count, rounded to nearest int, and assign tile[int] as the tile. That should keep it all in check.

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Tile selectedTile = SelectTile(noiseMap[x,y]);
                tileMap.SetTile(new Vector3Int(x - (width / 2), y - (height / 2), 0), selectedTile);
                
            }
        }

        //also need to fix coordinates to ensure it's all centered. So x = x - (width.Length / 2); dont for x and y should center around 0,0;
        //Need to consider the implications on chunks doing this though.
    }


    Tile SelectTile(float perlinValue)
    {
        int i = Mathf.RoundToInt(perlinValue * tiles.Length);
        int tileIndex = Mathf.Clamp(i, 0, 5);
        return tiles[tileIndex];
    }

}
