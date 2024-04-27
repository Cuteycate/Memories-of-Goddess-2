using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNumber : MonoBehaviour
{
    private float secondtodestroy = 1f;
    void Start()
    {
        Destroy(gameObject,secondtodestroy);
    }
}
