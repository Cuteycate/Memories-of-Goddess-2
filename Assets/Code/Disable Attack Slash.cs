using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAttackSlash : MonoBehaviour
{
    public float TimeToDisable = 0.3f;
    float timer;

    private void OnEnable()
    {
        timer = TimeToDisable;
    }
    private void LateUpdate()
    {
        timer -=Time.deltaTime;
        if(timer < 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
