using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    private Grid grid;

    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tile highlightTile = null;

    private Vector3Int prevMousePos = new Vector3Int();

    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
    }

    void Update()
    {
        Vector3Int mousePos = grid.WorldToCell(Utility.GetMouseWorldPosition());

        if (!mousePos.Equals(prevMousePos))
        {
            interactiveMap.SetTile(prevMousePos, null);
            interactiveMap.SetTile(mousePos, highlightTile);
            prevMousePos = mousePos;
        }
    }
}
