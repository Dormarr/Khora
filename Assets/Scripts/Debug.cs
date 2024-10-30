using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.InputSystem;

public class Debug : MonoBehaviour
{
    public Grid grid;
    public ChunkManager chunkManager;
    public WorldEngine worldEngine;

    private Tilemap tilemap;
    private Tilemap childTilemap;
    public TextMeshProUGUI cursorDebugText;
    public TextMeshProUGUI worldGenDebugText;
    public TextMeshProUGUI tickTimeDebugText;
    public GameObject player;

    private Vector2 mousePos;
    private Vector3Int tilePos;

    void Update()
    {
        mousePos = Utility.GetMousePosition();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        try{
            tilemap = chunkManager.GetChunkGameObject().GetComponent<Tilemap>();
            childTilemap = chunkManager.GetChunkGameObject().GetComponentInChildren<Tilemap>();
        }
        catch(Exception e){
            Log($"{e}"); 
        }

        try{
            tilePos = tilemap.WorldToCell(mouseWorldPos);
        }
        catch(Exception e){
            Log($"{e}");
            return;
        }
        
        // TileBase hoveredTile = chunkManager.IdentifyTile(mouseWorldPos);
        Biome biome = worldEngine.GenerateBiomeForCoordinate(tilePos);


        cursorDebugText.text = "<b>Cursor Coordinates</b>" + 
            $"\nGlobal: {tilePos.x}, {tilePos.y}" + 
            $"\n{ChunkPositionDebug()}" + 
            $"\nChunk Cache: {chunkManager.chunkCache.Count}" +
            $"\nMouse Pos: {mousePos.x}, {mousePos.y}";

        tickTimeDebugText.text = "<b>Tick Debug</b>" +
            $"\nCurrent Time: {TickManager.Instance.GetCurrentTick()}" +
            $"\nTick Rate: {TickManager.Instance.GetTickRate()}" +
            $"\nActual Tick Rate: {TickManager.Instance.GetActualTickRate()}" +
            $"\nElapsed Time: {TickManager.Instance.GetActualElapsedTime()}";

        // if(hoveredTile == null){
        //     Debug.Log($"HoveredTile is null.");
        //     return;
        // }

        if(biome == null){
            Debug.Log($"Debug: Biome is null.");
            return;
        }

        worldGenDebugText.text = "<b>Tile Debug</b>" +
            //$"\nTile Identity: {hoveredTile.name}" +
            $"\nBiome: {biome.Name}" +
            $"\n\nTemperature: {worldEngine.temperature}" +
            $"\nHumidity: {worldEngine.precipitation}" +
            $"\nTopology: {worldEngine.GenerateTopologyForCoordinate(tilePos)}" +
            $"\nElevation: {worldEngine.elevation}" +
            $"\nErosion: {worldEngine.erosion}";
            //$"\n\n Feature Info: {biome.FeatureSettings.naturalFeatures[0].type}"; //Not every biome has features.

    }

    string ChunkPositionDebug()
    {
        Vector3Int chunkPos = Utility.GetChunkPosition(tilePos);

        int withinChunkPosX = tilePos.x - (chunkPos.x * Config.chunkSize);
        int withinChunkPosY = tilePos.y - (chunkPos.y * Config.chunkSize);

        // if(withinChunkPosX < 0)
        // {
        //     withinChunkPosX += Config.chunkSize;
        // }
        // if(withinChunkPosY < 0)
        // {
        //     withinChunkPosY += Config.chunkSize;
        // }

        return $"Chunk {withinChunkPosX}, {withinChunkPosY} in {chunkPos.x}, {chunkPos.y}";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        int chunkSize = Config.chunkSize;

        Vector3Int playerChunkPos = BiomeUtility.GetVariableChunkPosition(player.transform.position);

        Vector3 chunkOrigin = new Vector3(playerChunkPos.x * chunkSize, playerChunkPos.y * chunkSize, 0);

        Vector3 bottomLeft = chunkOrigin;
        Vector3 bottomRight = chunkOrigin + new Vector3(chunkSize, 0, 0);
        Vector3 topLeft = chunkOrigin + new Vector3(0, chunkSize, 0);
        Vector3 topRight = chunkOrigin + new Vector3(chunkSize, chunkSize, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }


#region DebugLogs

    public static void Log(string msg){
        UnityEngine.Debug.Log(msg);
    }

    public static void LogWarning(string msg){
        UnityEngine.Debug.LogWarning(msg);
    }

    public static void LogError(string msg){
        UnityEngine.Debug.LogError(msg);
    }

#endregion
}
