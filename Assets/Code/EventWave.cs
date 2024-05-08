﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static Spawner;
using static UnityEngine.UI.CanvasScaler;

public class EventWave : MonoBehaviour
{
    public float speed = 2;

    Rigidbody2D rigid;
    public Transform[] spawnPoint;
    public Transform bestSpawnPoint = null;
    public Transform FirstSpawnPoint;

    public float Timer;
    public float coolTime = 5;

    public Rigidbody2D target;
    Vector2 playerPos;
    Vector2 objectPos;
    Vector2 oppositePos;


    bool check = true;
    public Spawner spawner;
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();      
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    public void Inti(Transform BestSpawnPoint, Transform f)
    {
        bestSpawnPoint = BestSpawnPoint;
        FirstSpawnPoint = f;
    }



    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        playerPos = target.position;
        objectPos = transform.position;
        Timer = 0;
        check = true;
        

    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if(Timer >= coolTime)
        {
            gameObject.SetActive(false);
        }
    }


    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

      

    }

    void LateUpdate()
    {
        
        if (!GameManager.instance.isLive)
            return;

        if(check)
        {
            foreach (Transform SpawnPoint in spawnPoint)
            {
                GameObject Wave = GameManager.instance.pool.Get(9);
                Wave.transform.position = SpawnPoint.position;
                Wave.GetComponent<EnemyEvent>().Init(bestSpawnPoint);
            }
            check = false;
        }
        

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            gameObject.SetActive(false);
        }
    }



}
