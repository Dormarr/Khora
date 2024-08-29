using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.InputSystem;

public class Debug : MonoBehaviour
{
    public ChunkManager chunkManager;
    public WorldEngine worldEngine;

    public Tilemap tilemap;
    public TextMeshProUGUI cursorDebugText;
    public TextMeshProUGUI worldGenDebugText;

    private Vector2 mousePos;
    private Vector3Int tilePos;

    void Update()
    {
        if(chunkManager.gate)
        {
            mousePos = Utility.GetMousePosition();
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            tilemap = chunkManager.GetChunkTilemap().GetComponent<Tilemap>();

            tilePos = tilemap.WorldToCell(mouseWorldPos);
            TileBase hoveredTile = chunkManager.IdentifyTile(mouseWorldPos);

            if(hoveredTile != null)
            {
                cursorDebugText.text = "Cursor Coordinates" + 
                    $"\nGlobal: {tilePos.x}, {tilePos.y}" + 
                    $"\n{ChunkPositionDebug()}" + 
                    $"\nChunk Cache: {chunkManager.chunkCache.Count}" +
                    $"\nMouse Pos: {mousePos.x}, {mousePos.y}";
                worldGenDebugText.text = "Tile Debug" +
                    $"\n<b>Biome: {worldEngine.GenerateBiomeForCoordinate(tilePos)}</b>" + //this is wrong and I have no idea why. Will fix later.
                    $"\nTile Identity: {hoveredTile.name}" + //this is accurate.
                    $"\n\nTemperature: {worldEngine.temperature}" +
                    $"\nHumidity: {worldEngine.precipitation}" +
                    $"\n<b>Topology: {worldEngine.GenerateTopologyForCoordinate(tilePos)}</b>" +
                    $"\nElevation: {worldEngine.elevation}" +
                    $"\nErosion: {worldEngine.erosion}";

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

    public void Log(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }
}
