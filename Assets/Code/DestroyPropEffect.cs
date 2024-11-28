using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPropEffect : MonoBehaviour
{
    public float TimeToDisable = 1f;
    float timer;

    private void OnEnable()
    {
        timer = TimeToDisable;
    }
    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            Destroy(gameObject);
        }
    }
}
