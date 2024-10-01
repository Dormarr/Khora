using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    //Establish the constants
    public const int chunkSize = 32;

    //Global Variables
    public static bool isPaused = false;
}

public static class Gates
{
    public static Gate registryGate = Gate.Open;
    public static Gate biomeClimateRegistryGate = Gate.Open;


}
