using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public TileBase blankTile;
    public float moveSpeed = 5f;
    public ChunkManager chunkManager;
    public Grid grid;

    [Header("Camera")]
    public CinemachineVirtualCamera vCam;
    public float zoomSpeed = 1f;
    public float minZoom = 30f;
    public float maxZoom = 5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 moveVelocity = moveInput * moveSpeed;
        rb.velocity = moveVelocity;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnZoom(InputValue value)
    {
        float currentZoom = vCam.m_Lens.OrthographicSize;
        Vector2 scrollValue = value.Get<Vector2>();
        float zoomDelta = (scrollValue.y / 120) * zoomSpeed;

        float newZoom = currentZoom - zoomDelta;

        vCam.m_Lens.OrthographicSize = Mathf.Clamp(newZoom, maxZoom, minZoom);
    }

    void OnMouseDown(InputValue value){

        if(Config.isPaused){
            return;
        }

        GameObject targetChunk = chunkManager.GetChunkGameObject(); //grabs by mouse position.
        targetChunk = targetChunk.transform.GetChild(0).gameObject;
        Vector3Int mouseGridPos = grid.WorldToCell(Utility.GetMouseWorldPosition());

        //CHECK INVENTORY
        //If tiles, check if valid placement before placing.
        //If tool, check if valid action before acting, else do nothing.

        chunkManager.modificationCache.Add(new TileData(mouseGridPos.x, mouseGridPos.y, blankTile.name));
        targetChunk.GetComponentInChildren<Tilemap>().SetTile(mouseGridPos, blankTile);        
    }

    void OnEsc(){
        if(Config.isPaused){
            MenuUtility.Resume();
        }
        else{
            MenuUtility.Pause();
        }
    }



}
