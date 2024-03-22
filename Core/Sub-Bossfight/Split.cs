// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using UnityEngine;

public class Split : MonoBehaviour
{
    public GameObject player;
    public float accuracy;
    public Vector3 offset;
    public AxisConstraint axis;
    public void OnEnable()
    {
        Invoke("StartShooting", 1f);
        GetComponent<Turret>().freeze = true;
    }
    public bool shoot;

    public void StartShooting()
    {
        
        shoot = true;
        GetComponent<Turret>().freeze = false;
        GetComponent<Turret>().Start();
        
    }

    private void Update()
    {
        if (shoot)
        {
            transform.DOLookAt(player.transform.position, accuracy, axis, offset);
        }
    }
    private void LateUpdate()
    {
        transform.position -= Vector3.forward * transform.position.z;
    }
}
