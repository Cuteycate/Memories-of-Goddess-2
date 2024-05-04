using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    private Coroutine healthRecoveryCoroutine;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    //weapon Scythe test
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
    }
    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.PlayerId];
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        if (inputVec.x != 0)
        {
            lastHorizontalVector = inputVec.x;
        }
        if (inputVec.y != 0)
        {
            lastVerticalVector = inputVec.y;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    void OnMove(InputValue value)
    {
        if (!GameManager.instance.isLive)
            return;
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        anim.SetFloat("Speed", inputVec.magnitude);
        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;

        if (collision.gameObject.CompareTag("Turet"))
        {
            return;
        }
        GameManager.instance.Health -= Time.deltaTime * 10;

        if (GameManager.instance.Health < 0)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.instance.isLive)
            return;
        if (collision.CompareTag("BulletE"))
        {
            GameManager.instance.Health -= Time.deltaTime * 10;
        }
        if (GameManager.instance.Health < 0)
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }



    public void StartHealthRecovery(float recoveryRate)
    {
        if (healthRecoveryCoroutine != null)
        {
            StopCoroutine(healthRecoveryCoroutine);
        }
        healthRecoveryCoroutine = StartCoroutine(HealthRecoveryCoroutine(recoveryRate));
    }
    public void StopHealthRecovery()
    {
        if (healthRecoveryCoroutine != null)
        {
            StopCoroutine(healthRecoveryCoroutine);
            healthRecoveryCoroutine = null;
        }
    }
    private IEnumerator HealthRecoveryCoroutine(float recoveryRate)
    {
        while (true)
        {
            GameManager gameManager = GameManager.instance;
            if (gameManager != null && gameManager.Health < gameManager.MaxHealth)
            {
                gameManager.Health = Mathf.Min(gameManager.Health + recoveryRate, gameManager.MaxHealth);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
