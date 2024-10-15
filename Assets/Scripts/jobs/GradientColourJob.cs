using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public struct GradientColourJob : IJob
{
    [ReadOnly] public NativeArray<Color> neighbourColours;
    public NativeArray<Color> gradientColours;

    public void Execute()
    {
        // Neighbor colors
        Color topLeft = neighbourColours[0];
        Color top = neighbourColours[1];
        Color topRight = neighbourColours[2];
        Color left = neighbourColours[3];
        Color center = neighbourColours[4]; // The tile's own color
        Color right = neighbourColours[5];
        Color bottomLeft = neighbourColours[6];
        Color bottom = neighbourColours[7];
        Color bottomRight = neighbourColours[8];

        // Corners
        gradientColours[0]  = Color.Lerp(top, left, 0.5f);
        gradientColours[3]  = Color.Lerp(top, right, 0.5f);
        gradientColours[15] = Color.Lerp(bottom, right, 0.5f);
        gradientColours[12] = Color.Lerp(bottom, left, 0.5f);

        // Edges
        gradientColours[1]  = Color.Lerp(center, top, 0.4f);
        gradientColours[2]  = Color.Lerp(center, top, 0.4f);
        gradientColours[4]  = Color.Lerp(center, left, 0.4f);
        gradientColours[8]  = Color.Lerp(center, left, 0.4f);
        gradientColours[7]  = Color.Lerp(center, right, 0.6f);
        gradientColours[11] = Color.Lerp(center, right, 0.6f);
        gradientColours[13] = Color.Lerp(center, bottom, 0.6f);
        gradientColours[14] = Color.Lerp(center, bottom, 0.6f);

        // Center
        gradientColours[5]  = ApplyNoiseToColour(Color.Lerp(center, gradientColours[1], 0.5f), 0.025f);
        gradientColours[10] = ApplyNoiseToColour(Color.Lerp(center, gradientColours[14], 0.5f), 0.025f);
        gradientColours[9]  = ApplyNoiseToColour(center, 0.02f);
        gradientColours[6]  = ApplyNoiseToColour(center, 0.02f);
    }

    private Color ApplyNoiseToColour(Color baseColor, float noiseAmount)
    {
        System.Random random = new System.Random();

        float noise = (float)(random.NextDouble() * 2.0 - 1.0) * noiseAmount;

        // Convert the colour to HSV to modify brightness (Value)
        float hue, saturation, brightness;
        Color.RGBToHSV(baseColor, out hue, out saturation, out brightness);

        // Apply noise to brightness
        brightness += noise;
        brightness = Mathf.Clamp(brightness, 0.0f, 1.0f); // Ensure brightness is within valid range

        // Return the colour with applied noise
        return Color.HSVToRGB(hue, saturation, brightness);
    }
}
