// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBoss : MonoBehaviour
{
    private void Update()
    {
        transform.position = new Vector3(Bossfight.Instance.transform.position.x, 0, 0);
    }
}
