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
    [TextArea]
    public string[] shopDesc;
    public Sprite shopIcon;

    [Header("# Level Data")]
    public float baseNumber;
    public int baseGold;
    public float[] Numbers;
    public int[] Golds;

    [Header("# Status")]
    public bool isMaxed = false;  //Kiem tra xem shop co maxed hay khong
    public int currentLevel = 0; // Kiem tra cap do hien tai cua Shop
}