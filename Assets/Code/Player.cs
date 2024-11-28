using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

    //Dash
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashinhTime = 0.2f;
    private float dashingCooldown = 5f;
    [SerializeField] private Slider skillcooldown;

    [SerializeField] private TrailRenderer tr;

    //DoubleWeapon
    [HideInInspector]
    public List<Weapon> weapons = Item.ListWeapon;
    private bool canDW = true;
    [HideInInspector]
    public bool isDWing;
    private float DWtime = 5f;
    private float DWCooldown = 25f;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);

        if (skillcooldown != null && GameManager.instance.PlayerId == 0)
        {
            skillcooldown.maxValue = dashingCooldown;
            skillcooldown.value = dashingCooldown; // Slider starts full
        }
        if (skillcooldown != null && GameManager.instance.PlayerId == 1)
        {
            skillcooldown.maxValue = DWCooldown;
            skillcooldown.value = DWCooldown; // Slider starts full
        }

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
        if (isDashing)
        {
            return;
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && canDash && GameManager.instance.PlayerId == 0)
        {
            StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(KeyCode.Space) && canDW && GameManager.instance.PlayerId == 1)
        {
            StartCoroutine(DoubleCountWeapon());
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        if (isDashing)
        {
            return;
        }
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
        if (isDashing)
        {
            return;
        }
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
        CapsuleCollider2D playerCollider = GetComponent<CapsuleCollider2D>();
        if (collision.otherCollider != playerCollider)
            return;
        if (collision.gameObject.CompareTag("Turet"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("Prop"))
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


    IEnumerator KnockBack()
    {
        /*   yield return null; //khựng 1 frame
           yield return new WaitForSeconds(2f); //coroutine 2s hết 2s mới tiếp tục bị knockback bới return null */
        yield return 1f;
        Vector3 playerPos = scanner.transform.position;
        Vector3 dirVec = transform.position - playerPos; // khoảng cách enemy - khoảng cách nhân vật
        rigid.AddForce(dirVec.normalized * 20, ForceMode2D.Impulse); //Truyền knockback ngược về so với PlayerPos
    }


    public void StartHealthRecovery(float gearRecoveryRate)
    {
        float shopRecoveryRate = ShopStats.Instance.healthrecoveryMultiplier;
        if (healthRecoveryCoroutine != null)
        {
            StopCoroutine(healthRecoveryCoroutine);
        }
        healthRecoveryCoroutine = StartCoroutine(HealthRecoveryCoroutine(gearRecoveryRate, shopRecoveryRate));
    }
    public void StopHealthRecovery()
    {
        if (healthRecoveryCoroutine != null)
        {
            StopCoroutine(healthRecoveryCoroutine);
            healthRecoveryCoroutine = null;
        }
    }
    private IEnumerator HealthRecoveryCoroutine(float gearRecoveryRate, float shopRecoveryRate)
    {
        while (true)
        {
            GameManager gameManager = GameManager.instance;
            if (gameManager != null && gameManager.Health < gameManager.MaxHealth)
            {
                // Tinh tong tu gear + shop
                float totalRecoveryRate = gearRecoveryRate + shopRecoveryRate;
                gameManager.Health = Mathf.Min(gameManager.Health + totalRecoveryRate, gameManager.MaxHealth);
            }
            // Moi 1 giay thi hoi nhu totalrecoveryrate mau.
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0;
        Vector2 dashDirection = inputVec.normalized;
        rigid.velocity = dashDirection * dashingPower;
        tr.emitting = true;
        // Chờ hết thời gian Dash
        yield return new WaitForSeconds(dashinhTime);
        tr.emitting = false;
        // Kết thúc Dash
        rigid.gravityScale = originalGravity;
        rigid.velocity = Vector2.zero; // Dừng lại sau Dash
        isDashing = false;
        // Chờ thời gian hồi chiêu trước khi cho phép Dash lần tiếp theo
        //Hien thi thoi gian cho tren slider
        float cooldownTimer = 0;
        while (cooldownTimer < dashingCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (skillcooldown != null)
            {
                skillcooldown.value = cooldownTimer;
            }
            yield return null;
        }

        if (skillcooldown != null)
        {
            skillcooldown.value = dashingCooldown;
        }
        canDash = true;
    }
    private IEnumerator DoubleCountWeapon()
    {
        canDW = false;
        isDWing = true;
        foreach (Weapon weapon in weapons)
        {
            if (weapon.id == 0)
                weapon.BroadcastMessage("Batch", SendMessageOptions.DontRequireReceiver);
            else
            {
                weapon.count *= 2;
                weapon.ExtraCount *= 2;
            }
        }
        // Kết thúc x2 weapon
        skillcooldown.value = 0;
        yield return new WaitForSeconds(DWtime);
        // Dừng lại sau x2 weapon
        isDWing = false;
        foreach (Weapon weapon in weapons)
        {
            if (weapon.id == 0)
                weapon.BroadcastMessage("Batch", SendMessageOptions.DontRequireReceiver);
            else
            {
                weapon.count /= 2;
                weapon.ExtraCount /= 2;
            }
        }
        // Chờ thời gian hồi chiêu trước khi cho phép Dash lần tiếp theo
        //Hien thi thoi gian cho tren slider
        float cooldownTimer = 0;
        while (cooldownTimer < DWCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (skillcooldown != null)
            {
                skillcooldown.value = cooldownTimer;
            }
            yield return null;
        }

        if (skillcooldown != null)
        {
            skillcooldown.value = DWCooldown;
        }
        canDW = true;
    }

}
