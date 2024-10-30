using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class Config
{
    //Establish the constants
    public const string version = "0.2.3";
    public const int chunkSize = 32;
    public const int regionSize = 32;
    public const int tileSize = 16;
    public const int maxChunks = 8; // Get rid

    //Global Variables
    public static bool isPaused = false;
}

public static class WorldSettings{
    public static PerlinSettings Temperature = new PerlinSettings(Config.chunkSize, 100, 4, 0.25f, 4, new Vector2(0,0));
    public static PerlinSettings Precipitation = new PerlinSettings(Config.chunkSize, 250, 8, 0.2f, 4, new Vector2(0,0));
    public static PerlinSettings Elevation = new PerlinSettings(Config.chunkSize, 30, 4, 0f, 2, new Vector2(0,0));
    public static PerlinSettings Erosion = new PerlinSettings(Config.chunkSize, 45, 4, 0f, 4, new Vector2(0,0));
}

public static class Gates
{
    public static Gate registryGate = Gate.Open;
    public static Gate biomeClimateRegistryGate = Gate.Open;


}