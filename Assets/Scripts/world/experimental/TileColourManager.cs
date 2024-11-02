using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColourManager : MonoBehaviour
{
    // After so so much experimentation and countless hours of trial and error, I simply can't get the effect I want with shaders.
    // I managed to get some funky stuff using this script, spliting the tilemap into a few different sections, but no dice.
    // It might be possible, but I just don't know how to pursue this idea any further.
    // I'll keep this here anyway, but I'll have to stick with individually drawn textures for tiles.

    // NOTE: This requires the custom shader material to be applied to the tilemap itself.
    public Tilemap tilemap; // Assign the Tilemap component
    public Material sharedMaterial; // The shared material with your custom shader

    private void Start()
    {
        // DISABLED FOR NOW
        // ApplyTileColours();
    }

    // Example method to apply colors to the entire tilemap
    public void ApplyTileColours()
    {
        // Loop through all positions within the tilemap's bounds
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        // Create a single MaterialPropertyBlock to reuse for all tiles

        foreach (var pos in bounds.allPositionsWithin)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Vector3Int localPos = new Vector3Int(pos.x, pos.y, 0);
            TileBase tile = tilemap.GetTile(localPos);
            if (tile == null) continue;

            // Calculate colours or load them from somewhere based on logic (example placeholder)
            Color[] newColours = CalculateColoursForTile(localPos);

            // Set each colour property to the MaterialPropertyBlock
            Color[] replaceColours = ColourLibrary.Get("debug");

            Vector4[] replaceColourVectors = new Vector4[16];
            Vector4[] newColourVectors = new Vector4[16];

            for(int i = 0; i < replaceColourVectors.Length; i++){
                replaceColourVectors[i] = replaceColours[i];
                newColourVectors[i] = newColours[i];
            }

            mpb.SetVectorArray("_ReplaceColors", replaceColourVectors);
            mpb.SetVectorArray("_NewColors", newColourVectors);
            // Set additional colours as needed

            // Apply the block to the tilemap renderer at this specific position
            tilemap.SetTileFlags(localPos, TileFlags.None); // Required to allow shader modifications

            // Apply the MaterialPropertyBlock to the tilemap renderer (at runtime)
            TileRendererExtensions.SetTileMPB(tilemap, localPos, mpb);
        }
    }

    private Color[] CalculateColoursForTile(Vector3Int position)
    {
        // Dummy implementation — replace with your color logic based on noise or other factors

        Vector3Int chunkPosition = ChunkUtility.GetVariableChunkPosition(new Vector2(position.x, position.y));

        float[,] temperatureMap = Noise.GenerateChunkNoiseMapWithBorderFromSettings(chunkPosition, WorldDataTransfer.worldSeed, WorldSettings.Temperature);
        float[,] precipitationMap = Noise.GenerateChunkNoiseMapWithBorderFromSettings(chunkPosition, WorldDataTransfer.worldSeed, WorldSettings.Precipitation);

        float temp = Noise.GenerateCoordinateNoiseFromSettings(position, WorldDataTransfer.worldSeed, WorldSettings.Temperature);
        float precip = Noise.GenerateCoordinateNoiseFromSettings(position, WorldDataTransfer.worldSeed, WorldSettings.Precipitation);

        Texture2D colourMap = TextureUtility.GetColourMapByName("debug");

        Color[] neighbourColours = WorldRenderer.GetTileNeighbourColours(temperatureMap, precipitationMap, position.x, position.y, colourMap, "debug");
        Color[] colours = TextureUtility.Get8WayGradient(neighbourColours);

        return colours;
    }
}

public static class TileRendererExtensions
{
    public static void SetTileMPB(Tilemap tilemap, Vector3Int position, MaterialPropertyBlock mpb)
    {
        // This still impacts the entire renderer.
        tilemap.GetComponent<TilemapRenderer>().SetPropertyBlock(mpb);
    }
}

