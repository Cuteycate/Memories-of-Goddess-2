using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Spawner : MonoBehaviour
{
    // Update is called once per frame
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public GameObject PointWave;

    float[] Rotation = { 0, 45, 90, 135, 180, 225, 270, 315, 160 };

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
               
                StartCoroutine(SpawnEvent(spawnData[level].spawnTime, level, spawnData[level].TypeEvent ));            
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

    IEnumerator SpawnEvent(float time, int level, int TypeEvent)
    {
       if ( TypeEvent == 1)
        {
            for (int i = 0; i < spawnData[level].CountEvent; i++)
            {
                int ran;
                ran = Random.Range(0, Rotation.Length);
                GameObject Wave = GameManager.instance.pool.Get(8);
                GameObject PointWave = GameManager.instance.pool.Get(11);
                PointWave.transform.position = GameManager.instance.player.transform.position;
                PointWave.transform.Rotate(0, 0, Rotation[ran]);
                Transform[] childTransforms = GetChildPositions(PointWave);
                Wave.transform.position = childTransforms[1].position;             
                Wave.GetComponent<EventWave>().Inti(childTransforms[2], childTransforms[1], TypeEvent);
                yield return new WaitForSeconds(3f);
            
            }
        }
        else if (TypeEvent == 2)
        {

            GameObject Wave = GameManager.instance.pool.Get(10);
            Wave.transform.position= GameManager.instance.player.transform.position;
            Wave.GetComponent<EventWave>().Inti(GameManager.instance.player.transform, null, TypeEvent);

        }
        yield return new WaitForSeconds(3f);
    }

    Transform[] GetChildPositions(GameObject parentObject)
    {
        Transform[] childTransforms = parentObject.GetComponentsInChildren<Transform>();      
        return childTransforms;
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
        public int TypeEvent;

    }
}
