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
    public TextMeshProUGUI tickTimeDebugText;

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
                    //$"\nBiome: {worldEngine.GenerateBiomeForCoordinate(tilePos)}" + //redo with updated methods.
                    $"\nTile Identity: {hoveredTile.name}" +
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

    public static void Log(string msg)
    {
        UnityEngine.Debug.Log(msg);
    }
}
