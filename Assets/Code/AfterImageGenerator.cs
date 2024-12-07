using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageGenerator : MonoBehaviour
{
    [Header("Afterimage Settings")]
    public float interval = 0.05f; // Time between afterimages
    public float afterimageDuration = 0.1f; // How long the afterimage lasts
    public int poolIndex = 20; // Index of the afterimage prefab in the pool
    
    private SpriteRenderer spriteRenderer;
    private List<GameObject> activeAfterimages = new List<GameObject>(); // Track active afterimages
   
    public SpriteRenderer spE;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("AfterImageGenerator: No SpriteRenderer found on this object.");
        }
    }

    /// <summary>Start generating afterimages.</summary>
    public void StartAfterImages()
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(SpawnAfterimages());
        }
    }

    /// <summary>Stop generating afterimages and deactivate all active ones.</summary>
    public void StopAfterImages()
    {
        StopAllCoroutines();

        // Deactivate all currently active afterimages
        foreach (GameObject afterimage in activeAfterimages)
        {
            if (afterimage != null && afterimage.activeSelf)
            {
                afterimage.SetActive(false);               
            }
        }

        activeAfterimages.Clear(); // Clear the list after deactivating
    }

    private IEnumerator SpawnAfterimages()
    {
        while (true)
        {
            // Retrieve an afterimage from the pool
            GameObject afterimageObj = GameManager.instance.pool.Get(poolIndex);
            if (afterimageObj != null)
            {
                // Track this afterimage
                activeAfterimages.Add(afterimageObj);

                // Initialize afterimage position, rotation, and visuals
                afterimageObj.transform.position = transform.position;
                afterimageObj.transform.rotation = transform.rotation;
                afterimageObj.transform.localScale = transform.localScale;
                SpriteRenderer afterimageRenderer = afterimageObj.GetComponent<SpriteRenderer>();
                if (afterimageRenderer != null)
                {
                    if ( poolIndex == 28)
                    {
                        afterimageRenderer.sprite = spE.sprite;
                        afterimageRenderer.color = spE.color;
                        afterimageRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
                    }
                    else
                    {
                        afterimageRenderer.sprite = spriteRenderer.sprite;
                        afterimageRenderer.color = spriteRenderer.color;
                        afterimageRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
                    }
                    // Start the fade coroutine
                    StartCoroutine(FadeAndReturnToPool(afterimageRenderer, afterimageObj));
                }
            }

            // Wait for the next interval
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator FadeAndReturnToPool(SpriteRenderer afterimageRenderer, GameObject afterimageObj)
    {
        float fadeSpeed = 1f / afterimageDuration;
        Color startColor = afterimageRenderer.color;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed)
        {
            afterimageRenderer.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                Mathf.Lerp(startColor.a, 0, t)
            );
            yield return null;
        }

        // Deactivate and return the afterimage to the pool
        afterimageObj.SetActive(false);

        // Remove from the active list
        activeAfterimages.Remove(afterimageObj);
    }

    

}
