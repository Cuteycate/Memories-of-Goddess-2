using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float gameTime;
    public float maxgameTime = 5 * 60f;
    public PoolManager pool;
    public Player player;

    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        gameTime += Time.deltaTime;
        if (gameTime > maxgameTime)
        {
            gameTime = maxgameTime;
        }
    }
}
