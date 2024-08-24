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
    
    public Tilemap tilemap; //this needs to be dynamically updated based on the current chunk the player is on.
    public TextMeshProUGUI debugText;

    private Vector2 mousePos;
    public InputAction pointPositionAction;

    private Vector3Int tilePos;
    private TileBase hoveredTile; //change this to standing tile at some point.

    void Update()
    {
        if(chunkManager.gate)
        {
            mousePos = pointPositionAction.ReadValue<Vector2>();
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
            //UnityEngine.Debug.Log("Game Object: " + chunkManager.GetChunkTilemap(mousePos).name);
            tilemap = chunkManager.GetChunkTilemap(mouseWorldPos).GetComponent<Tilemap>();


            tilePos = tilemap.WorldToCell(mouseWorldPos);
            hoveredTile = tilemap.GetTile(tilePos);
            //UnityEngine.Debug.Log("Hovered Tile Pos: " + hoveredTile.name);

            if(hoveredTile != null)
            {
                debugText.text = "Coordinates" + 
                    $"\nGlobal: {tilePos.x}, {tilePos.y}" + 
                    $"\n{ChunkPositionDebug()}" + 
                    //$"\nTile Name: {hoveredTile.name}" + 
                    $"\nTile Identity: {chunkManager.IdentifyTile(mouseWorldPos).name}" + 
                    $"\nChunk Cache: {chunkManager.chunkCache.Count}" +
                    $"\nMouse Pos: {mousePos.x}, {mousePos.y}";
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


    void OnEnable()
    {
        pointPositionAction.Enable();
    }

    void OnDisable()
    {
        pointPositionAction.Disable();
    }
}
