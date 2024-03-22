// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWall : MonoBehaviour
{
    public GameObject other;
    
    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && col.transform.position.y > -5)


            if (transform.position.x < 0)
            {
                transform.DOMoveX(-25, 0.2f);
                other.transform.DOMoveX(21.75f, 0.2f);
            }

            else
            {
                transform.DOMoveX(25, 0.2f);
                other.transform.DOMoveX(-21.75f, 0.2f);

            }
       
    }
}
