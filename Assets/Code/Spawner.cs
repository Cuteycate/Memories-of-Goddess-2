using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Update is called once per frame
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    int level;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
        timer += Time.deltaTime; //Timer tính giờ
        level =Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10f),spawnData.Length -1); //Mỗi 10s sẽ tăng 1 level 
        if (timer > spawnData[level].spawnTime)  //Khi timer > level thì qua cái khác (vd element 0 chạy đc 10s spawntime 0.7 -> element 1 chạy 10s tiếp theo spawntime 0.2
        {
            timer = 0;
            Spawn();
        }
    }
    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0); //Chỉ có 1 enemy trong pool nên đổi luôn thành 0
        // GameObject enemy = GameManager.instance.pool.Get(Random.Range(0, 2));
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; //Spawn random theo 14 điểm spawnPoint trong SampleScene
        enemy.GetComponent<Enemy>().Init(spawnData[level]); //Lấy dữ liệu đầu vào từ Enemy
    }
    [System.Serializable]
    public class SpawnData
    {
        public float spawnTime; //thời gian spawn 
        public int spriteType; // sprite của nhân vật được truyền trong enemy (kiểm tra prefab của enemy coi phần anim controller)
        public int health; // Máu của quái
        public float speed; //tốc độ quái
        public int expOnDefeat;
    }
}
