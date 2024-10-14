using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureToCSVWindow : EditorWindow
{
    private Texture2D texture;
    private string filePath = "Assets/texture_data.csv";
    
    [MenuItem("Tools/Texture to CSV Converter")]
    public static void ShowWindow()
    {
        GetWindow<TextureToCSVWindow>("Texture to CSV");
    }

    void OnGUI()
    {
        GUILayout.Label("Texture to CSV Converter", EditorStyles.boldLabel);
        
        texture = (Texture2D)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), false);
        
        filePath = EditorGUILayout.TextField("Save CSV Path", filePath);
        
        if (GUILayout.Button("Convert to CSV"))
        {
            if (texture != null)
            {
                ConvertTextureToCSV(texture, filePath);
            }
            else
            {
                Debug.LogError("Please assign a texture first!");
            }
        }
    }

    private void ConvertTextureToCSV(Texture2D texture, string filePath)
    {
        // Convert the texture to a CSV
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write width and height in the first line (optional)
            // writer.WriteLine($"{width},{height}");

            for (int y = 0; y < height; y++)
            {
                string row = "";
                for (int x = 0; x < width; x++)
                {
                    Color pixel = pixels[y * width + x];
                    row += $"{pixel.r},{pixel.g},{pixel.b},{pixel.a}";
                    if (x < width - 1) row += ",";
                }
                writer.WriteLine(row);
            }
        }

        Debug.Log($"CSV saved at {filePath}");
    }
}
