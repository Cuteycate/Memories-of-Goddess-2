using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage; //dame của vũ khí
    public int per; //Penetration

    public void Init(float damage,int per)
    {
        this.damage = damage;
        this.per = per;
    }
}
