using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEditor;

public static class TextureUtility
{


    public static Color[] LerpBetweenColours(Color colourA, Color colourB, int steps)
    {
        //return an array the size of steps, consisting of colours equal between colourA and colourB.

        return null;
    }

    public static Color[] Get4WayGradient(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
    {
        Color[] colours = new Color[16];

        for(int i = 0; i < 4; i++){
            float xLerp = i / 3.0f;
            Color leftLerp = Color.Lerp(topLeft, bottomLeft, xLerp);
            Color rightLerp = Color.Lerp(topRight, bottomRight, xLerp);

            for(int j = 0; j < 4; j++){
                float yLerp = j / 3.0f;
                colours[i * 4 + j] = Color.Lerp(leftLerp, rightLerp, yLerp);
            }
        }

        return colours;
    }

    public static async Task<Color[]> Get8WayGradientAsync(Color[] neighbourColours)
    {
        // Color array structure
        // [ 0, 1, 2 ]
        // [ 3, 4, 5 ]
        // [ 6, 7, 8 ]
        
        Color topLeft = neighbourColours[0];
        Color top = neighbourColours[1];
        Color topRight = neighbourColours[2];
        Color left = neighbourColours[3];
        Color center = neighbourColours[4]; // The tile's own color
        Color right = neighbourColours[5];
        Color bottomLeft = neighbourColours[6];
        Color bottom = neighbourColours[7];
        Color bottomRight = neighbourColours[8];

        // We will create a 4x4 grid to smoothly blend colors across the tile

        Color[] gradientColours = new Color[16];

        // [ 0,  1,  2,  3, ]
        // [ 4,  5,  6,  7, ]
        // [ 8,  9,  10, 11,]
        // [ 12, 13, 14, 15 ]


        await Task.Run(() => {

            // Corners
            gradientColours[0]  =   Color.Lerp(top, left, 0.5f);
            gradientColours[3]  =   Color.Lerp(top, right, 0.5f);
            gradientColours[15] =   Color.Lerp(bottom, right, 0.5f);
            gradientColours[12] =   Color.Lerp(bottom, left, 0.5f);

            //Edges
            gradientColours[1]  =   Color.Lerp(center, top, 0.4f);
            gradientColours[2]  =   Color.Lerp(center, top, 0.4f);
            gradientColours[4]  =   Color.Lerp(center, left, 0.4f);
            gradientColours[8]  =   Color.Lerp(center, left, 0.4f);
            gradientColours[7]  =   Color.Lerp(center, right, 0.6f);
            gradientColours[11] =   Color.Lerp(center, right, 0.6f);
            gradientColours[13] =   Color.Lerp(center, bottom, 0.6f);
            gradientColours[14] =   Color.Lerp(center, bottom, 0.6f);
            
            //Center
            gradientColours[5]  =   ApplyNoiseToColour(Color.Lerp(center, gradientColours[1], 0.5f), 0.025f);
            gradientColours[10] =   ApplyNoiseToColour(Color.Lerp(center, gradientColours[14], 0.5f), 0.025f);
            gradientColours[9]  =   ApplyNoiseToColour(center, 0.02f);
            gradientColours[6]  =   ApplyNoiseToColour(center, 0.02f);
        });

        return gradientColours;
    }

    public static Color[] Get8WayGradient(Color[] neighbourColours)
    {
        // Color array structure
        // [ 0, 1, 2 ]
        // [ 3, 4, 5 ]
        // [ 6, 7, 8 ]
        
        Color topLeft = neighbourColours[0];
        Color top = neighbourColours[1];
        Color topRight = neighbourColours[2];
        Color left = neighbourColours[3];
        Color center = neighbourColours[4]; // The tile's own color
        Color right = neighbourColours[5];
        Color bottomLeft = neighbourColours[6];
        Color bottom = neighbourColours[7];
        Color bottomRight = neighbourColours[8];

        // We will create a 4x4 grid to smoothly blend colors across the tile
        Color[] gradientColours = new Color[16];


        // [ 0,  1,  2,  3, ]
        // [ 4,  5,  6,  7, ]
        // [ 8,  9,  10, 11,]
        // [ 12, 13, 14, 15 ]

        // Corners
        gradientColours[0]  =   Color.Lerp(top, left, 0.5f);
        gradientColours[3]  =   Color.Lerp(top, right, 0.5f);
        gradientColours[15] =   Color.Lerp(bottom, right, 0.5f);
        gradientColours[12] =   Color.Lerp(bottom, left, 0.5f);

        //Edges
        gradientColours[1]  =   Color.Lerp(center, top, 0.4f);
        gradientColours[2]  =   Color.Lerp(center, top, 0.4f);
        gradientColours[4]  =   Color.Lerp(center, left, 0.4f);
        gradientColours[8]  =   Color.Lerp(center, left, 0.4f);
        gradientColours[7]  =   Color.Lerp(center, right, 0.6f);
        gradientColours[11] =   Color.Lerp(center, right, 0.6f);
        gradientColours[13] =   Color.Lerp(center, bottom, 0.6f);
        gradientColours[14] =   Color.Lerp(center, bottom, 0.6f);
        
        //Center
        gradientColours[5]  =   ApplyNoiseToColour(Color.Lerp(center, gradientColours[1], 0.5f), 0.025f);
        gradientColours[10] =   ApplyNoiseToColour(Color.Lerp(center, gradientColours[14], 0.5f), 0.025f);
        gradientColours[9]  =   ApplyNoiseToColour(center, 0.02f);
        gradientColours[6]  =   ApplyNoiseToColour(center, 0.02f);

        return gradientColours;
    }

    public static Color ApplyRandomHueShift(Color colour){
        float hue, saturation, brightness;
        Color.RGBToHSV(colour, out hue, out saturation, out brightness);

        // Id'd rather this wasn't random, but it's inconsequential so whatever. I'll revisit it.
        hue += UnityEngine.Random.Range(-0.02f, 0.02f);
        hue = Mathf.Repeat(hue, 1.0f);

        return Color.HSVToRGB(hue, saturation, brightness);
    }

    public static Color ApplyNoiseToColour(Color color, float noiseIntensity)
    {
        // Id'd rather this wasn't random, but it's inconsequential so whatever. I'll revisit it.
        

        System.Random random = new System.Random();

        float noise = (float)(random.NextDouble() * 2.0 - 1.0) * noiseIntensity;

        // Convert the colour to HSV to modify brightness (Value)
        float hue, saturation, brightness;
        Color.RGBToHSV(color, out hue, out saturation, out brightness);

        // Apply noise to brightness
        brightness += noise;
        brightness = Mathf.Clamp(brightness, 0.0f, 1.0f); // Ensure brightness is within valid range

        // Return the colour with applied noise
        return Color.HSVToRGB(hue, saturation, brightness);
    }

    public static Color[,] LoadCSVAsColourArray(TextAsset csvFile, int width, int height){
        
        
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        Color[,] colourArray = new Color[width, height];

        for(int y = 0; y < height; y++){

            string[] pixelValues = lines[y].Split(',');

            if(pixelValues.Length != width * 4){
                Debug.LogError($"Unexpected number of pixel values at line {y}. Expected {width * 4}, but got {pixelValues.Length}.");
            }

            for(int x = 0; x < width; x++){

                try
                {
                    // Parse the RGBA values
                    float r = float.Parse(pixelValues[x * 4]);
                    float g = float.Parse(pixelValues[x * 4 + 1]);
                    float b = float.Parse(pixelValues[x * 4 + 2]);
                    float a = float.Parse(pixelValues[x * 4 + 3]);

                    // Assign the color to the array
                    colourArray[x, y] = new Color(r, g, b, a);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing colour at ({x}, {y}): {e.Message}");
                }
            }
        }

        Debug.Log("Loaded CSV as Colour Array.");
        return colourArray;
    }

    public static string[] LoadPixelValues(TextAsset csvFile, int size){

        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        string[] pixelValues = new string[size * size * 4]; // We multiply by 4 since each pixel has 4 values (RGBA).

        int index = 0; // This will track where to insert the next values in the array.

        for (int y = 0; y < size; y++)
        {
            string[] lineValues = lines[y].Split(',');

            if (lineValues.Length != size * 4)
            {
                Debug.LogError($"Unexpected number of pixel values at line {y}. Expected {size * 4}, but got {lineValues.Length}.");
                return null; // If the line doesn't have the correct number of values, we return null to indicate an error.
            }

            // Copy the values from the current line into the correct position in the pixelValues array.
            for (int i = 0; i < lineValues.Length; i++)
            {
                pixelValues[index] = lineValues[i];
                index++; // Move to the next position.
            }
        }

        return pixelValues;
    }

    public static Color[,] LoadColourArrayFromPixelValues(string[] pixelValues, int size){

        Color[,] colourArray = new Color[size, size];
        for(int x = 0; x < size; x++){
            for(int y = 0; y < size; y++)
            {
                try
                {
                    // Parse the RGBA values
                    float r = float.Parse(pixelValues[x * 4]);
                    float g = float.Parse(pixelValues[x * 4 + 1]);
                    float b = float.Parse(pixelValues[x * 4 + 2]);
                    float a = float.Parse(pixelValues[x * 4 + 3]);

                    // Assign the color to the array
                    colourArray[x, y] = new Color(r, g, b, a);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing colour at ({x}, {y}): {e.Message}");
                }
            }
        }

        Debug.Log("Returned Colour Array from Pixel Values.");
        return colourArray;
    }

    public static Color[] Convert2DTo1D(Color[,] csvColours){
        int width = csvColours.GetLength(0);
        int height = csvColours.GetLength(1);

        Color[] flatColours = new Color[width * height];

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                flatColours[y * width + x] = csvColours[x,y];
            }
        }

        return flatColours;
    }

    public static int[] ConvertTextureToInts(Texture2D texture, string tileName){
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        // Loop through the textures pixels and replace using...
        // a switch statement 1-16 to match each colour.

        int[] pixelIndices = new int[width*height];

        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                // Compare the colour from pixels[x + y * width] to the colours in a colourlibrary array.
                // Then add the index of the colourlibrary array to an int array.

                Color pixelColour = pixels[x + y * width];

                int matchedIndex = FindMatchingColourIndex(pixelColour, ColourLibrary.Get(tileName));

                if(matchedIndex != -1){
                    pixelIndices[x + y * width] = matchedIndex;
                }
                else{
                    Debug.LogWarning($"No matching colour found for pixel at ({x}, {y})");
                }
            }
        }

        return pixelIndices;
    }

    public static int FindMatchingColourIndex(Color colour, Color[] uvColours){
        
        for(int i = 0; i < uvColours.Length; i++){
            if(CompareColours(colour, uvColours[i])){
                return i;
            }
        }
        return -1;
    }

    public static bool CompareColours(Color colorA, Color colorB)
    {
        float sqrDistance = 
            Mathf.Pow(colorA.r - colorB.r, 2) + 
            Mathf.Pow(colorA.g - colorB.g, 2) + 
            Mathf.Pow(colorA.b - colorB.b, 2) +
            Mathf.Pow(colorA.a - colorB.a, 2);

        return sqrDistance < 0.0001f * 0.0001f;
    }

    public static TileBase GetTestSpriteByInt(int i){

        string path = $"Assets/textures/test/tiles/tileAssets/{i}.asset";

        if(!File.Exists(path)){
            Debug.LogError($"Tile {i} could not be found.");
            return null;
        }

        TileBase tile= AssetDatabase.LoadAssetAtPath<TileBase>(path);
        return tile;
    }
}
