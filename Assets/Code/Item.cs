using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public Weapon weapon;
    public Gear gear;
    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    public static List<Weapon> ListWeapon = new List<Weapon>();
    public static List<Gear> ListGear = new List<Gear>();

    // Shared level data across all instances
    public static Dictionary<int, int> ItemLevels = new Dictionary<int, int>();

    public delegate void ItemLevelUpHandler(Item item);
    public static event ItemLevelUpHandler OnItemLevelUp;

    public static List<ItemData> allItemData = new List<ItemData>();

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts.Length > 2 ? texts[2] : null;
        textName.text = data.itemName;
        if (!ItemLevels.ContainsKey(data.itemId))
        {
            Debug.Log($"Initializing itemId {data.itemId} in ItemLevels.");
            ItemLevels[data.itemId] = 0; 
        }
    }



    void OnEnable()
    {
        UpdateItemUI();
    }

    void UpdateItemUI()
    {
        int currentLevel = ItemLevels[data.itemId];
        if (currentLevel < data.damages.Length)
        {
            textLevel.text = "Lv." + (currentLevel + 1);
            if (textDesc != null)
            {
                textDesc.text = GenerateDescription(currentLevel);
            }
        }
        else
        {
            Debug.LogWarning($"Current level {currentLevel} exceeds the number of damage levels {data.damages.Length} for item {data.itemId}.");
        }
    }


    public string GenerateDescription(int currentLevel)
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Shovel:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100, data.counts[currentLevel]);
            case ItemData.ItemType.Gun:
            case ItemData.ItemType.Shotgun:
            case ItemData.ItemType.SniperRifle:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100, data.counts[currentLevel], data.penetrations[currentLevel]);
            case ItemData.ItemType.Scythe:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100, data.counts[currentLevel] * 50);
            case ItemData.ItemType.Lightning:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100, data.counts[currentLevel], data.penetrations[currentLevel] * 50);
            case ItemData.ItemType.Axe:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100, data.counts[currentLevel], data.penetrations[currentLevel], data.sizes[currentLevel] * 100);
            case ItemData.ItemType.Knife:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100, data.counts[currentLevel], data.penetrations[currentLevel], data.sizes[currentLevel]);
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.EmptyHeart:
            case ItemData.ItemType.XpCrown:
            case ItemData.ItemType.Radius:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel] * 100);
            case ItemData.ItemType.ExtraProjectile:
            case ItemData.ItemType.Bandage:
                return string.Format(data.itemDesc[currentLevel], data.damages[currentLevel]);
            default:
                return string.Format(data.itemDesc[currentLevel]);
        }
    }

    public void OnClick()
    {
        int currentLevel = ItemLevels[data.itemId];

        switch (data.itemType)
        {
            case ItemData.ItemType.Shovel:
            case ItemData.ItemType.Gun:
            case ItemData.ItemType.Shotgun:
            case ItemData.ItemType.SniperRifle:
            case ItemData.ItemType.Knife:
            case ItemData.ItemType.Lightning:
            case ItemData.ItemType.Axe:
            case ItemData.ItemType.Scythe:
                HandleWeaponItem(currentLevel);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            case ItemData.ItemType.EmptyHeart:
            case ItemData.ItemType.ExtraProjectile:
            case ItemData.ItemType.Bandage:
            case ItemData.ItemType.XpCrown:
            case ItemData.ItemType.Radius:
                HandleGearItem(currentLevel);
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.ResHealth(30);
                break;
        }

        if (currentLevel == data.damages.Length - 1)
        {
            allItemData.Remove(data);
            GetComponent<Button>().interactable = false;
        }
    }

    void HandleWeaponItem(int currentLevel)
    {
        if (currentLevel == 0)
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
            float nextDamage = data.baseDamage * data.damages[currentLevel];
            int nextCount = data.counts[currentLevel];
            int nextPenetration = data.penetrations[currentLevel];
            float nextSize = weapon.size * data.sizes[currentLevel];
            weapon.LevelUp(nextDamage, nextCount, nextPenetration, nextSize);
        }
        LevelCount();
    }

    void HandleGearItem(int currentLevel)
    {
        if (currentLevel == 0)
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
            float nextRate = data.damages[currentLevel];
            gear.LevelUp(nextRate);
        }
        LevelCount();
    }

    public void LevelCount()
    {
        // Increment level in the shared dictionary
        ItemLevels[data.itemId]++;

        OnItemLevelUp?.Invoke(this);

        if (ItemLevels[data.itemId] == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

        UpdateItemUI(); // Update the UI after leveling up
    }
    public static void ResetItems()
    {
        Debug.Log("[Item.ResetItems] Attempting Reset...");
        ItemLevels.Clear(); 
        allItemData.Clear(); 

        if (ListWeapon != null)
        {
            Debug.Log($"[Item.ResetItems] Processing {ListWeapon.Count} weapon(s) for destruction.");
            for (int i = ListWeapon.Count - 1; i >= 0; i--)
            {
                if (ListWeapon[i] != null)
                {
                    if (ListWeapon[i].gameObject != null)
                    {
                        Object.DestroyImmediate(ListWeapon[i].gameObject);
                    }
                    else
                    {
                        Debug.LogWarning($"[Item.ResetItems] Weapon at index {i} had a null GameObject. Already destroyed?");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Item.ResetItems] Found a null Weapon entry at index {i}.");
                }
            }
            ListWeapon.Clear();
        }
        else
        {
            Debug.LogError("[Item.ResetItems] ListWeapon is NULL! Re-initializing.");
            ListWeapon = new List<Weapon>();
        }
        if (ListGear != null)
        {
            Debug.Log($"[Item.ResetItems] Processing {ListGear.Count} gear(s) for destruction.");
            for (int i = ListGear.Count - 1; i >= 0; i--)
            {
                if (ListGear[i] != null)
                {
                    if (ListGear[i].gameObject != null)
                    {
                        Object.DestroyImmediate(ListGear[i].gameObject);
                    }
                    else
                    {
                        Debug.LogWarning($"[Item.ResetItems] Gear at index {i} had a null GameObject. Already destroyed?");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Item.ResetItems] Found a null Gear entry at index {i}.");
                }
            }
            ListGear.Clear();
        }
        else
        {
            Debug.LogError("[Item.ResetItems] ListGear is NULL! Re-initializing.");
            ListGear = new List<Gear>();
        }


        Debug.Log("[Item.ResetItems] Reset complete.");
    }
}
