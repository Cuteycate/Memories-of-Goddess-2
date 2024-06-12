using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Options : MonoBehaviour
{
    RectTransform rect;
    bool isShowing = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
                if (isShowing)
                    Hide();
                else
                    Show();
        }
    }
    public void Show()
    {
        if (GameManager.instance.isLive)
        {
            rect.localScale = Vector3.one;
            GameManager.instance.Stop();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
            AudioManager.instance.EffectBgm(true);
            isShowing = true;
        }
    }

    public void Hide()
    {
            rect.localScale = Vector3.zero;
            GameManager.instance.Resume();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
            AudioManager.instance.EffectBgm(false);
            isShowing = false;
    }
}