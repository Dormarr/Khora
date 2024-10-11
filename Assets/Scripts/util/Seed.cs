using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System;

public class Seed
{
    private static int seed;
    public static int GetSeed(){
        return seed;
    }

#nullable enable
    public static int GenerateSeed(string? input = null){
        if(input != null && input != ""){
            return GenerateSeedFromString(input);
        }
        return GenerateRandomSeed();
    }
#nullable disable

    private static int GenerateRandomSeed(){
        System.Random random = new System.Random();
        seed = random.Next(0, int.MaxValue);
        Debug.Log($"Generated random seed: {seed}");
        return seed;
    }

    private static int GenerateSeedFromString(string input){
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            seed = BitConverter.ToInt32(hashBytes, 0);

            seed = Mathf.Abs(seed);
            Debug.Log($"Generated seed from string: {seed}");
            return seed;
        }
    }

}
