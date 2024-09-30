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

    public Tilemap tilemap;
    public Tilemap childTilemap;
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
            tilemap = chunkManager.GetChunkTilemap().GetComponent<Tilemap>();
            childTilemap = chunkManager.GetChunkTilemap().GetComponentInChildren<Tilemap>();
        }
        catch(Exception e){
            Log($"{e}"); 
        }

        tilePos = tilemap.WorldToCell(mouseWorldPos);
        TileBase hoveredTile = chunkManager.IdentifyTile(mouseWorldPos);
        Biome biome = worldEngine.GenerateBiomeForCoordinate(tilePos);

        cursorDebugText.text = "<b>Cursor Coordinates</b>" + 
            $"\nGlobal: {tilePos.x}, {tilePos.y}" + 
            $"\n{ChunkPositionDebug()}" + 
            $"\nChunk Cache: {chunkManager.chunkCache.Count}" +
            $"\nMouse Pos: {mousePos.x}, {mousePos.y}";
        if(hoveredTile != null)
        {
            worldGenDebugText.text = "<b>Tile Debug</b>" +
                $"\nTile Identity: {hoveredTile.name}" +
                $"\nBiome: {biome.Name}" +
                $"\n\nTemperature: {worldEngine.temperature}" +
                $"\nHumidity: {worldEngine.precipitation}" +
                $"\nTopology: {worldEngine.GenerateTopologyForCoordinate(tilePos)}" +
                $"\nElevation: {worldEngine.elevation}" +
                $"\nErosion: {worldEngine.erosion}" +
                $"\n\n Feature Info: {biome.FeatureSettings.naturalFeatures[0].type}";
        }
        tickTimeDebugText.text = "<b>Tick Debug</b>" +
            $"\nCurrent Time: {TickManager.Instance.GetCurrentTick()}" +
            $"\nTick Rate: {TickManager.Instance.GetTickRate()}" +
            $"\nActual Tick Rate: {TickManager.Instance.GetActualTickRate()}" +
            $"\nElapsed Time: {TickManager.Instance.GetActualElapsedTime()}";
    }

    string ChunkPositionDebug()
    {
        int inChunkX = Mathf.FloorToInt((float)tilePos.x / Config.chunkSize); //fix 0,0 issue
        int inChunkY = Mathf.FloorToInt((float)tilePos.y / Config.chunkSize);

        int chunkPosX = tilePos.x - (inChunkX * Config.chunkSize);
        int chunkPosY = tilePos.y - (inChunkY * Config.chunkSize);

        if(chunkPosX < 0)
        {
            chunkPosX += Config.chunkSize;
        }
        if(chunkPosY < 0)
        {
            chunkPosY += Config.chunkSize;
        }

        return $"Chunk {chunkPosX}, {chunkPosY} in {inChunkX}, {inChunkY}";
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
