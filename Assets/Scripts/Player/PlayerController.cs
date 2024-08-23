using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

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


        //float newZoom = Mathf.Clamp(currentZoom + zoomDelta, minZoom, maxZoom);

        float newZoom = currentZoom - zoomDelta;

        vCam.m_Lens.OrthographicSize = Mathf.Clamp(newZoom, maxZoom, minZoom);
    }

}
