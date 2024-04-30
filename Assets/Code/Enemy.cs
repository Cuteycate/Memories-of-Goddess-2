using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spawner;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int expOnDefeat;
    public RuntimeAnimatorController[] animController; //Dùng đề đưa animation enemies vào vd animation 1 zombie animation 2 skeleton
    public Rigidbody2D target;
    public GameObject floatingtextPrefab;

    bool isLive;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Collider2D coll;
    Animator anim;
    WaitForFixedUpdate wait;



    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
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
        if (!GameManager.instance.isLive)
            return;
        if (!isLive)
        {
            return;
        }
        spriter.flipX = target.position.x < rigid.position.x;

    }
    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }
    public void Init(SpawnData Data)
    {
        anim.runtimeAnimatorController = animController[Data.spriteType];
        speed = Data.speed;
        maxHealth = Data.health;
        health = Data.health;
        expOnDefeat = Data.expOnDefeat;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;
        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());
        ShowDamage(collision.GetComponent<Bullet>().damage.ToString());
        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            //.. sống,bị trúng
        }
        else
        {
            //Chết
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead",true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp(this);
            if(GameManager.instance.isLive)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }
    IEnumerator KnockBack()
    {
        /*   yield return null; //khựng 1 frame
           yield return new WaitForSeconds(2f); //coroutine 2s hết 2s mới tiếp tục bị knockback bới return null */
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; // khoảng cách enemy - khoảng cách nhân vật
        rigid.AddForce(dirVec.normalized * 3,ForceMode2D.Impulse); //Truyền knockback ngược về so với PlayerPos
    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
    void ShowDamage(string text)
    {
        GameObject prefab = Instantiate(floatingtextPrefab, transform.position, Quaternion.identity);
        prefab.GetComponentInChildren<TextMesh>().text = text;
    }
    public void TakeDamage(float damage)
    {
        // Subtract damage from health
        health -= damage;
        ShowDamage(damage.ToString());
        // Check if the enemy is dead
        if (health <= 0)
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp(this);
            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }
}
