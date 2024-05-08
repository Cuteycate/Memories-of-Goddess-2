using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Spawner : MonoBehaviour
{
    // Update is called once per frame
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;



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
        timer += Time.deltaTime; //Timer tính giờ
        level =Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 30f),spawnData.Length -1); //Mỗi 10s sẽ tăng 1 level 
        if (timer > spawnData[level].spawnTime)  //Khi timer > level thì qua cái khác (vd element 0 chạy đc 10s spawntime 0.7 -> element 1 chạy 10s tiếp theo spawntime 0.2
        {
            timer = 0;
            Spawn();          
            if (spawnData[level].EventWave)
            {
               
                StartCoroutine(SpawnEvent(spawnData[level].spawnTime, level));            
                spawnData[level].EventWave = false;
            }      
        }
    }
    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0); //Chỉ có 1 enemy trong pool nên đổi luôn thành 0
        // GameObject enemy = GameManager.instance.pool.Get(Random.Range(0, 2));
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; //Spawn random theo 14 điểm spawnPoint trong SampleScene
        enemy.GetComponent<Enemy>().Init(spawnData[level], false); //Lấy dữ liệu đầu vào từ Enemy
    }

    IEnumerator SpawnEvent( float time, int level )
    {
        for (int i = 0; i < spawnData[level].CountEvent; i++)
        {
    
            GameObject Wave = GameManager.instance.pool.Get(8);
            int ran = Random.Range(1, spawnPoint.Length);
            Wave.transform.position = spawnPoint[ran].position;

            float distance = 0;
            Transform BestSpawnPoint = null;
            foreach (Transform transform in spawnPoint)
            {
                if(transform.position == spawnPoint[ran].position)
                {
                    continue;
                }
                Vector2 ToSpawnPoint = transform.position - spawnPoint[ran].position;
                float DistanceToPlayer = Vector2.Distance(GameManager.instance.player.transform.position, ToSpawnPoint);           
                if (DistanceToPlayer > distance)
                {
                    distance = DistanceToPlayer;
                    BestSpawnPoint = transform;
                }

            }
            Wave.GetComponent<EventWave>().Inti(BestSpawnPoint, spawnPoint[ran]);
            yield return new WaitForSeconds(2f);
            
        }
        yield return new WaitForSeconds(2f);
    }

    private void OnEnable()
    {
        
    }

    [System.Serializable]
    public class SpawnData
    {
        public float spawnTime; //thời gian spawn 
        public int spriteType; // sprite của nhân vật được truyền trong enemy (kiểm tra prefab của enemy coi phần anim controller)
        public int health; // Máu của quái
        public float speed; //tốc độ quái
        public int expOnDefeat;
        public bool EventWave;
        public int CountEvent;
    }
}
