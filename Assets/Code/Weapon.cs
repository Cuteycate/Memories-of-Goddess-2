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
                default:
                    break;

            }
            if (Input.GetButtonDown("Jump"))
            {
                LevelUp(10, 1,1);
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

            for(int i=0; i<GameManager.instance.pool.prefabs.Length;i++)
            {
                if(data.projectiles == GameManager.instance.pool.prefabs[i])
                {
                    prefabId = i;
                    break;
                }
            }
            switch (id)
            {
                case 0:
                    speed = 150 *Character.WeaponSpeed;
                    Batch();
                    batchCoroutine = StartCoroutine(ToggleBatchCoroutine());
                break;
                case 1:
                    speed = 2f * Character.WeaponRate;
                    break;
                default:
                    break;
            }
            Hand hand = player.hands[(int)data.itemType];
            hand.spriter.sprite = data.hand;
            hand.gameObject.SetActive(true);
            player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
        }
        public void LevelUp(float damage, int count,int penetration)
        {
            this.damage = damage * Character.Damage;
            this.count += count;
            this.penetration += penetration;
            if (id == 0)
                Batch();
            player.BroadcastMessage("ApplyGear",SendMessageOptions.DontRequireReceiver);
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
                bullet.Translate(bullet.up * 1.5f, Space.World);
                bullet.GetComponent<Bullet>().Init(damage, penetration, Vector3.zero,count);
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
                    yield break;
                Vector3 targetPos = player.scanner.nearestTarget.position;
                Vector3 dir = targetPos - transform.position;
                dir = dir.normalized;

                Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.position = transform.position;
                bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                bullet.GetComponent<Bullet>().Init(damage,penetration, dir, i);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
                yield return new WaitForSeconds(0.1f); // Delay viên đạn để ko stack lên nhau
            }
        count = count - initialCount;
    }
    }
