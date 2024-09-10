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
    public TextMeshProUGUI cursorDebugText;
    public TextMeshProUGUI worldGenDebugText;
    public TextMeshProUGUI tickTimeDebugText;
    public GameObject player;

    private Vector2 mousePos;
    private Vector3Int tilePos;

    void Update()
    {
        if(chunkManager.gate)
        {
            mousePos = Utility.GetMousePosition();
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            try{
                tilemap = chunkManager.GetChunkTilemap().GetComponent<Tilemap>();
            }
            catch(Exception e){
               Log($"{e}"); 
            }

            tilePos = tilemap.WorldToCell(mouseWorldPos);
            TileBase hoveredTile = chunkManager.IdentifyTile(mouseWorldPos);

            if(hoveredTile != null)
            {
                cursorDebugText.text = "<b>Cursor Coordinates</b>" + 
                    $"\nGlobal: {tilePos.x}, {tilePos.y}" + 
                    $"\n{ChunkPositionDebug()}" + 
                    $"\nChunk Cache: {chunkManager.chunkCache.Count}" +
                    $"\nMouse Pos: {mousePos.x}, {mousePos.y}";
                worldGenDebugText.text = "<b>Tile Debug</b>" +
                    $"\nTile Identity: {hoveredTile.name}" +
                    //$"\nBiome: {worldEngine.GenerateBiomeForCoordinate(tilePos)}" + //redo with updated methods.
                    $"\n\nTemperature: {worldEngine.temperature}" +
                    $"\nHumidity: {worldEngine.precipitation}" +
                    $"\nTopology: {worldEngine.GenerateTopologyForCoordinate(tilePos)}" +
                    $"\nElevation: {worldEngine.elevation}" +
                    $"\nErosion: {worldEngine.erosion}";
                tickTimeDebugText.text = "<b>Tick Debug</b>" +
                    $"\nCurrent Time: {TickManager.Instance.GetCurrentTick()}" +
                    $"\nTick Rate: {TickManager.Instance.GetTickRate()}" +
                    $"\nActual Tick Rate: {TickManager.Instance.GetActualTickRate()}" +
                    $"\nElapsed Time: {TickManager.Instance.GetActualElapsedTime()}";

            }
        }
    }

    string ChunkPositionDebug()
    {
        int inChunkX = Mathf.FloorToInt((float)tilePos.x / Config.chunkSize); //fix 0,0 issue
        int inChunkY = Mathf.FloorToInt((float)tilePos.y / Config.chunkSize);

        int chunkPosX = tilePos.x - (inChunkX * Config.chunkSize);
        int chunkPosY = tilePos.y - (inChunkY * Config.chunkSize);

        //can I normalize * 2 - 1? And subtract the negative of the tilePos. Then absolute it, the negative will be positive after double neg, and positive will go through absolution.

        //find a mathematical way to avoid if statements.
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

    // Assuming chunkSize is 32 units
    int chunkSize = 32;

    // Get the player's current chunk position
    Vector3Int playerChunkPos = BiomeUtility.GetVariableChunkPosition(player.transform.position);

    // Calculate the bottom-left corner of the current chunk
    Vector3 chunkOrigin = new Vector3(playerChunkPos.x * chunkSize, playerChunkPos.y * chunkSize, 0);

    // Calculate the four corners of the chunk
    Vector3 bottomLeft = chunkOrigin;
    Vector3 bottomRight = chunkOrigin + new Vector3(chunkSize, 0, 0);
    Vector3 topLeft = chunkOrigin + new Vector3(0, chunkSize, 0);
    Vector3 topRight = chunkOrigin + new Vector3(chunkSize, chunkSize, 0);

    // Draw the four sides of the chunk boundary
    Gizmos.DrawLine(bottomLeft, bottomRight);  // Bottom
    Gizmos.DrawLine(bottomRight, topRight);    // Right
    Gizmos.DrawLine(topRight, topLeft);        // Top
    Gizmos.DrawLine(topLeft, bottomLeft);      // Left
}


#region DebugLogs

    public static void Log(string msg)
    {
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
