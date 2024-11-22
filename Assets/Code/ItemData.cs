using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item",menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType {Shovel,Gun,Glove,Shoe,EmptyHeart,ExtraProjectile,Bandage,XpCrown,Shotgun,SniperRifle,Scythe,Lightning,Radius,Axe,Knife,Heal}
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
    public float baseCoolDown;
    public float baseSize;
    public float[] damages;
    public int[] counts;
    public int[] penetrations;
    public float[] sizes;
    [Header("# Weapon")]
    public GameObject projectiles;
    public GameObject HitEffect;
    public Sprite hand;

}
