using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    void Start()
    {
        Init();
    }
    void Update()
    {
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime); //Làm cho vũ khí xoay theo chiều kim đồng hồ
                break;
            default:
                break;
             
        }
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 2);
        }
    }
    public void Init()
    {
        switch(id)
        {
            case 0:
                speed = 150;
                Batch();
                break;
            default:
                break;
        }
    }
    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;
        if (id == 0)
            Batch();
    }
    void Batch()
    {
        for(int i = 0; i < count; i++) //Count số lượng projectiles count ở đây là lương projectiles
        {
            Transform bullet;
            if(i < transform.childCount)
            {
                bullet = transform.GetChild(i); 
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            bullet.localPosition = Vector3.zero;  // Đặt vị trí cục bộ của viên đạn về (0, 0, 0)
            bullet.localRotation = Quaternion.identity; // Đặt quay cục bộ của viên đạn về không quay
            Vector3 rotVec = Vector3.forward * 360 * i / count; // (0,0,1) * 360 * i/count
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World); //đẩy lên 1.5f dựa theo world
            bullet.GetComponent<Bullet>().Init(damage, -1); //-1 là infinity per để khi chạm vào quái nếu còn trong người quái thì sẽ ko gây dame liên tục đc


        }
    }
}
