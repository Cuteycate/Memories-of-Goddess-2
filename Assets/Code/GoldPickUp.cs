using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickUp : MonoBehaviour
{
    public int goldAmount = 50; 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Lấy gameManager
            GameManager gameManager = GameManager.instance;

            // nếu gamemamanger còn hoạt đồng và nhân vật còn sống
            if (gameManager != null && gameManager.isLive)
            {
                gameManager.gold += goldAmount;

                // AudioManager.instance.PlaySfx(AudioManager.Sfx.GoldPickup);

                // phá vàng
                Destroy(gameObject);
            }
        }
    }
}
