using UnityEngine;

public class ShopStats : MonoBehaviour
{
    public static ShopStats Instance { get; private set; }
    public float damageMultiplier = 1f;      // Additional multiplier for damage upgrades
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    // Method to apply an upgrade based on shopID

    public void ApplyUpgrade(int shopID, float upgradeValue)
    {
        switch (shopID)
        {
            case 0: // Might
                IncreaseDamage(upgradeValue);
                break;
            case 1: // Movement Speed (ID 1)
                IncreaseSpeed(upgradeValue);
                break;
            default:
                Debug.LogWarning("Unknown shopID: " + shopID);
                break;
        }
        SaveStats();
    }

    // Tăng dame theo %
    private void IncreaseDamage(float value)
    {
        damageMultiplier = 1 + value; // Cộng dồn dame theo chỉ số %
        Debug.Log("Damage upgraded! New multiplier: " + damageMultiplier);
    }

    private void IncreaseSpeed(float value)
    {
        // Example implementation for increasing speed (if needed)
        Debug.Log("Speed upgraded! New multiplier: " + value);
    }
    // Lưu Stats qua PlayerPrefs
    public void SaveStats()
    {
        PlayerPrefs.SetFloat("DamageMultiplier", damageMultiplier);
        PlayerPrefs.Save();
    }

    // Load Stats qua PlayerPrefs
    public void LoadStats()
    {
        damageMultiplier = PlayerPrefs.GetFloat("DamageMultiplier", 1f);  // Default 1f nếu như không lưu
    }
}
