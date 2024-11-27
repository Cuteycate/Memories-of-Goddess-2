using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;
    Camera uiCamera; // The camera rendering the UI
    Canvas parentCanvas; // The parent canvas

    void Start()
    {
        rect = GetComponent<RectTransform>();
        uiCamera = Camera.main; // Assign your UI camera here if it's not the main camera
        parentCanvas = GetComponentInParent<Canvas>(); // Get the parent canvas
    }

    void FixedUpdate()
    {
        // Convert the player's world position to viewport point (normalized 0-1 range)
        Vector3 viewportPosition = uiCamera.WorldToViewportPoint(GameManager.instance.player.transform.position);

        // Check if the object is within the camera's viewport
        if (viewportPosition.z > 0) // Ensure the object is in front of the camera
        {
            // Convert viewport position to canvas position
            Vector2 canvasSize = parentCanvas.GetComponent<RectTransform>().sizeDelta;

            Vector3 canvasPosition = new Vector3(
                (viewportPosition.x - 0.5f) * canvasSize.x,
                (viewportPosition.y - 0.5f) * canvasSize.y,
                0);

            rect.anchoredPosition = canvasPosition;
        }
        else
        {
            // Hide the health bar if the object is behind the camera
            rect.anchoredPosition = new Vector2(-1000, -1000); // Move offscreen
        }
    }
}
