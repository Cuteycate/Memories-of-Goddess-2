using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxgameTime = 20f;
    [Header("# Player Info")]
    public int PlayerId;
    public float Health;
    public float MaxHealth = 100;
    public int level;
    public int kill;
    public float exp;
    public float[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };
    public float ExtraRateExp=0;
    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public TreasureChest treasureChest;
    public Result uiResult;
    public GameObject enemyCleaner;

    void Awake()
    {
        instance = this;
    }
    public void GameStart(int id)
    {
        PlayerId = id;
        Health = MaxHealth;
        player.gameObject.SetActive(true);
        uiLevelUp.Select(PlayerId%2);
        Resume();
        AudioManager.instance.PlayOpening(false);
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }
    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }
    public void GameRetry()
    {
        AudioManager.instance.PlayOpening(true);
        SceneManager.LoadScene(0);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
    public void Quit()
    {
        Application.Quit();
    }
    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;
        if (gameTime > maxgameTime)
        {
            gameTime = maxgameTime;
            GameVictory();
        }
    }
    public void GetExp(Enemy enemy)
    {
        if (!isLive)
            return;
        Debug.Log("Base expOnDefeat: " + enemy.expOnDefeat);
        Debug.Log("ExtraRateExp: " + ExtraRateExp);
        exp += enemy.expOnDefeat + (enemy.expOnDefeat * ExtraRateExp);
        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }
    public void GetExp(BossEnemy enemy)
    {
        if (!isLive)
            return;
        Debug.Log("Base expOnDefeat: " + enemy.expOnDefeat);
        Debug.Log("ExtraRateExp: " + ExtraRateExp);
        exp += enemy.expOnDefeat + (enemy.expOnDefeat * ExtraRateExp);
        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }
    public void ResHealth(float amount)
    {
        if(Health < MaxHealth)
        {
            Health += amount;
            if(Health >= MaxHealth)
            {
                Health = MaxHealth;
            }
        }
      
    }
    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
    public void PlaySelect()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
}
