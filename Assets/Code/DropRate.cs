using UnityEngine;
using System;
using System.Collections.Generic;


public class DropRate : MonoBehaviour
{
    public static GameManager instance;
    [Serializable]
    public class PropDrop
    {
        public string name;
        public GameObject ItemPrefab;
        public float dropRate;
    }

    public List<PropDrop> propDrops;
    private void OnDestroy()
    {
        if (!GameManager.instance.isLive)
            return;
            float RandomNumber = UnityEngine.Random.Range(0f, 100f);
            foreach (var drop in propDrops)
            {
                if (RandomNumber <= drop.dropRate)
                {
                    Instantiate(drop.ItemPrefab, transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }