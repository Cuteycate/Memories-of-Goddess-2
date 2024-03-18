using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lớp PoolManager quản lý việc pool đối tượng cho nhiều prefab.
public class PoolManager : MonoBehaviour
{
    // Mảng các prefab để pool.
    public GameObject[] prefabs;

    // Danh sách các pool, mỗi pool lưu trữ các thể hiện của một prefab cụ thể.
    List<GameObject>[] pools;

    // Awake được gọi khi phiên bản script đang được tải.
    void Awake()
    {
        // Khởi tạo mảng pools có cùng độ dài với mảng prefabs.
        pools = new List<GameObject>[prefabs.Length];

        // Lặp qua từng prefab.
        for (int i = 0; i < pools.Length; i++)
        {
            // Tạo một danh sách mới để lưu trữ các thể hiện của prefab hiện tại.
            pools[i] = new List<GameObject>();
        }

        // Xuất độ dài của mảng pools ra console để debug.
        Debug.Log(pools.Length);
    }

    public GameObject Get(int i)
    {
        GameObject select = null;
        foreach(GameObject item in pools[i])
        {
            if(!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if(!select)
        {
            select = Instantiate(prefabs[i],transform);
            pools[i].Add(select);
        }
        return select;
    }
}
