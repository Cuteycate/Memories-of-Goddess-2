using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public float expOnDefeat;

    public bool canDash = true;
    public bool isDashing = false;
    public float dashingPower = 5f;
    public float dashingTime = 2f;
    public float dashingCoolDown = 15f;


    public bool CanFire = true;
    public bool isFiring = false;
    public float FiringCoolDown = 10f;



    public Rigidbody2D target;
    public GameObject floatingtextPrefab;


    Rigidbody2D rigid;
    SpriteRenderer spriter;
    BoxCollider2D coll;
    Animator anim;
    WaitForFixedUpdate wait;
    public TrailRenderer tr;
   
    public Collider2D[] Collider2Ds;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }



    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
       
        Vector2 dirVec = target.position - rigid.position;
        //Debug.Log("DirVec" + dirVec);
        Vector2 nextVec;    
        nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;

        anim.SetFloat("Speed", nextVec.magnitude);
        float distanceToPlayer = dirVec.magnitude;
        //Debug.Log("distanceToPlayer: " + distanceToPlayer);
        if (distanceToPlayer < 4.5f)
        {
            anim.SetTrigger("Cleave");
        }
        
        if ( distanceToPlayer > 5 && distanceToPlayer < 8 && canDash )
        {          
            StartCoroutine(Dash());
        }

        if ( distanceToPlayer >= 8)
        {
            if (CanFire && !isFiring)
            {
                StartCoroutine(FireCoroutine());
            }
        }
    }


    public void CheckSlashBoss()
    {
        Collider2D[] results = new Collider2D[10]; 
        int numResults = Physics2D.OverlapCollider(Collider2Ds[0], new ContactFilter2D(), results);

        for (int i = 0; i < numResults; i++)
        {
            if (results[i].CompareTag("Player"))
            {
                Debug.Log("Da Chem Trung");
                GameManager.instance.Health -= 20;
                break;
            }
        }
    }


    IEnumerator Dash()
    {
        if (target == null || rigid == null || tr == null)
        {   
            yield break;
        }


        canDash = false;
        isDashing = true;
        Vector2 startPosition = rigid.position;
        Vector2 endPosition = target.position;
        float elapsedTime = 0;

        tr.emitting = true;

        while (elapsedTime < dashingTime)
        {
            rigid.position = Vector2.MoveTowards(rigid.position, endPosition, dashingPower * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rigid.velocity = Vector2.zero;
        tr.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }






    IEnumerator KnockBack()
    {
        /*   yield return null; //khựng 1 frame
           yield return new WaitForSeconds(2f); //coroutine 2s hết 2s mới tiếp tục bị knockback bới return null */
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; // khoảng cách enemy - khoảng cách nhân vật
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse); //Truyền knockback ngược về so với PlayerPos
    }


    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        
        if(isDashing)
        {
            return;
        }    

        //spriter.flipX = target.position.x < rigid.position.x;
         if(target.position.x < rigid.position.x)
        {
            transform.localScale = new Vector3(1, 1, 0);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 0);
        }


    }


    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();  
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !GameManager.instance.FinalBossStillAlive)
            return;
        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());
        ShowDamage(collision.GetComponent<Bullet>().damage.ToString());

        if (health > 0)
        {
            anim.SetTrigger("hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            //.. sống,bị trúng
        }
        else
        {
            //Chết
            GameManager.instance.FinalBossStillAlive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            //GameManager.instance.GetExp(this);
            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
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
        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
            //.. sống,bị trúng
        }
        else
        {
            //Chết
            GameManager.instance.FinalBossStillAlive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            //GameManager.instance.GetExp(this);
            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }



    IEnumerator FireCoroutine()
    {
        for ( int i = 0; i < 3; i++)
        {
            isFiring = true;
            Vector3 targetPos = target.position;
            Vector3 dir = targetPos - transform.position;
            Transform targett = target.transform;


            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get(13).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.right, dir);
            bullet.GetComponent<BulletEnemy>().Init(0, dir, targett, true);
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
            // Delay viên đạn để ko stack lên nhau
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(FiringCoolDown);
        isFiring = false;

    }





}
