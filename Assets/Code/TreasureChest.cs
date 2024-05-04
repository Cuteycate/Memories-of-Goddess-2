using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public RectTransform rect;
    Item[] items;
    public LevelUp uiLevelUp;
    GameManager manager;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }
    public void Show()
    {
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
    public void HideAllitems()
    {
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
    }
    public void Next()
    {
        // Deactivate all items
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        // Create a list to store indices of upgradable items
        List<int> upgradableIndices = new List<int>();
        for (int i = 0; i < Item.allItemData.Count; i++)
        {
            ItemData currentItemData = Item.allItemData[i];
            for (int j = 0; j < items.Length; j++)
            {
                Item currentItem = items[j];
                if (currentItem.data == currentItemData && currentItem.level < currentItem.data.damages.Length)
                {
                    upgradableIndices.Add(j);
                }
            }
        }
        // Determine rarity
        int rarity = Random.Range(0, 100); // Assuming 0-100 represents rarity percentage

        // Activate items based on rarity
        if (upgradableIndices.Count > 0)
        {
            if (rarity < 10) // Rare chance: Activate 3 items
            {
                ActivateItems(upgradableIndices, 3);
            }
            else if (rarity < 40) // Medium chance: Activate 2 items
            {
                ActivateItems(upgradableIndices, 2);
            }
            else // Common chance: Activate 1 item
            {
                ActivateItems(upgradableIndices, 1);
            }
        }
        else
        {
            if (items.Length > 10)
            {
                items[11].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No upgradable items and no fallback item found.");
            }
        }
    }
    private void ActivateItems(List<int> indices, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (indices.Count > 0)
            {
                int randomIndex = Random.Range(0, indices.Count);
                int activatedIndex = indices[randomIndex];
                Debug.Log("Activated index: " + activatedIndex);
                items[indices[randomIndex]].gameObject.SetActive(true);
                uiLevelUp.Select(activatedIndex);
                indices.RemoveAt(randomIndex);
            }
            else
            {
                Debug.LogWarning("Not enough upgradable items to activate.");
                break;
            }
        }
    }
    public void Select(int index)
    {
        items[index].LevelCount();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Show();
            Destroy(gameObject);
        }
    }
}
