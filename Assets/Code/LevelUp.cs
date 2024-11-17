using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    public List<Weapon> weapons = Item.ListWeapon;
    public List<Gear> gears = Item.ListGear;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);

    }
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);

    }
    public void Select(int index)
    {
        items[index].OnClick();
    }
    void Next()
    {
        // Deactivate all items
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // Create a list to store indices of upgradable items
        List<int> upgradableIndices = new List<int>();
        List<int> ListWeaponId = new List<int> {0,1,8,9,10,11};//id data item of weapon
        List<int> ListGearId = new List<int> { 2,3,4,5,6,7,12 };  //id data item of gear
        // Find upgradable items and store their indices
        for (int i = 0; i < items.Length-1; i++)
        {
            Item currentItem = items[i];
            if (currentItem.level < currentItem.data.damages.Length)
            {
                upgradableIndices.Add(i);
            }
        }

        if (weapons.Count >= 2)
        {
            ListWeaponId.RemoveAll(id => weapons.Any(w => w.id == id));
            upgradableIndices.RemoveAll(id => ListWeaponId.Contains(id));
        }
        if (gears.Count >= 2)
        {
            ListGearId.RemoveAll(id => gears.Any(w => w.id == id));
            upgradableIndices.RemoveAll(id => ListGearId.Contains(id));
        }
        // Activate items based on the number of upgradable items available
        if (upgradableIndices.Count >= 3)
        {
            // If there are at least three upgradable items, activate exactly three of them
            for (int i = 0; i < 3; i++)
            {
                int randomIndex = Random.Range(0, upgradableIndices.Count);
                items[upgradableIndices[randomIndex]].gameObject.SetActive(true);
                upgradableIndices.RemoveAt(randomIndex);
            }
        }
        else if (upgradableIndices.Count == 2)
        {
            // If there are two upgradable items, activate two of them
            for (int i = 0; i < 2; i++)
            {
                int randomIndex = Random.Range(0, upgradableIndices.Count);
                items[upgradableIndices[randomIndex]].gameObject.SetActive(true);
                upgradableIndices.RemoveAt(randomIndex);
            }
        }
        else if (upgradableIndices.Count == 1)
        {
            items[upgradableIndices[0]].gameObject.SetActive(true);
        }
        else
        {
            // If there are no upgradable items, activate items[4]
            items[13].gameObject.SetActive(true);
        }
    }
}
