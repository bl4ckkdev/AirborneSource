// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAround : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Moving>().points[0].to.y = transform.position.y;
    }
}
