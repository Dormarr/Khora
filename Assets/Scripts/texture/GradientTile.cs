using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEditor;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu(fileName = "New Gradient Tile", menuName = "Tiles/Gradient Tile")]
    public class GradientTile : TileBase
    {
        public int id;
        public Sprite mainTexture;
        public Texture2D mainTexture2D;
        public Texture2D colourMap;
        //I would like to add weighted texture variations.
        private Color[] originalColours;


        public void Initialize(Vector3Int position, Color[] inputColours, Sprite inputSprite)
        {
            Debug.Log("Initialize: Starting initialization.");

            // Assign input sprite to mainTexture
            mainTexture = inputSprite;

            // Ensure mainTexture and colours are valid

            if (mainTexture == null)
            {
                Debug.LogError("Initialize: inputSprite is null.");
                return;
            }

            if (inputColours == null || inputColours.Length == 0)
            {
                Debug.LogError("Initialize: inputColours are invalid.");
                return;
            }

            mainTexture = ApplyGradientToTile(inputColours);

            // Check if the texture was successfully updated
            if (mainTexture == null)
            {
                Debug.LogError("Initialize: Failed to apply gradient to tile.");
                return;
            }

            Debug.Log("Initialize: Completed initialization successfully.");
        }
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var iden = Matrix4x4.identity;
         
            tileData.sprite = mainTexture;
            tileData.color = Color.white; 
            tileData.transform = iden;

            Matrix4x4 transform = iden;
        }

        public void SetTileColours(Color[] newColours){
            originalColours = newColours;
        }

        public Sprite ApplyGradientToTile(Color[] inputColours)
        {
            Debug.Log("ApplyGradientToTile: Processing Started.");
            if (mainTexture == null)
            {
                Debug.LogError("Tile does not have a valid texture.");
                return null;
            }

            Texture2D updatedTexture = ReplaceColours(inputColours);

            Sprite updatedSprite = Sprite.Create(updatedTexture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            

            // mainTexture = updatedSprite;
            Debug.Log("Finished Rendering Tile.");
            return updatedSprite;
        }

        public Texture2D ReplaceColours(Color[] inputColours)
        {
            Debug.Log("ReplaceColours: Processing Started.");
            if (inputColours.Length != 16)
            {
                Debug.LogError("Input color array must have exactly 16 colors.");
                return null;
            }

            // Color[] pixels = CycleColours(inputColours, Config.tileSize);

            // ---------------------------------------------------------------------
            string name = "grass";
            int[] uvInts = TextureManager.GetTextureInts(name);
            Color[] pixels = CycleColoursJob(inputColours, uvInts, Config.tileSize);
            // ---------------------------------------------------------------------

            Debug.Log("ReplaceColours: Started Creating NewTexture.");

            Texture2D newTexture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            newTexture.SetPixels(pixels);
            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.anisoLevel = 0;
            newTexture.Compress(true);
            // newTexture.Apply(updateMipmaps: true);
            newTexture.Apply();

            Debug.Log("ReplaceColours: Created NewTexture.");

            return newTexture;
        }

        public Color[] CycleColours(Color[] inputColours, int tileSize)
        {
            string name = "grass";
            Debug.Log($"CycleColours: Started Generated Colour Array for {name}.");            

            int[] uvInts = TextureManager.GetTextureInts(name);
            Color[] colours = new Color[tileSize * tileSize];

            // Cache the input colours length to avoid repeated length checks
            int inputLength = inputColours.Length;

            for (int i = 0; i < tileSize * tileSize; i++)
            {
                int uvIndex = uvInts[i];
                
                // Handle out-of-range cases
                if (uvIndex >= 0 && uvIndex < inputLength)
                {
                    colours[i] = inputColours[uvIndex];
                }
                else if (uvIndex == -1)
                {
                    colours[i] = Color.blue;
                }
                else
                {
                    colours[i] = Color.red;
                }
            }

            return colours;
        }

        public Color[] CycleColoursJob(Color[] inputColours, int[] uvInts, int tileSize){
            NativeArray<int> uvIntsNative = new NativeArray<int>(uvInts, Allocator.TempJob);
            NativeArray<Color> inputColoursNative = new NativeArray<Color>(inputColours, Allocator.TempJob);
            NativeArray<Color> outputColoursNative = new NativeArray<Color>(tileSize * tileSize, Allocator.TempJob);

            ColourCyclingJob job = new ColourCyclingJob{
                uvInts = uvIntsNative,
                inputColours = inputColoursNative,
                outputColours = outputColoursNative
            };

            JobHandle jobHandle = job.Schedule(tileSize * tileSize, 64);
            jobHandle.Complete();

            Color[] outputColours = outputColoursNative.ToArray();

            uvIntsNative.Dispose();
            inputColoursNative.Dispose();
            outputColoursNative.Dispose();

            return outputColours;
        }
    }
}

[CustomEditor(typeof(GradientTile))]
public class GradientTileEditor : Editor{
    public override void OnInspectorGUI(){
        GradientTile gradientTile = (GradientTile)target;

        DrawDefaultInspector();

        // if(GUILayout.Button("Process CSV.")){
        //     gradientTile.csvColourData = TextureUtility.LoadCSVAsColourArray(gradientTile.csvFile, 16, 16);
        //     Debug.Log("Successfully processed CSV data to colour array.");

        //     gradientTile.csvStrings = TextureUtility.LoadPixelValues(gradientTile.csvFile, 16);

        // }
    }
}