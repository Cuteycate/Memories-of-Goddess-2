using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickUp : MonoBehaviour
{
    public float expAmount; // Kinh nghiem
    public SpriteRenderer spriteRenderer;
    public List<Sprite> xpSprites; //Danh sách Sprites
    public float magnetSpeed = 5f; // Tốc độ Magnet
    private Transform playerTransform;
    private bool isBeingPulled = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteBasedOnExp();
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        if (isBeingPulled && playerTransform != null)
        {
            // di chuyển XP Orb tới vị trí của người chơi
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
    }
    void SetSpriteBasedOnExp()
    {
        if (expAmount >= 1 && expAmount <= 5)
        {
            spriteRenderer.sprite = xpSprites[0];
        }
        else if (expAmount >= 6 && expAmount <= 10)
        {
            spriteRenderer.sprite = xpSprites[1];
        }
        else if (expAmount >= 11 && expAmount <= 15)
        {
            spriteRenderer.sprite = xpSprites[2];
        }
        else
        {
            spriteRenderer.sprite = xpSprites[3];
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CircleCollider"))
        {
            isBeingPulled = true;
            playerTransform = other.transform;
        }
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = GameManager.instance;

            if (gameManager != null && gameManager.isLive)
            {
                // Truyen XP cua nguoi choi dua vao chi so
                // cong thuc exp = exp quai * ti le tu Item * ti le tu multiplier shop 
                // Co the sai neu nhu sai thi kho noi :(((( t luoi test vai dai ra 
                gameManager.exp += expAmount * (1 + gameManager.ExtraRateExp) * ShopStats.Instance.xpMultiplier;

                // Kiem tra level up
                if (gameManager.exp >= gameManager.nextExp[Mathf.Min(gameManager.level, gameManager.nextExp.Length - 1)])
                {
                    gameManager.level++;
                    gameManager.exp = 0;
                    gameManager.uiLevelUp.Show();
                }

                //Tieng game Work In Progress
                // AudioManager.instance.PlaySfx(AudioManager.Sfx.ExpPickup);
                // Pha XP sau khi nguoi choi nhat dc
                gameObject.SetActive(false);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CircleCollider"))
        {
            isBeingPulled = false;
            playerTransform = null;
        }
    }
}
