// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using UnityEngine;

public class BigTurret : MonoBehaviour
{
    private int _ammo = 10;
    public int hit;
    
    public Transform bullet, bulletTip;
    public AudioSource crashSFX, shootSFX;

    public Material mat;

    public static BigTurret Instance;

    private void Start()
    {
        Instance = this;

        mat = transform.GetChild(0).GetChild(1).gameObject.GetComponent<Renderer>().material;
        
        shootSFX = Bossfight.Instance.shootSFX;
        crashSFX = Bossfight.Instance.crashSFX;
    }

    void Update()
    {
        transform.DOLookAt(Bossfight.Instance.cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 50)), 0.5f);
        if (Input.GetButtonDown("Fire1") && _ammo > 0)
        {
            _ammo--;
            
            shootSFX.Play();
            
            Bullet b = Instantiate(bullet, bulletTip.position, transform.rotation).GetComponent<Bullet>();

            b.crashSFX = crashSFX;
            b.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            b.finale = true;
            b.speed = 12;
            
            if (_ammo <= 0) 
            {
                mat.SetColor("_EmissionColor", Color.red);
                Bossfight.Instance.StopAttacks();
                Bossfight.Instance.DoSomething();
            }
        }
    }
}
