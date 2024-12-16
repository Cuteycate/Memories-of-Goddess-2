using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChestPopUp : MonoBehaviour
{
    public Image icon;
    public Text itemNameText;
    public Text itemLevelText;
    public Text itemDescText;
    public Text itemCounterText; // To show the number of items info like (01/03)
    public Button nextButton;
    public Button closeButton;

    public List<GameObject> chestItemObjects = new List<GameObject>();
    private int currentItemIndex = 0;
    public TreasureChest treasureChest; // Reference to the TreasureChest

    void OnEnable()
    {
        Item.OnItemLevelUp += UpdateChestItem;
    }

    void OnDisable()
    {
        Item.OnItemLevelUp -= UpdateChestItem;
    }

    public void InitializeChest()
    {
        // Reset the current item index to the first item
        currentItemIndex = 0;

        // Display the first item if there is any
        if (chestItemObjects.Count > 0)
        {
            DisplayCurrentItem(currentItemIndex);
        }
        else
        {
            Debug.LogWarning("No unlocked items found.");
        }
    }



    private void DisplayCurrentItem(int activatedIndex)
    {
        if (chestItemObjects.Count > 0 && activatedIndex < chestItemObjects.Count)
        {
            GameObject selectedItemObject = chestItemObjects[activatedIndex];
            Item itemData = selectedItemObject.GetComponent<Item>();

            if (itemData != null)
            {
                // Update UI components
                icon.sprite = itemData.data.itemIcon;
                itemNameText.text = itemData.data.itemName;

                // Get the current level and max level
                int currentLevel = Item.ItemLevels.ContainsKey(itemData.data.itemId)
                    ? Item.ItemLevels[itemData.data.itemId]
                    : 0; // Default level if not found

                int maxLevel = itemData.data.damages.Length;

                if (currentLevel >= maxLevel)
                {
                    itemLevelText.text = "Lv. MAX";
                }
                else
                {
                    itemLevelText.text = $"Lv. {currentLevel}";
                }
                int preUpgradeLevel = Mathf.Max(currentLevel - 1, 0);
                itemDescText.text = itemData.GenerateDescription(preUpgradeLevel);
            }

            // Ensure the button is always interactable
            nextButton.interactable = true;
        }

        // Update item counter
        UpdateItemCounter();
    }



    private void UpdateChestItem(Item updatedItem)
    {
        int index = chestItemObjects.FindIndex(obj => obj.GetComponent<Item>() == updatedItem);
        if (index != -1)
        {
            DisplayCurrentItem(index);
        }
    }

    public void OnNextButtonClick()
    {
        List<int> activatedIndices = treasureChest.GetActivatedItems();
        int maxClicks = activatedIndices.Count - 1;

        if (maxClicks <= 0)
        {
            Debug.Log("Not enough items to navigate to the next one.");
            return;
        }
        currentItemIndex = (currentItemIndex + 1) % chestItemObjects.Count;
        DisplayCurrentItem(currentItemIndex);
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }
    private void UpdateItemCounter()
    {
        itemCounterText.text = $"{currentItemIndex + 1:00}/{chestItemObjects.Count:00}";
    }
    public void ClearChestItems()
    {
        chestItemObjects.Clear();
    }

    public void AddChestItem(GameObject item)
    {
        chestItemObjects.Add(item);
    }

}
