using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float damage; // Dame vũ khí
    public int per; // Xuyên qua bao nhiêu 
    public int projectileNumber; // Số lượng projectile

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    public void Init( int per, Vector3 dir)
    {
       
        this.per = per;
        
        if (per > -1)
        {
            rigid.velocity = dir * 5f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Bullet") || collision.CompareTag("Player"))
        {
            per--;
        }

        if (per == -1)
        {
            rigid.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Area"))
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
