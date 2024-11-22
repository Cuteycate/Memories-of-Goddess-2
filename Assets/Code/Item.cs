using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;
    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    public static List<Weapon> ListWeapon = new List<Weapon>();
    public static List<Gear> ListGear = new List<Gear>();
    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts.Length > 2 ? texts[2] : null;
        textName.text = data.itemName;
    }
    public static List<ItemData> allItemData = new List<ItemData>();
    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);
        if (textDesc != null)
        {
            switch (data.itemType)
            {
                case ItemData.ItemType.Shovel:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level]);
                    break;
                case ItemData.ItemType.Gun:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level]);
                    break;
                case ItemData.ItemType.Shotgun:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level]);
                    break;
                case ItemData.ItemType.SniperRifle:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level]);
                    break;
                case ItemData.ItemType.Scythe:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level]*50) ;
                    break;
                case ItemData.ItemType.Lightning:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level] * 50);
                    break;
                case ItemData.ItemType.Axe:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level] ,data.sizes[level] * 100);
                    break;
                case ItemData.ItemType.Knife:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level], data.sizes[level]);
                    break;
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                case ItemData.ItemType.EmptyHeart:
                case ItemData.ItemType.XpCrown:
                case ItemData.ItemType.Radius:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100);
                    break;
                case ItemData.ItemType.ExtraProjectile:
                case ItemData.ItemType.Bandage:
                    textDesc.text = string.Format(data.itemDesc[level], data.damages[level]);
                    break;
                default:
                    textDesc.text = string.Format(data.itemDesc[level]);
                    break;
            }
        }
    }
    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Shovel:
            case ItemData.ItemType.Gun:
            case ItemData.ItemType.Shotgun:
            case ItemData.ItemType.SniperRifle:
            case ItemData.ItemType.Scythe:
            case ItemData.ItemType.Lightning:
            case ItemData.ItemType.Axe:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    
                    if (!allItemData.Contains(data))
                    {
                        allItemData.Add(data);
                    }
                    weapon.Init(data);
                    ListWeapon.Add(weapon);
                }
                else
                {
                    float nextDamage = 0;
                    int nextCount = 0;
                    int nextPenetration = 0;
                    float nextSize = 0;
                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];
                    nextPenetration += data.penetrations[level];
                    nextSize += weapon.size * data.sizes[level];
                    weapon.LevelUp(nextDamage, nextCount,nextPenetration,nextSize);
                }
                LevelCount();
                break;
            case ItemData.ItemType.Knife:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();

                    if (!allItemData.Contains(data))
                    {
                        allItemData.Add(data);
                    }
                    weapon.Init(data);
                    ListWeapon.Add(weapon);
                }
                else
                {
                    float nextDamage = 0;
                    int nextCount = 0;
                    int nextPenetration = 0;
                    float nextSize = 0;
                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];
                    nextPenetration += data.penetrations[level];
                    nextSize += data.baseSize * data.sizes[level];
                    weapon.LevelUp(nextDamage, nextCount, nextPenetration, nextSize);
                }
                LevelCount();
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.EmptyHeart:
            case ItemData.ItemType.ExtraProjectile:
            case ItemData.ItemType.Bandage:
            case ItemData.ItemType.XpCrown:
            case ItemData.ItemType.Radius:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    if (!allItemData.Contains(data))
                    {
                        allItemData.Add(data);
                    }
                    gear.Init(data);
                    ListGear.Add(gear);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                LevelCount();
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.ResHealth(30);
                break;
        }
        if(level == data.damages.Length)
        {
            allItemData.Remove(data);
            GetComponent<Button>().interactable = false;
        }
    }
    public void LevelCount()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Shovel:
            case ItemData.ItemType.Gun:
            case ItemData.ItemType.Shotgun:
            case ItemData.ItemType.SniperRifle:
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.EmptyHeart:
            case ItemData.ItemType.ExtraProjectile:
            case ItemData.ItemType.Bandage:
            case ItemData.ItemType.XpCrown:
            case ItemData.ItemType.Scythe:
            case ItemData.ItemType.Lightning:
            case ItemData.ItemType.Radius:
            case ItemData.ItemType.Axe:
            case ItemData.ItemType.Knife:
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.ResHealth(30);
                break;
        }
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }

}
