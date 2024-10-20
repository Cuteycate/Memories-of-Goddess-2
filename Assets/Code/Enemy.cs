﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spawner;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animController; //Dùng đề đưa animation enemies vào vd animation 1 zombie animation 2 skeleton
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate()
    {
        if(!isLive)
        {
            return;
        }
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position+nextVec);
        rigid.velocity = Vector2.zero;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!isLive)
        {
            return;
        }
        spriter.flipX = target.position.x < rigid.position.x;

    }
    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }
    public void Init(SpawnData Data)
    {
        anim.runtimeAnimatorController = animController[Data.spriteType];
        speed = Data.speed;
        maxHealth = Data.health;
        health = Data.health;
    }
}
