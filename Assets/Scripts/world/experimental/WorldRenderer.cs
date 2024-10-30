using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TextureUtility;
using System.Diagnostics;

public static class WorldRenderer{
    // Aiming to separate out the rendering code from the generation code for world gen.

    // Depending on ticket type, render fully or partially, by chunk or by tile.
    static int chunkSize => Config.chunkSize;

    public static void RenderTestChunk(Chunk chunk, int type){
        // Pass through the perlin.

        // The following is just for testing and debugging purposes.
        int chunkSize = Config.chunkSize;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // TileBase tile = GetTestSpriteByInt(type);
        TileBase tile = GetGradientTileByName("debug", "debug");

        for(int x = 0; x < chunkSize; x ++){
            for(int y = 0; y < chunkSize; y++){
                int localX = chunk.Position.x * chunkSize + x;
                int localY = chunk.Position.y * chunkSize + y;

                chunk.SetTile(new Vector3Int(localX, localY, 0), tile);
            }
        }

        stopwatch.Stop();
        Debug.Log($"WorldRenderer.RenderChunk: Succesfully rendered chunk {chunk.Position}. \n Time Taken: {stopwatch.ElapsedMilliseconds} ms.");
    }
    public static void RenderChunk(Chunk chunk){
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        RenderTileColours(chunk, "debug");

        stopwatch.Stop();
        Debug.Log($"WorldRenderer.RenderChunk: Successfully rendered chunk {chunk.Position}. \n Time taken: {stopwatch.ElapsedMilliseconds} ms.");

        //Draw mods has to be done using save data, which isn't yet implemented.
        //DrawModifications();
    }

    public static void RenderTileColours(Chunk chunk, string tileName){

        // This can't be the best way to do this.
        float[,] temperatureMap = Noise.GenerateChunkNoiseMapWithBorderFromSettings(chunk.Position, WorldDataTransfer.worldSeed, WorldSettings.Temperature);
        float[,] precipitationMap = Noise.GenerateChunkNoiseMapWithBorderFromSettings(chunk.Position, WorldDataTransfer.worldSeed, WorldSettings.Precipitation);

        //This needs redoing depending on the noiseMap values.
        GradientTile selectedTile = GetGradientTileByName(tileName, tileName);
        List<(Vector3Int pos, GradientTile tile)> tiles = new List<(Vector3Int pos, GradientTile tile)>();


        int width = Config.chunkSize;
        int height = Config.chunkSize;

        int chunkOffsetX = chunk.Position.x * width;
        int chunkOffsetY = chunk.Position.y * height;

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                //Should define the selected tile here. Dictated by temp and precip values, according to climateRange.
                //For now I'll leave it hard coded, but Gradient tile will need a name.
                string name = "debug";

                Color[] neighbourColours = GetTileNeighbourColours(temperatureMap, precipitationMap, x, y, selectedTile.colourMap, name);
                
                Color[] colours = TextureUtility.Get8WayGradient(neighbourColours);
                
                GradientTile tile = ScriptableObject.CreateInstance<GradientTile>();                
                Vector3Int tilePos = new Vector3Int(chunkOffsetX + x, chunkOffsetY + y, 0);

                // The initialization process needs to be improved.
                tile.Initialize(tilePos, colours, selectedTile.mainTexture, name);


                tiles.Add((tilePos, tile));
            }
        }

        var stopwatch1 = new Stopwatch();
        stopwatch1.Start();
        foreach (var (tilePos, tile) in tiles)
        {
            // This is what's bottlenecking the generation time.
            // Can I send this out to a Unity job?
            chunk.Tilemap.SetTile(tilePos, tile);

            
        }
        stopwatch1.Stop();
        Debug.Log($"WorldRenderer.RenderTileColours: Applied tiles to tilemap. \n Time taken: {stopwatch1.ElapsedMilliseconds} ms.");

        stopwatch.Stop();
        Debug.Log($"WorldRenderer.RenderTileColours: Tiles rendered and set in \b{stopwatch.ElapsedMilliseconds} ms.");

    }

    private static Color[] GetTileNeighbourColours(float[,] temperatureMap, float[,] precipitationMap, int x, int y, Texture2D colourMap, string name){

        // Cross reference name with dedicated list of tiles requiring colour replacement.
        // If name is not in list, return.

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Color[] tileColours = new Color[9];
        int mapSize = temperatureMap.GetLength(0);

        int index = 0;

        for(int yOffset = 1; yOffset >= -1; yOffset--){
            for(int xOffset = -1; xOffset <= 1; xOffset++){

                int neighbourX = x + xOffset + 1;
                int neighbourY = y + yOffset + 1;

                if(neighbourX >= 0 && neighbourX < mapSize && neighbourY >= 0 && neighbourY < mapSize){

                    float temp = temperatureMap[neighbourX, neighbourY];
                    float precip = precipitationMap[neighbourX, neighbourY];

                    tileColours[index] = UVColourMap.GetColourFromUVMap(temp, precip, colourMap);
                }
                else{
                    tileColours[index] = Color.white;
                }

                index++;
            }
        }

        stopwatch.Stop();
        Debug.Log($"WorldRenderer.GettileNeighbourColours: Indexed tile neighbour colours. \n Time taken: {stopwatch.ElapsedMilliseconds} ms");

        return tileColours;
    }



}