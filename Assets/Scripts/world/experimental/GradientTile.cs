using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu(fileName = "New Gradient Tile", menuName = "Tiles/Gradient Tile")]
    public class GradientTile : TileBase
    {
        public int id;
        public Color tint;
        public Sprite mainTexture;
        private Texture2D mainTexture2D;


         private Color[] originalColours = new Color[16] 
        {
            new Color(0.9607f, 0.8235f, 0.3885f, 1),
            new Color(0.8588f, 0.6902f, 0.1451f, 1),
            new Color(0.6667f, 0.4627f, 0.0667f, 1),
            new Color(0.5059f, 0.2980f, 0.0275f, 1),
            new Color(0.4000f, 0.8627f, 0.5608f, 1),
            new Color(0.2392f, 0.7333f, 0.4157f, 1),
            new Color(0.1059f, 0.5569f, 0.2627f, 1),
            new Color(0.0314f, 0.3725f, 0.1529f, 1),
            new Color(0.4039f, 0.3686f, 0.8549f, 1),
            new Color(0.2549f, 0.2157f, 0.7059f, 1),
            new Color(0.1176f, 0.0863f, 0.5098f, 1),
            new Color(0.0667f, 0.0431f, 0.3412f, 1),
            new Color(0.9059f, 0.3725f, 0.5647f, 1),
            new Color(0.7490f, 0.1608f, 0.3765f, 1),
            new Color(0.5451f, 0.0745f, 0.2471f, 1),
            new Color(0.4000f, 0.0275f, 0.1608f, 1)
        };

        public void Initialize(Vector3Int position, Color[] inputColours, Sprite inputSprite){
            mainTexture = inputSprite;
            ApplyGradientToTile(inputColours);
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var iden = Matrix4x4.identity;
         
            tileData.sprite = mainTexture;
            tileData.color = Color.white; 
            tileData.transform = iden;

            Matrix4x4 transform = iden;
        }

        public void SetTileColors(Color[] newColours){
            originalColours = newColours;
        }

        // Maybe I can colour replace in here?

        public Texture2D ReplaceColors(Texture2D originalTexture, Color[] inputColours)
        {
            if (inputColours.Length != 16)
            {
                Debug.LogError("Input color array must have exactly 16 colors.");
                return null;
            }

            // Create a new texture to store the modified pixels
            Texture2D newTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
            newTexture.SetPixels(originalTexture.GetPixels());
            newTexture.Apply();

            // Get all pixels from the original texture
            Color[] pixels = newTexture.GetPixels();

            // Loop through each pixel and replace matching original colors with input colors
            for (int i = 0; i < pixels.Length; i++)
            {
                for (int j = 0; j < originalColours.Length; j++)
                {
                    // if (pixels[i] == originalColours[j])
                    // {
                    //     Replace the original color with the corresponding input color
                    //     pixels[i] = inputColours[j];
                    //     pixels[i] = Color.red;
                    //     break;
                    // }

                    if(CompareColour(pixels[i], originalColours[j])){
                        pixels[i] = inputColours[j];
                        break;
                    }
                }
            }

            // Apply the modified pixels to the new texture
            newTexture.SetPixels(pixels);
            newTexture.Apply(); // Make sure changes are applied to the texture

            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp; // Avoid edge artifacts
            newTexture.anisoLevel = 0; // Disable anisotropic filtering
            newTexture.Compress(false); // Disable compression

            return newTexture;
        }
        bool CompareColour(Color colorA, Color colorB)
        {
            // Calculate the Euclidean distance between two colors
            float distance = Mathf.Sqrt(
                Mathf.Pow(colorA.r - colorB.r, 2) + 
                Mathf.Pow(colorA.g - colorB.g, 2) + 
                Mathf.Pow(colorA.b - colorB.b, 2) +
                Mathf.Pow(colorA.a - colorB.a, 2)
            );

            return distance < 0.01f;
        }
        
        public void ApplyGradientToTile(Color[] inputColors)
        {
            if (mainTexture == null)
            {
                Debug.LogError("Tile does not have a sprite.");
                return;
            }

            mainTexture2D = mainTexture.texture;

            // Replace the colors in the texture
            Texture2D updatedTexture = ReplaceColors(mainTexture2D, inputColors);

            // Create a new sprite from the updated texture
            Sprite updatedSprite = Sprite.Create(
                updatedTexture,
                new Rect(0, 0, updatedTexture.width, updatedTexture.height),
                new Vector2(0.5f, 0.5f), // pivot in the center
                16
            );

            // Assign the updated sprite back to the tile
            mainTexture = updatedSprite;
            // mainTexture.texture = updatedTexture;
            Debug.Log("Updated tile sprite.");

        }
    }

}
