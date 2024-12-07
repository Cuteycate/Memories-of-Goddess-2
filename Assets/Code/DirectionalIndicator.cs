using UnityEngine;

public class DirectionalIndicator : MonoBehaviour
{
    public Sprite upSprite; // Sprite used for mouse aiming
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite upLeftSprite;
    public Sprite upRightSprite;
    public Sprite downLeftSprite;
    public Sprite downRightSprite;

    public float offset = 2f;

    private SpriteRenderer spriteRenderer;
    private Player player;
    private Vector2 lastInputDirection = Vector2.left;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<Player>();

        if (player == null)
        {
            Debug.LogError("Player reference is missing in DirectionalIndicator!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            if (player.useMouseToAim)
            {
                UpdateMouseAim();
            }
            else
            {
                UpdateKeyboardAim();
            }
            UpdatePosition();
        }
    }

    private void UpdateKeyboardAim()
    {
        Vector2 inputVec = player.inputVec;

        if (inputVec != Vector2.zero)
        {
            lastInputDirection = inputVec.normalized;
            UpdateSprite(lastInputDirection);
        }
    }

    private void UpdateMouseAim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 aimDirection = ((Vector2)mousePosition - (Vector2)player.transform.position).normalized;

        if (aimDirection != Vector2.zero)
        {
            lastInputDirection = aimDirection;

            spriteRenderer.sprite = upSprite;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }



    private void UpdatePosition()
    {
        if (player != null && lastInputDirection != Vector2.zero)
        {
            Vector2 offsetPosition = (Vector2)player.transform.position + lastInputDirection * offset;
            transform.position = offsetPosition;
        }
    }


    private void UpdateSprite(Vector2 direction)
    {
        transform.rotation = Quaternion.identity;
        if (direction == Vector2.up)
            spriteRenderer.sprite = upSprite;
        else if (direction == Vector2.down)
            spriteRenderer.sprite = downSprite;
        else if (direction == Vector2.left)
            spriteRenderer.sprite = leftSprite;
        else if (direction == Vector2.right)
            spriteRenderer.sprite = rightSprite;
        else if (direction == (Vector2.up + Vector2.left).normalized)
            spriteRenderer.sprite = upLeftSprite;
        else if (direction == (Vector2.up + Vector2.right).normalized)
            spriteRenderer.sprite = upRightSprite;
        else if (direction == (Vector2.down + Vector2.left).normalized)
            spriteRenderer.sprite = downLeftSprite;
        else if (direction == (Vector2.down + Vector2.right).normalized)
            spriteRenderer.sprite = downRightSprite;
    }
}
