using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.Security.Cryptography;
using System;

public static class Utility
{
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

    public static int GenerateWorldSeedFromString(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            int seed = BitConverter.ToInt32(hashBytes, 0);

            seed = Mathf.Abs(seed);

            return seed;
        }
    }
}
