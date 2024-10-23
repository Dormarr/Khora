using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WorldRenderer{
    // Aiming to separate out the rendering code from the generation code for world gen.

    // Depending on ticket type, render fully or partially, by chunk or by tile.
    static int chunkSize => Config.chunkSize;

    public static void RenderChunk(Chunk chunk, int type){
        // Pass through the perlin.

        // The following is just for testing and debugging purposes.
        int chunkSize = Config.chunkSize;

        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        TileBase tile = TextureUtility.GetTestSpriteByInt(type);

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

}