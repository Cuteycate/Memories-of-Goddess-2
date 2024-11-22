using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickUp : MonoBehaviour
{
    public float expAmount; // Kinh nghiem
    public SpriteRenderer spriteRenderer;
    public List<Sprite> xpSprites; //Danh sách Sprites
    public float magnetSpeed = 5f; // Tốc độ Magnet
    public float mergeRadius = 0.5f; // Ban Kinh ket hop exp
    private Transform playerTransform;
    public RuntimeAnimatorController[] animControllers;
    public Animator animator;
    private bool isBeingPulled = false;

    private bool isMerging = false; //EXP co merge duoc hay khong
    private Transform mergeTarget; //Muc tieu merge
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        SetAnimationBasedOnExp();
        StartCoroutine(MergeCheckCoroutine());
    }
    private IEnumerator MergeCheckCoroutine() // Dung de kiem tra merge moi x giay
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f); // kiem tra merge moi 1.5 giay
            if (!isBeingPulled && GameManager.instance != null && GameManager.instance.isLive)
            {
                CheckForNearbyMerges();
            }
        }
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        if (isBeingPulled && playerTransform != null)
        {
            // di chuyển XP Orb tới vị trí của người chơi
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, magnetSpeed * Time.deltaTime);
        }
        if (isMerging && mergeTarget != null && !isBeingPulled)
        {
            transform.position = Vector2.MoveTowards(transform.position, mergeTarget.position, magnetSpeed * Time.deltaTime);

            // merge kinh nghiem lai khi den du gan
            if (Vector2.Distance(transform.position, mergeTarget.position) < 0.1f)
            {
                CompleteMerge();
            }
        }
    }
    public void SetAnimationBasedOnExp()
    {
        int tier = GetSpriteTier(expAmount);
        if (animControllers != null && tier < animControllers.Length)
        {
            animator.runtimeAnimatorController = animControllers[tier]; // Dựa vào tier để chỉnh sửa drop kinh nghiệm
        }
    }
    public void ResetState(float newExpAmount)
    {
        expAmount = newExpAmount;
        isBeingPulled = false;
        isMerging = false;
        playerTransform = null;
        mergeTarget = null;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        SetAnimationBasedOnExp(); // Reset Animation
        gameObject.SetActive(true);
    }
    public void SetMergeAnimationBasedOnExp(float totalExp)
    {
        int newTier = GetSpriteTier(totalExp);
        if (animControllers != null && newTier < animControllers.Length)
        {
            animator.runtimeAnimatorController = animControllers[newTier];
        }
    }
    int GetSpriteTier(float amount)
    {
        if (amount >= 1 && amount <= 5) return 0;
        if (amount >= 6 && amount <= 10) return 1;
        if (amount >= 11 && amount <= 15) return 2;
        return 3;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CircleCollider"))
        {
            isBeingPulled = true;
            playerTransform = other.transform;
        }
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = GameManager.instance;

            if (gameManager != null && gameManager.isLive)
            {
                // Truyen XP cua nguoi choi dua vao chi so
                // cong thuc exp = exp quai * ti le tu Item * ti le tu multiplier shop 
                // Co the sai neu nhu sai thi kho noi :(((( t luoi test vai dai ra 
                gameManager.exp += expAmount * (1 + gameManager.ExtraRateExp) * ShopStats.Instance.xpMultiplier;

                // Kiem tra level up
                if (gameManager.exp >= gameManager.nextExp[Mathf.Min(gameManager.level, gameManager.nextExp.Length - 1)])
                {
                    gameManager.level++;
                    gameManager.exp = 0;
                    gameManager.uiLevelUp.Show();
                }

                //Tieng game Work In Progress
                // AudioManager.instance.PlaySfx(AudioManager.Sfx.ExpPickup);
                // Pha XP sau khi nguoi choi nhat dc
                gameObject.SetActive(false);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CircleCollider"))
        {
            isBeingPulled = false;
            playerTransform = null;
        }
    }
    public void CheckForNearbyMerges()
    {
        Collider2D[] nearbyPickups = Physics2D.OverlapCircleAll(transform.position, mergeRadius);
        List<ExpPickUp> mergeablePickups = new List<ExpPickUp>();
        float totalExp = expAmount;
        int currentTier = GetSpriteTier(expAmount);
        float expNeeded = GetExpForNextTier(currentTier) - expAmount; // Tính EXP cần để qua tier tiếp theo

        // Filter pickup xung quanh
        foreach (Collider2D collider in nearbyPickups)
        {
            if (collider != null && collider.gameObject != gameObject && collider.CompareTag("ExpPickUp"))
            {
                ExpPickUp otherExpPickUp = collider.GetComponent<ExpPickUp>();
                if (otherExpPickUp != null && !otherExpPickUp.isMerging)
                {
                    int otherTier = GetSpriteTier(otherExpPickUp.expAmount);

                    // Chỉ merge nếu như cùng tier
                    if (otherTier == currentTier)
                    {
                        mergeablePickups.Add(otherExpPickUp);
                    }
                }
            }
        }

        // Sort những Pickups tăng dần
        mergeablePickups.Sort((a, b) => a.expAmount.CompareTo(b.expAmount));

        // Chỉ merge đủ không merge dư
        List<ExpPickUp> selectedPickups = new List<ExpPickUp>();
        foreach (ExpPickUp pickup in mergeablePickups)
        {
            if (expNeeded <= 0) break;

            selectedPickups.Add(pickup);
            expNeeded -= pickup.expAmount;
        }

        if (selectedPickups.Count > 0 && expNeeded <= 0)
        {
            // Bắt đầu merge
            foreach (ExpPickUp pickup in selectedPickups)
            {
                pickup.isMerging = true; // Mark là đang bị merge
                pickup.mergeTarget = this.transform; // Đặt Target merge
                StartCoroutine(PullPickupToMerge(pickup)); // Kéo merge vào target
                totalExp += pickup.expAmount; // Cập nhật tổng exp
            }

            expAmount = totalExp; // cập nhật kinh nghiệm
            SetMergeAnimationBasedOnExp(totalExp); // Cập nhật Sprite
        }
    }

    private float GetExpForNextTier(int tier)
    {
        // Tính EXP cho mỗi tier trong th này tier 0 = 6 tier 1 = 11 tier 2 = 16 có thể chỉnh sửa ở phía trên dựa vào new tier
        if (tier == 0) return 6f; 
        if (tier == 1) return 11f; 
        if (tier == 2) return 16f;
        return float.MaxValue; 
    }
    private IEnumerator PullPickupToMerge(ExpPickUp pickup)
    {
        while (pickup != null && Vector2.Distance(pickup.transform.position, transform.position) > 0.1f)
        {
            pickup.transform.position = Vector2.MoveTowards(pickup.transform.position, transform.position, magnetSpeed * Time.deltaTime);
            yield return null;
        }

        if (pickup != null)
        {
            pickup.gameObject.SetActive(false); // xoa sau khi merge
        }
    }
    void CompleteMerge()
    {
        isMerging = false;
        mergeTarget = null;
    }
    private void OnDrawGizmosSelected() // Dung de debug
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mergeRadius);
    }
}
