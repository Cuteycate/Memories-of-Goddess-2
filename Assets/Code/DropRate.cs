using UnityEngine;
using System;
using System.Collections.Generic;


public class DropRate : MonoBehaviour
{
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
        float RandomNumber = UnityEngine.Random.Range(0f, 100f);
        foreach(var drop in propDrops)
        {
            if(RandomNumber <= drop.dropRate)
            {
                Instantiate(drop.ItemPrefab,transform.position, Quaternion.identity);
            }
        }
    }

}
