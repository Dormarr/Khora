using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile[] tiles;
    public Tile blankTile;
    public Sprite defaultSprite;
    public GradientTile gradientTile;

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

                tilemap.SetTile(tilePosition, gradientTile);
                //tilemap.RefreshAllTiles();
            }
        }
    }

    public void EmptyShaderColours(MaterialPropertyBlock mpb){
        mpb.SetColor("_Color1",  Color.clear);
        mpb.SetColor("_Color2",  Color.clear);
        mpb.SetColor("_Color3",  Color.clear);
        mpb.SetColor("_Color4",  Color.clear);
        mpb.SetColor("_Color5",  Color.clear);
        mpb.SetColor("_Color6",  Color.clear);
        mpb.SetColor("_Color7",  Color.clear);
        mpb.SetColor("_Color8",  Color.clear);
        mpb.SetColor("_Color9",  Color.clear);
        mpb.SetColor("_Color10", Color.clear);
        mpb.SetColor("_Color11", Color.clear);
        mpb.SetColor("_Color12", Color.clear);
        mpb.SetColor("_Color13", Color.clear);
        mpb.SetColor("_Color14", Color.clear);
        mpb.SetColor("_Color15", Color.clear);
        mpb.SetColor("_Color16", Color.clear);
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

                // Color[] colours = Utility.Get4WayGradient(tiles[0], tiles[2], tiles[6], tiles[8]);
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

        //  [0,1,2]
        //  [3,4,5]
        //  [6,7,8]

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


    Tile SelectTile(float perlinValue)
    {
        int i = Mathf.RoundToInt(perlinValue * tiles.Length);
        int tileIndex = Mathf.Clamp(i, 0, tiles.Length-1);
        return tiles[tileIndex];
    }

}
