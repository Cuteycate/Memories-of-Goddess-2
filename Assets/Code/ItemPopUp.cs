using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopUp : MonoBehaviour
{
    public Image icon;
    public Text textName;
    public Text textLevel;
    public Text textDesc;
    public Text goldAmount;
    public ShopData data;
    private Shop linkedShop;
    private int currentLevel = 0;
    public Button upgradeButton;
    public ShopStats shopStats;
    public void SetShopData(ShopData newData, int level, Shop shop, ShopStats stats)
    {
        if (newData != null)
        {
            data = newData;
            currentLevel = level;
            linkedShop = shop;
            shopStats = stats;
            // Assign icon
            icon = GetComponentsInChildren<Image>()[1];
            icon.sprite = data.shopIcon;

            // Tìm và assign text
            Text[] texts = GetComponentsInChildren<Text>();
            if (texts.Length >= 4)
            {
                textName = texts[0];
                textLevel = texts[1];
                textDesc = texts[2];
                goldAmount = texts[3];
            }

            // cập nhật display
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        textName.text = data.shopName;
        textLevel.text = "Lv." + (currentLevel + 1);

        if (data.isMaxed)
        {
            // Nếu như Maxed thì truyền những dữ liệu bên dưới và xóa interactable nút upgrade
            textDesc.text = "Maxed";
            goldAmount.text = "0";
            if (upgradeButton != null)
            {
                upgradeButton.interactable = false;
            }
        }
        else
        {
            // Hiển thị bth theo data
            textDesc.text = string.Format(data.shopDesc[currentLevel], data.Numbers[currentLevel] * 100);
            goldAmount.text = data.Golds[currentLevel].ToString();
            if (upgradeButton != null)
            {
                // Nếu như người chơi có vàng để mua thì có thể mua được hoặc không thì nút sẽ bị disable
                upgradeButton.interactable = GameManager.instance.totalGold >= data.Golds[currentLevel];
            }
        }
    }

    public void OnUpgradeClick()
    {
        // Kiểm tra xem người chơi có đủ vàng hay chưa
        int requiredGold = data.Golds[currentLevel];
        if (!data.isMaxed && currentLevel < data.shopDesc.Length - 1)
        {
            
            if (GameManager.instance.totalGold >= requiredGold)
            {
                // Giảm vàng bên GameManager và lưu lại
                GameManager.instance.totalGold -= requiredGold;
                PlayerPrefs.SetInt("TotalGold", GameManager.instance.totalGold);
                PlayerPrefs.Save();

                // Cộng level và tăng cấp đồng thời gán currentLevel
                currentLevel++;
                data.currentLevel = currentLevel;
                UpdateDisplay();
                if (shopStats != null)
                {
                    float upgradeValue = data.Numbers[currentLevel-1];  // dun vao data
                    shopStats.ApplyUpgrade(data.shopId, upgradeValue);  // Them stats shop
                }
                // Cập nhật Cấp bên Shop
                if (linkedShop != null)
                {
                    linkedShop.UpdateLevel(currentLevel);
                }
            }
            else
            {
                // Không đủ vàng để nâng cập
                Debug.Log("Not enough gold to upgrade!");
            }
        }
        else
        {
            if (upgradeButton != null)
            {
                upgradeButton.interactable = false;
            }
            GameManager.instance.totalGold -= requiredGold;
            PlayerPrefs.SetInt("TotalGold", GameManager.instance.totalGold);
            PlayerPrefs.Save();
            float upgradeValue = data.Numbers[currentLevel];
            shopStats.ApplyUpgrade(data.shopId, upgradeValue);
            data.isMaxed = true;
            textDesc.text = "Maxed";
            goldAmount.text = "0";
        }
    }
}
