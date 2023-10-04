using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This script allows for basic camera movement within a game or application.
/// It enables camera movement both through keyboard input (ZQSD or arrow keys) and by moving the mouse to the edges of the screen.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _camSpeed = 5f; 
    [SerializeField] private float _edgeSize = 20f; // The size of the screen edge that triggers camera movement.

    void Update()
    {
       CheckAndMove();
    }

    private void CheckAndMove()
    {
        Vector3 newPos = Vector3.zero; 
        Vector3 mousePos = Input.mousePosition; 
        Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0); 

        float xInput = Input.GetAxisRaw("Horizontal"); 
        float yInput = Input.GetAxisRaw("Vertical"); 

        if (mousePos.x >= screenSize.x - _edgeSize || xInput > 0)
        {
            newPos.x = _camSpeed * Time.deltaTime; 
        }

        else if (mousePos.x <= _edgeSize || xInput < 0)
        {
            newPos.x = -_camSpeed * Time.deltaTime; 
        }

        if (mousePos.y >= screenSize.y - _edgeSize || yInput > 0)
        {
            newPos.y = _camSpeed * Time.deltaTime; 
        }

        else if (mousePos.y <= _edgeSize || yInput < 0)
        {
            newPos.y = -_camSpeed * Time.deltaTime; 
        }

        transform.position += newPos;
    }
}

