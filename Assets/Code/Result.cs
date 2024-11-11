using System.Collections;
using System.Collections.Generic;
using Unity.IO.Archive;
using UnityEngine;

public class Result : MonoBehaviour
{
    public AchiveManager achiveManager;
    public GameObject[] titles;
    public void Lose()
    {
        int id = GameManager.instance.PlayerId;
        titles[0].SetActive(true);
        titles[id+2].SetActive(true);
    }
    public void Win()
    {
        int mapid = GameManager.instance.mapid;
        if(mapid == 1)
        {
            if (PlayerPrefs.GetInt("Map", 0) != 2)
            {
                achiveManager.map2unlockNotice();
            }
            PlayerPrefs.SetInt("Map", 2);
        }
        int id = GameManager.instance.PlayerId;
        titles[1].SetActive(true);
        titles[id + 2].SetActive(true);
        
    }

}
