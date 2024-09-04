using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.Security.Cryptography;
using System;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor;

public static class Utility
{
    //I'll separate these into more appropriate scripts at some other point.
    public static InputAction mousePosition;

    //Get raw mouse input.
    public static Vector2 GetMousePosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        return mousePosition;
    }

    //Get mouse position in relation to the tilemap/world.
    public static Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        return mouseWorldPos;
    }

    //Get chunk coordinates of a Vector2. Mostly used for managing chunks and debugging.
    public static Vector3Int GetVariableChunkPosition(Vector2 focus)
    {
        return new Vector3Int(Mathf.FloorToInt(focus.x / Config.chunkSize), Mathf.FloorToInt(focus.y / Config.chunkSize), 0);
    }

    public static TileBase ConvertTexture2DToTile(Texture2D texture){
        Sprite sprite = Sprite.Create(texture, new Rect(0,0,texture.width, texture.height), new Vector2(0.5f, 0.5f), 12);

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;

        return tile;
    }

    public static Texture2D LoadTexture2DFromPath(string path){
        if(File.Exists(path)){
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2,2);
            if(texture.LoadImage(fileData)){
                return texture;
            }
        }
        throw new Exception($"File path invalid: '{path}'");
    }

    public static TileBase GetTileFromBiome(Biome biome)
    {
        TileBase tile;
        string tilePath = "Assets/textures/world/tiles/tilesObjects/" + biome.Name + ".asset";
        if(!File.Exists(tilePath)){
            string texturePath = "Assets/textures/world/tiles/" + biome.Name + ".png";
            Texture2D texture = LoadTexture2DFromPath(texturePath);
            tile = ConvertTexture2DToTile(texture);
            Debug.Log("YUCKY TILE GEN USED: " + biome.Name);
            //handle null or save the tile.
        }else{
            tile = AssetDatabase.LoadAssetAtPath<TileBase>(tilePath);
            Debug.Log("NICE TILE GEN USED: " + biome.Name);
        }
        
        if(tile == null){
            Debug.Log($"TileBase not found at path: {tilePath}");
        }

        return tile;
    }
}
