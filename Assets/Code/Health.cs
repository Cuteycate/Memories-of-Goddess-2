using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public float healPercentage = 0.2f; // Percentage of maximum health restored by the potion
        
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the GameManager instance
            GameManager gameManager = GameManager.instance;

            // If GameManager instance exists and player's health can be restored
            if (gameManager != null && gameManager.isLive && gameManager.Health <= gameManager.MaxHealth)
            {
                // Calculate the amount of health to restore
                float maxHealth = gameManager.MaxHealth;
                float healAmount = maxHealth * healPercentage;

                // Restore player's health
                gameManager.ResHealth(healAmount);

                // Destroy the potion GameObject after the player picks it up
                Destroy(gameObject);
            }

        }
    }
}

