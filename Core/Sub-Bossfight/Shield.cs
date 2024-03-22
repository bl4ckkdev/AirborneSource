// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Vector3 rotation;
    
    void Update()
    {
        transform.rotation *= Quaternion.Euler(rotation * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        transform.DOScale(0f, 0.1f);
        Bossfight.Instance.StopAttacks();
    }
}
