using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.Archive;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;
    enum Achive {UnlockCharacter2,UnlockCharacter3 }
    Achive[] achives;
    WaitForSecondsRealtime wait;
    void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);
        if(!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }
    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);
        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }
    void Start()
    {
        UnlockCharacter();
    }
    void UnlockCharacter()
    {
        for(int i =0;i<lockCharacter.Length;i++)
        {
            string achiveName = achives[i].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacter[i].SetActive(!isUnlock);
            unlockCharacter[i].SetActive(isUnlock);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Achive achive in achives)
        {
            CheckAchive(achive);
        }

    }
    void CheckAchive(Achive achive)
    {
        bool isAchive = false;

        switch (achive)
        {
            case Achive.UnlockCharacter2:
                isAchive = GameManager.instance.kill >= 10;
                break;
            case Achive.UnlockCharacter3:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxgameTime;
                break;
        }
        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);
            for(int i =0; i<uiNotice.transform.childCount;i++)
            {
                bool isActive = i == (int)achive;
                uiNotice.transform.GetChild(i).gameObject.SetActive(isActive);
            }
            StartCoroutine(NoticeRoutine());
        }
    }
    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        yield return wait;
        uiNotice.SetActive(false);
    }
}
