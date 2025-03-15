using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    public float newHorizontalOffset = 2f;
    public float newVerticalOffset = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Camera2D cameraScript = FindObjectOfType<Camera2D>();
            if (cameraScript != null)
            {
                //cameraScript.UpdateCameraOffset(newHorizontalOffset, newVerticalOffset);
            }
        }
    }
}
