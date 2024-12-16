using System.Collections;
using System.Collections.Generic;
using Unity.IO.Archive;
using UnityEngine;

public class Result : MonoBehaviour
{
    public AchiveManager achiveManager;
    public GameObject[] titles;
    public GameObject HUD;
    public void Lose()
    {
        int id = GameManager.instance.PlayerId;
        titles[0].SetActive(true);
        for (int i = 2; i < titles.Length; i++)
        {
            if (id + 2 == i)
            {
                titles[id + 2].SetActive(true);
            }
            else
            {
                titles[i].SetActive(false);
            }
        }
        HUD.SetActive(false);
    }
    public void Win()
    {
        int mapid = GameManager.instance.mapid;
        if (mapid == 1)
        {
            if (PlayerPrefs.GetInt("Map", 0) != 2)
            {
                achiveManager.map2unlockNotice();
            }
            PlayerPrefs.SetInt("Map", 2);
        }
        titles[1].SetActive(true);
        int id = GameManager.instance.PlayerId;
        for (int i = 2; i < titles.Length; i++)
        {
            if (id + 2 == i)
            {
                titles[id + 2].SetActive(true);
            }
            else
            {
                titles[i].SetActive(false);
            }
        }
        HUD.SetActive(false);

    }

}
