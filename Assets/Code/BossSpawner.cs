using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public bool bossSpawnedAt300; 
    public bool bossSpawnedAt600; 
    public int level;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        timer += Time.deltaTime; // Timer tính giờ
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 20f), spawnData.Length - 1); // Mỗi 10s sẽ tăng 1 level 

        // Spawn boss after 300 seconds (5 minutes) if it hasn't been spawned yet
        if (!bossSpawnedAt300 && GameManager.instance.gameTime >= 0f && GameManager.instance.gameTime < 600f)
        {
            SpawnBoss();
            bossSpawnedAt300 = true;
        }

        // Spawn boss after 600 seconds (10 minutes) if it hasn't been spawned yet
        if (!bossSpawnedAt600 && GameManager.instance.gameTime >= 600f)
        {
            SpawnBoss();
            bossSpawnedAt600 = true; 
        }
    }

    void SpawnBoss()
    {
        GameObject boss = GameManager.instance.pool.Get(7); // Chỉ có 1 boss trong pool nên đổi luôn thành 1
        boss.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // Spawn random theo 14 điểm spawnPoint trong SampleScene
        boss.GetComponent<BossEnemy>().Init(spawnData[level], false); // Lấy dữ liệu đầu vào từ Enemy
    }

    [System.Serializable]
    public class SpawnData
    {
        public int spriteType; // Sprite của nhân vật được truyền trong enemy (kiểm tra prefab của enemy coi phần anim controller)
        public int health; // Máu của quái
        public float speed; // Tốc độ quái
        public int expOnDefeat;
    }
}
