using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite defaultSprite;
    public GradientTile gradientTile;

    public UVColourMap uVColourMap;

    public void DrawBiomeMap(Biome[,] biomeMap){
        tilemap.ClearAllTiles();
        int width = biomeMap.GetLength(0);
        int height = biomeMap.GetLength(1);

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){

                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, gradientTile);
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

    public void RenderTileColours(float[,] temperatureMap, float[,] precipitationMap){

        // Values set to 1 and -1 so as to leave a border of data which avoids out-of-bounds issues.
        for(int x = 1; x < temperatureMap.GetLength(0) -1; x++){
            for(int y = 1; y < temperatureMap.GetLength(1) -1; y++){
                Color[] tiles = GetTileNeighbourColours(temperatureMap, precipitationMap, x, y);

                Color[] colours = Utility.Get8WayGradient(tiles);

                GradientTile tile = ScriptableObject.CreateInstance<GradientTile>();
                tile.Initialize(new Vector3Int(x,y,0), colours, defaultSprite);

                tilemap.SetTile(new Vector3Int(x,y,0), tile);
                tilemap.RefreshAllTiles();
            }
        }
    }

    public Color[] GetTileNeighbourColours(float[,] temperatureMap, float[,] precipitationMap, int x, int y){

        Color[] tileColours = new Color[9];
        
        int index = 0;

        // [0,1,2]
        // [3,4,5]
        // [6,7,8]

        //Gets the colours in rows from bottom left.
        for(int yOffset = 1; yOffset >= -1; yOffset--){
            for(int xOffset = -1; xOffset <= 1; xOffset++){
                int xCoord = x + xOffset;
                int yCoord = y + yOffset;

                // Safety check to ensure we don't access out-of-bounds
                if (xCoord >= 0 && xCoord < temperatureMap.GetLength(0) && yCoord >= 0 && yCoord < precipitationMap.GetLength(1))
                {
                    // Get the colour based on the temperature and precipitation map values at this tile
                    tileColours[index] = uVColourMap.GetColourFromUVMap(temperatureMap[xCoord, yCoord], precipitationMap[xCoord, yCoord]);
                }
                else
                {
                    // Backup colour.
                    tileColours[index] = Color.white;
                }

                index++;
            }
        }

        return tileColours;
    }
}
