using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage; // Dame vũ khí
    public int per; // Xuyên qua bao nhiêu 
    public int projectileNumber; // Số lượng projectile

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    public void Init(float damage, int per, Vector3 dir, int projectileNumber)
    {
        this.damage = damage;
        this.per = per;
        this.projectileNumber = projectileNumber; // Truyền các số lượng vào

        if (per > -1)
        {
            rigid.velocity = dir * 15f; // Tốc độ viên đạn
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")|| per == -1)
            return;
        per--;

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
