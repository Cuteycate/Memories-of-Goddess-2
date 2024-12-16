using UnityEngine;

public class CanvasCameraAssigner : MonoBehaviour
{
    public string cameraName = "UICamera"; // Tên GameObject của Camera trong Hierarchy

    void OnEnable()
    {
        GameObject uiCameraObject = GameObject.Find(cameraName);
        if (uiCameraObject != null)
        {
            Camera uiCamera = uiCameraObject.GetComponent<Camera>();
            if (uiCamera != null)
            {
                Canvas canvas = GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.worldCamera = uiCamera;
                    Debug.Log($"UICamera đã được gán vào Render Camera của {canvas.gameObject.name}");
                }
                else
                {
                    Debug.LogError("Không tìm thấy component Canvas trên GameObject.");
                }
            }
            else
            {
                Debug.LogError("Không tìm thấy component Camera trên GameObject.");
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy GameObject tên {cameraName} trong Hierarchy.");
        }
    }
}
