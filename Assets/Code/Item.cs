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
    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }
    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);
        switch (data.itemType)
        {
            case ItemData.ItemType.Shovel:
                textDesc.text = string.Format(data.itemDesc[level], data.damages[level]*100, data.counts[level]);
                break;
            case ItemData.ItemType.Gun:
                textDesc.text = string.Format(data.itemDesc[level], data.damages[level]*100, data.counts[level], data.penetrations[level]);
                break;
            case ItemData.ItemType.Shotgun:
                textDesc.text = string.Format(data.itemDesc[level], data.damages[level] * 100, data.counts[level], data.penetrations[level]);
                break;
            case ItemData.ItemType.Scythe:
                textDesc.text = string.Format(data.itemDesc[level], data.damages[level], data.counts[level]*20);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.EmptyHeart:
            case ItemData.ItemType.XpCrown:
                textDesc.text = string.Format(data.itemDesc[level], data.damages[level]*100);
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
    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.Shovel:
            case ItemData.ItemType.Gun:
            case ItemData.ItemType.Shotgun:
            case ItemData.ItemType.Scythe:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;
                    int nextPenetration = 0;
                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];
                    nextPenetration += data.penetrations[level];
                    weapon.LevelUp(nextDamage, nextCount,nextPenetration);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.EmptyHeart:
            case ItemData.ItemType.ExtraProjectile:
            case ItemData.ItemType.Bandage:
            case ItemData.ItemType.XpCrown:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.Health = GameManager.instance.MaxHealth;
                break;
        }
        if(level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
