using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    SpriteRenderer player;
    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);
    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }
    void LateUpdate()
    {
        bool isReversed = player.flipX;

        if(isLeft) //Bên Trái
        {
            transform.localRotation = isReversed ? leftRotReverse : leftRot;
            spriter.flipY = isReversed;
            spriter.sortingOrder = isReversed ? 4 : 6;
        }
        else //Bên Phải
        {
            transform.localPosition = isReversed ? rightPosReverse : rightPos;
            spriter.flipX = isReversed;
            spriter.sortingOrder = isReversed ? 6 : 4;
        }
    }
}
