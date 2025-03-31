using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop", menuName = "Scriptable Object/ShopData")]
public class ShopData : ScriptableObject
{
    public enum ShopType
    {
        Might,
        MaxHealth,
        HealthRecovery,
        MovementSpeed,
        Amount,
        WeaponSpeed,
        Magnet,
        Growth,
        Greed
    }

    [Header("# Main Info")]
    public ShopType shopType;
    public int shopId;
    public string shopName;
    [TextArea] public string[] shopDesc;
    public Sprite shopIcon;

    [Header("# Level Data")]
    public float baseNumber;
    public int baseGold;
    public float[] Numbers;
    public int[] Golds;

    [Header("# Status")]
    public bool isMaxed = false;
    public int currentLevel = 0;

    // Save data for this shop item
    public void SaveData()
    {
        PlayerPrefs.SetInt($"{shopName}_currentLevel", currentLevel);
        PlayerPrefs.SetInt($"{shopName}_isMaxed", isMaxed ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Load data for this shop item
    public void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt($"{shopName}_currentLevel", 0); // Default to level 0
        isMaxed = PlayerPrefs.GetInt($"{shopName}_isMaxed", 0) == 1;      // Default to not maxed
    }
}
