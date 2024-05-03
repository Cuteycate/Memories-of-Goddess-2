using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item",menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType {Shovel,Gun,Glove,Shoe,EmptyHeart,ExtraProjectile,Bandage,XpCrown,Shotgun,SniperRifle,Heal}
    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string[] itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public int basePenetration;
    public float[] damages;
    public int[] counts;
    public int[] penetrations;
    [Header("# Weapon")]
    public GameObject projectiles;
    public Sprite hand;

}
