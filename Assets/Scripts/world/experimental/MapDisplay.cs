using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] tiles;
    public Tile blankTile;

    public UVColourMap uVColourMap;

    public void DrawNoiseMap(float[,] noiseMap)
    {
        tilemap.ClearAllTiles();

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);


        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Tile selectedTile = SelectTile(noiseMap[x,y]);
                tilemap.SetTile(new Vector3Int(x - (width / 2), y - (height / 2), 0), selectedTile);
                
            }
        }
    }

    public void DrawBiomeMap(Biome[,] biomeMap){
        tilemap.ClearAllTiles();
        int width = biomeMap.GetLength(0);
        int height = biomeMap.GetLength(1);

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){
                Biome tileBiome = biomeMap[x, y];
                TileBase selectedTile = BiomeUtility.GetTileFromBiome(tileBiome);
                if(selectedTile == null){
                    selectedTile = blankTile;
                }
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, blankTile);
            }
        }
    }

    public void RenderTileTint(float[,] temperatureMap, float[,] precipitationMap){
        int width = temperatureMap.GetLength(0);
        int height = temperatureMap.GetLength(1);

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                uVColourMap.ApplyColourToTile(temperatureMap[x,y], precipitationMap[x,y], tilemap, new Vector3Int(x,y,0));
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
