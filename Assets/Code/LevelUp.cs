using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
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

        // Find upgradable items and store their indices
        for (int i = 0; i < items.Length-1; i++)
        {
            Item currentItem = items[i];
            if (currentItem.level < currentItem.data.damages.Length)
            {
                upgradableIndices.Add(i);
            }
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
            items[10].gameObject.SetActive(true);
        }
    }
}
