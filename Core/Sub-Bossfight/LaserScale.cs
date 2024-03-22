// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScale : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("Grow", 4);
    }

    public void Grow()
    {
        transform.DOScaleX(50, 1f);
    }
}
