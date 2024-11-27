using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChest : MonoBehaviour
{
    public RectTransform rect; // Chest RectTransform
    public RectTransform goldTitleRect; // Gold title RectTransform
    public RectTransform imageToShake; // Image inside chest to shake
    Item[] items;
    public LevelUp uiLevelUp;
    public Text goldText;
    public GameObject goldBurst;
    public GameObject goldRainEffect;
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
        AudioManager.instance.PauseBgm();
        AudioManager.instance.EffectBgm(true);
        goldText.text = "000";
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);
        AudioManager.instance.ResumeBgm();
        foreach (var item in items)
        {
            GameObject itemGameObject = item.gameObject;
            Transform vfxTransform = itemGameObject.transform.Find("ItemVFX"); 

            if (vfxTransform != null)
            {
                vfxTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"VFX not found in item: {item.name}");
            }
        }
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
        HideAllitems();
        if (goldBurst != null)
        {
            goldRainEffect.SetActive(true);
            ParticleSystem burstSystem = goldRainEffect.GetComponent<ParticleSystem>();
            if (burstSystem != null)
            {
                burstSystem.Play();
            }
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

        // Start shaking and gold counting, and activate items afterward
        StartCoroutine(ShakeAndCountGold(upgradableIndices));
    }

    private IEnumerator ShakeAndCountGold(List<int> upgradableIndices)
    {
        // Calculate gold reward
        int baseGold = Random.Range(100, 450);
        float multiplier = ShopStats.Instance.goldMultiplier;
        int totalGold = Mathf.RoundToInt(baseGold * multiplier);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Treasuremusic0);
        // Start shaking
        StartCoroutine(Shake(goldTitleRect, 5f, 5f)); // Shake chest
        if (imageToShake != null)
        {
            StartCoroutine(Shake(imageToShake, 5f, 5f)); // Shake image
        }

        // Animate gold text
        int currentGold = 0;
        float duration = 5f; // Duration for counting
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / duration;

            // Use easing function for more natural counting
            currentGold = Mathf.RoundToInt(Mathf.Lerp(1, totalGold, progress * progress));
            goldText.text = currentGold.ToString();
            yield return null;
        }

        // Ensure final value is correct
        goldText.text = totalGold.ToString();

        // Stop shaking
        StopAllCoroutines();
        rect.localPosition = Vector3.zero;
        if (imageToShake != null)
        {
            imageToShake.localPosition = Vector3.zero;
        }

        // Play gold burst particle effect
        if (goldBurst != null)
        {
            goldBurst.SetActive(true);
            ParticleSystem burstSystem = goldBurst.GetComponent<ParticleSystem>();
            if (burstSystem != null)
            {
                burstSystem.Play();
            }
        }
        // Activate items
        ActivateItems(upgradableIndices);
        GameManager.instance.gold += totalGold;
    }

    private void ActivateItems(List<int> indices)
    {
        int rarity = Random.Range(0, 100);

        if (indices.Count > 0)
        {
            if (rarity < 10) // Rare chance: Activate 3 items
            {
                ActivateItemsHelper(indices, 3);
            }
            else if (rarity < 40) // Medium chance: Activate 2 items
            {
                ActivateItemsHelper(indices, 2);
            }
            else // Common chance: Activate 1 item
            {
                ActivateItemsHelper(indices, 1);
            }
        }
        else
        {
            if (items.Length > 14)
            {
                items[15].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No upgradable items and no fallback item found.");
            }
        }
    }

    private void ActivateItemsHelper(List<int> indices, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (indices.Count > 0)
            {
                // Select a random index from the list of available indices
                int randomIndex = Random.Range(0, indices.Count);
                int activatedIndex = indices[randomIndex];

                Debug.Log("Activated index: " + activatedIndex);

                // Access the associated GameObject from the Item
                GameObject itemGameObject = items[activatedIndex].gameObject;
                itemGameObject.SetActive(true);

                // Activate the VFX child if it exists
                Transform vfxTransform = itemGameObject.transform.Find("ItemVFX");
                if (vfxTransform != null)
                {
                    vfxTransform.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"VFX child not found for item at index: {activatedIndex}");
                }

                // Notify UI about the activation
                uiLevelUp.Select(activatedIndex);

                // Remove the used index from the list
                indices.RemoveAt(randomIndex);
            }
            else
            {
                Debug.LogWarning("Not enough upgradable items to activate.");
                break;
            }
        }
    }



    private IEnumerator Shake(RectTransform target, float duration, float magnitude)
    {
        Vector3 originalPosition = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            target.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.unscaledDeltaTime; // Use unscaled time here
            yield return null;
        }

        target.localPosition = originalPosition; // Reset to original position
    }

}
