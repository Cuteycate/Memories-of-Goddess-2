using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D aimCursor;
    public Vector2 defaultHotSpot = Vector2.zero;
    public Player player;
    public GameManager gamemanager;

    private Vector2 aimHotSpot;

    void Start()
    {
        if (aimCursor != null)
        {
            aimHotSpot = new Vector2(aimCursor.width * 0.5f, aimCursor.height * 0.5f);
        }
    }

    void Update()
    {
        UpdateCursor();
    }

    void UpdateCursor()
    {
        if (player != null)
        {
            if (player.useMouseToAim && gamemanager.isLive)
            {
                Cursor.SetCursor(aimCursor, aimHotSpot, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(defaultCursor, defaultHotSpot, CursorMode.Auto);
            }
        }
    }
}
