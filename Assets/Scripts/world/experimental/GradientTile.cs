using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu(fileName = "New Gradient Tile", menuName = "Tiles/Gradient Tile")]
    public class GradientTile : TileBase
    {
        public int id;
        public Sprite mainTexture;
        private Texture2D mainTexture2D;
        //I would like to add weighted texture variations.


         private Color[] originalColours;

        public void Initialize(Vector3Int position, Color[] inputColours, Sprite inputSprite){
            mainTexture = inputSprite;
            originalColours = ColourLibrary.grassUV;
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

            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.anisoLevel = 0;
            //newTexture.Compress(false);
            newTexture.Apply();

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

            return distance < 0.0001f;
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
