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
        //I would like to add weighted texture variations.


         private Color[] originalColours = new Color[16] 
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

                    if(CompareColour(pixels[i], originalColours[j])){
                        pixels[i] = inputColours[j];
                        break;
                    }
                }
            }
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.anisoLevel = 0;
            newTexture.Compress(false);

            return newTexture;
        }
        bool CompareColour(Color colorA, Color colorB)
        {
            float distance = Mathf.Sqrt(
                Mathf.Pow(colorA.r - colorB.r, 2) + 
                Mathf.Pow(colorA.g - colorB.g, 2) + 
                Mathf.Pow(colorA.b - colorB.b, 2) +
                Mathf.Pow(colorA.a - colorB.a, 2)
            );

            return distance < 0.000075f;
        }
        
        public void ApplyGradientToTile(Color[] inputColors)
        {
            if (mainTexture == null)
            {
                Debug.LogError("Tile does not have a sprite.");
                return;
            }

            mainTexture2D = mainTexture.texture;

            Texture2D updatedTexture = ReplaceColors(mainTexture2D, inputColors);

            Sprite updatedSprite = Sprite.Create(
                updatedTexture,
                new Rect(0, 0, updatedTexture.width, updatedTexture.height),
                new Vector2(0.5f, 0.5f),
                16
            );

            mainTexture = updatedSprite;
            Debug.Log("Updated tile sprite.");

        }
    }

}
