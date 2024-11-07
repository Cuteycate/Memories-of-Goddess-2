using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public ShopData data; // Reference to the ShopData ScriptableObject
    public Image icon; // UI element for displaying icon
    public int level = 0;
    public Text textName; // UI element for displaying shop name
    public Text textLevel; // UI element for displaying level
    public Text textDesc; // UI element for displaying description
    public Text goldAmount; // UI element for displaying gold amount
    public ItemPopUp itemPopUp;
    public ShopStats shopStats;
    void Awake()
    {
        // Add click listener for the shop button
        GetComponent<Button>().onClick.AddListener(OnShopClick);
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.shopIcon;

        // Retrieve and assign text components in the hierarchy
        Text[] texts = GetComponentsInChildren<Text>();
        if (texts.Length >= 4)
        {
            textName = texts[0];
            textLevel = texts[1];
            textDesc = texts[2];
            goldAmount = texts[3];
        }

        // Set icon and name if not null
        if (icon != null && data != null)
        {
            icon.sprite = data.shopIcon;
        }

        if (textName != null && data != null)
        {
            textName.text = data.shopName;
        }
    }

    void OnShopClick()
    {
        if (itemPopUp != null && data != null)
        {
            // Pass current level and data to the ItemPopUp
            itemPopUp.SetShopData(data, data.currentLevel, this, shopStats);

            // Scale popup from (0,0,0) to (1,1,1)
            itemPopUp.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void UpdateLevel(int newLevel)
    {
        data.currentLevel = newLevel;
        if (data.currentLevel >= data.shopDesc.Length)
        {
            data.isMaxed = true;
        }
    }
}
