using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEngine.Tilemaps
{
    [CreateAssetMenu(fileName = "New Shader Tile", menuName = "Tiles/Shader Tile")]
    public class ShaderTile : TileBase
    {
        public Sprite mainTexture;
        public Texture2D colourMap;
        private Color[] originalColours;

    }
}
