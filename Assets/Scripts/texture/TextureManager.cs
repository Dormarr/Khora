using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureManager
{
    // Initialize the textures, register them through a utility class.

    private static Registry<int[]> textureIntRegistry;

    public static void InitializeTextureInts(Registry<int[]> registry){
        textureIntRegistry = registry;

        textureIntRegistry.Register("grass", TextureUtility.ConvertTextureToInts(BiomeUtility.GetGradientTextureByName("grass"), "grass"));
    }

    public static int[] GetTextureInts(string id){
        return textureIntRegistry.Get(id);
    }
}
