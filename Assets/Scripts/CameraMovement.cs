using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

/// <summary>
/// This script allows for basic camera movement within a game or application.
/// It enables camera movement both through keyboard input (ZQSD or arrow keys) and by moving the mouse to the edges of the screen.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [Header("--- Parameters ---")]
    [SerializeField] private float _camSpeed = 5f;
    [SerializeField] private float _edgeSize = 100f; // The size of the screen edge that triggers camera movement.
    
    [Header("--- Setup ---")] 
    [SerializeField] private Tilemap _mapReference;

    private Vector2 _maxPos;
    private Vector2 _minPos;
    private bool _hasMapReferenced;

    private void Start()
    {
        ResetPos();
        SetMinMaxCam();
    }

    private void ResetPos()
    {
        // Reset Pos to 0 0
        transform.position = new Vector3(0, 0, -10);
    }

    private void SetMinMaxCam()
    {
        // Set Min and Max from MapReference
        if (_mapReference != null)
        {
            Camera cam = Camera.main;
            // Get the ortho size
            Vector2 camOrthoSize = new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);

            _minPos = new Vector2(_mapReference.origin.x + camOrthoSize.x, _mapReference.origin.y + camOrthoSize.y);
            _maxPos = new Vector2(_mapReference.size.x - Math.Abs(_minPos.x) - camOrthoSize.x * 2,
                _mapReference.size.y - Math.Abs(_minPos.y) - camOrthoSize.y * 2);
            _hasMapReferenced = true;
        }
        else
        {
            _hasMapReferenced = false;
            Debug.LogWarning(
                "In CameraMovement Script, Map Reference not set to define min and max pos that the camera can reach");
        }

        // Set good speed and multiply by .01f for better value in inspector
        _camSpeed *= .01f;
    }

    void Update()
    {
        CheckAndMove();
    }

    private void CheckAndMove()
    {
        Vector3 newPos = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;
        Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0);
        
        // Check if mouse not outside the screen
        if (mousePos.x > screenSize.x || mousePos.x < 0 || mousePos.y > screenSize.y || mousePos.y < 0)
        {
            return;
        }

        // Get Input -> Should be come from InputManager
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        // Apply movement from Inputs
        newPos.x = (mousePos.x >= screenSize.x - _edgeSize || xInput > 0) ? Time.deltaTime :
            (mousePos.x <= _edgeSize || xInput < 0) ? -Time.deltaTime : 0f;

        newPos.y = (mousePos.y >= screenSize.y - _edgeSize || yInput > 0) ? Time.deltaTime :
            (mousePos.y <= _edgeSize || yInput < 0) ? -Time.deltaTime : 0f;

        // Normalize the vector for diagonal movement
        newPos.Normalize();

        // Apply camSpeed
        newPos *= _camSpeed;

        // Clamp the camera to the map if its referenced
        if (_hasMapReferenced)
        {
            float newPosClampX = Mathf.Clamp(transform.position.x + newPos.x, _minPos.x, _maxPos.x);
            float newPosClampY = Mathf.Clamp(transform.position.y + newPos.y, _minPos.y, _maxPos.y);
            
            transform.position = new Vector3(newPosClampX, newPosClampY, transform.position.z);
        }
        else
        {
            transform.position += newPos;
        }
    }
}