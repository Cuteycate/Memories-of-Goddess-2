using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public int penetration;
    public float speed;
    public int ExtraCount;
    float timer;
    private bool isBatchEnabled = false;
    public float MeleeCoolDown = 3f;

    public GameObject hiteffect;
    private Coroutine batchCoroutine;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime); // Cho vũ khí xoay ngược chiều kim đồng hồ
                break;
            case 1:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    StartCoroutine(FireCoroutine()); // Bắt đầu bắn
                }
                break;
            case 8:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;  // Bắt đầu bắn
                    FireShotgun();
                }
                break;
            case 9:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;  // Bắt đầu bắn
                    StartCoroutine(SniperFireCoroutine());
                }
                break;
            case 10:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    MeleeAttack();
                }
                break;
            case 11:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    LightningAttack();
                }
                break;
            default:
                break;

        }
    }
    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count + ExtraCount;
        penetration = data.basePenetration;
        //hiteffect
        hiteffect = data.HitEffect;
        for (int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
        {
            if (data.projectiles == GameManager.instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }
        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                Batch();
                batchCoroutine = StartCoroutine(ToggleBatchCoroutine());
                break;
            case 1:
            case 8:
            case 10:
                speed = 3f * Character.WeaponRate;
                break;
            case 9:
                speed = 7f * Character.WeaponRate;
                break;
            case 11:
                speed = 3f * Character.WeaponRate;
                break;
            default:
                break;
        }
        if ((int)data.itemType == 0 || (int)data.itemType == 1)
        {
            Hand hand = player.hands[(int)data.itemType];
            hand.spriter.sprite = data.hand;
            hand.gameObject.SetActive(true);
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
    public void LevelUp(float damage, int count, int penetration)
    {
        this.damage = damage * Character.Damage;
        this.count += count;
        this.penetration += penetration;
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
        if (id == 0)
            Batch();
    }
    IEnumerator ToggleBatchCoroutine()
    {
        while (true)
        {
            float cooldownDuration = isBatchEnabled ? 3f : MeleeCoolDown;

            yield return new WaitForSeconds(cooldownDuration);

            if (isBatchEnabled)
            {
                DisableBatch();
            }
            else
            {
                EnableBatch();
            }
        }
    }

    void EnableBatch()
    {
        isBatchEnabled = true;
        // Enable all children
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    void DisableBatch()
    {
        isBatchEnabled = false;
        // Disable all children
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    void Batch()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        count = count + ExtraCount;
        for (int i = 0; i < count; i++)
        {
            Transform bullet;
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            Vector3 rotVec = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 3f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, penetration, Vector3.zero, count);
        }
        count = count - ExtraCount;
    }

    IEnumerator FireCoroutine()
    {
        int initialCount = ExtraCount;
        count = count + initialCount;
        for (int i = 0; i < count; i++)
        {
            if (!player.scanner.nearestTarget)
            {
                count = count - initialCount;
                yield break;
            }
            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, penetration, dir, i);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
            yield return new WaitForSeconds(0.1f);
        }
        count = count - initialCount;
    }
    void FireShotgun()
    {
        if (!player.scanner.nearestTarget)
        {
            return;
        }
        int initialCount = ExtraCount;
        count = count + initialCount;
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        for (int i = 0; i < count; i++)
        {
            Vector3 perpendicularDir = Vector3.Cross(dirToTarget, Vector3.up).normalized;
            Vector3 spreadDir = dirToTarget + perpendicularDir * Random.Range(-0.4f, 0.4f);

            spreadDir = Quaternion.Euler(0, -45f, 0) * spreadDir;

            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dirToTarget);
            bullet.GetComponent<Bullet>().Init(damage, penetration, spreadDir, i);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
        count = count - initialCount;
    }
    IEnumerator SniperFireCoroutine()
    {
        int initialCount = ExtraCount;
        count = count + initialCount;
        for (int i = 0; i < count; i++)
        {
            if (!player.scanner.farthestTarget)
            {
                count = count - initialCount;
                yield break;
            }
            Vector3 targetPos = player.scanner.farthestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            bullet.GetComponent<Bullet>().Init(damage, penetration, dir, i);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
            yield return new WaitForSeconds(0.5f);
        }
        count = count - initialCount;
    }
    void MeleeAttack()
    {
        float temp = 1f + (float)count * 0.2f;
        if (!player.scanner.nearestTarget)
            return;
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;
        Quaternion rotation;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;

        if (player.lastHorizontalVector > 0)
        {
            rotation = Quaternion.Euler(0f, 0f, 0);
        }
        else
        {
            rotation = Quaternion.Euler(0f, 0f, 180f);
        }
        bullet.localScale = new Vector3(temp, temp, 1f); //tang Scale Cua SLash
        bullet.rotation = rotation; //Xoay Slash
        bullet.Translate(bullet.right * 3f, Space.World); //Khaong Cach Slash so voi nguoi choi
        bullet.GetComponent<Bullet>().Init(damage, penetration, dir, 1); // Tao Slash
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);

    }

    HashSet<Transform> enemiesHit = new HashSet<Transform>(); // HashSet to store enemies hit by lightning

    void LightningAttack()
    {
        int initialCount = ExtraCount;
        int TempCount = count + initialCount;
        // Find the nearest enemies to the player
        RaycastHit2D[] hitResults = GameManager.instance.player.GetComponent<Scanner>().targets;
        List<Transform> targetTransforms = new List<Transform>();

        foreach (RaycastHit2D hitResult in hitResults)
        {
            if (hitResult.transform != null)
            {
                targetTransforms.Add(hitResult.transform);
            }
        }

        Transform[] targetEnemies = targetTransforms.ToArray();

        // Check if there are any enemies found
        if (targetEnemies != null && targetEnemies.Length > 0)
        {
            // Define the lightning bolt effect prefab
            GameObject lightningBoltEffectPrefab = hiteffect;

            // Define the damage and range of the lightning attack
            float damage = this.damage;
            float radius = 1f;
            int temp;
            // Limit the count to the number of enemies if count exceeds the number of enemies
            temp = Mathf.Min(TempCount, targetEnemies.Length);

            // Loop through the count and spawn lightning bolts on each enemy
            for (int i = 0; i < temp; i++)
            {
                // Instantiate the lightning bolt effect at the position of the target enemy
                GameObject lightningBolt = Instantiate(lightningBoltEffectPrefab, targetEnemies[i].position, Quaternion.identity);

                // Perform an area attack around the lightning bolt's position
                AreaAttack(lightningBolt.transform.position, radius, damage);
            }
        }
        else
        {
            // If no enemy is found, apply cooldown or wait before attempting another attack
            StartCoroutine(CooldownCoroutine());
        }
    }


    void AreaAttack(Vector3 center, float radius, float damage)
    {
        // Detect all colliders within the specified radius around the center position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);

        // Loop through all colliders found in the radius
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Get the enemy component and deal damage to it
                Enemy enemy = collider.GetComponent<Enemy>();
                BossEnemy bossEnemy = collider.GetComponent<BossEnemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);

                }
                else if (bossEnemy != null)
                {
                    bossEnemy.TakeDamage(damage);

                }
            }
            if (collider.CompareTag("Prop"))
            {
                PropBreak prop = collider.GetComponent<PropBreak>();
                if (prop != null)
                {
                    prop.TakeDamage(damage);
                }
            }
        }


    }




    IEnumerator CooldownCoroutine()
    {
        // Apply a cooldown or wait before attempting another attack
        yield return new WaitForSeconds(this.speed); // Adjust the cooldown duration as needed

        // Xoa Ke thu bi danh de LightNing hoat dong tot
        enemiesHit.Clear();

        // After cooldown, retry the lightning attack
        LightningAttack();
    }





}
