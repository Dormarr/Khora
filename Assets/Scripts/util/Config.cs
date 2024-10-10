using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    //Establish the constants
    public const int chunkSize = 32;
    public const int tileSize = 16;

    //Global Variables
    public static bool isPaused = false;
}

public static class Gates
{
    public static Gate registryGate = Gate.Open;
    public static Gate biomeClimateRegistryGate = Gate.Open;


}

public static class ColourLibrary
{
    // list the colours used throughout the project.

    public static Color[] grassUV = new Color[16] 
    {
        new Color(0.9608f, 0.8235f, 0.3882f, 1f),
        new Color(0.4000f, 0.8627f, 0.5608f, 1f),
        new Color(0.4039f, 0.3686f, 0.8549f, 1f),
        new Color(0.9059f, 0.3725f, 0.5647f, 1f),
        new Color(0.8588f, 0.6902f, 0.1451f, 1f),
        new Color(0.2392f, 0.7333f, 0.4157f, 1f),
        new Color(0.2549f, 0.2157f, 0.7059f, 1f),
        new Color(0.7490f, 0.1608f, 0.3765f, 1f),
        new Color(0.6667f, 0.4627f, 0.0667f, 1f),
        new Color(0.1059f, 0.5569f, 0.2627f, 1f),
        new Color(0.1176f, 0.0863f, 0.5098f, 1f),
        new Color(0.5451f, 0.0745f, 0.2471f, 1f),
        new Color(0.5059f, 0.2980f, 0.0275f, 1f),
        new Color(0.0314f, 0.3725f, 0.1529f, 1f),
        new Color(0.0667f, 0.0431f, 0.3412f, 1f),
        new Color(0.4000f, 0.0275f, 0.1608f, 1f)
    };
}