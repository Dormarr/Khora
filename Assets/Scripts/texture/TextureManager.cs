using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureUtility;

public static class TextureManager
{
    // Initialize the textures, register them through a utility class.

    private static Registry<int[]> textureIntRegistry;

    public static void InitializeTextureInts(Registry<int[]> registry){
        textureIntRegistry = registry;

        textureIntRegistry.Register("grass", ConvertTextureToInts(GetGradientTextureByName("grass", "natural"), "grass"));
        textureIntRegistry.Register("debug", ConvertTextureToInts(GetGradientTextureByName("debug", "debug"), "debug"));
    }

    public static int[] GetTextureInts(string id){
        return textureIntRegistry.Get(id);
    }
}