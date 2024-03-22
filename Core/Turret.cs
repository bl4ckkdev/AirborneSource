// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform bullet;
    public GameObject gunTip;

    public float cooldown;
    public float bulletSpeed;
    public float offset;
    public AudioSource crashSFX, shootSFX;
    public bool targetPlayer;

    public bool isEditor = true;
    public ObjectComponent objComp;
    public bool freeze;
    public GameObject player;

    public Material tipMat;
    public GameObject tip;
    [ColorUsage(false, true)]
    public Color origin, about, shot;
    private void Awake()
    {
        tipMat = tip.GetComponent<Renderer>().material;
    }
    public void Start()
    {
        
        crashSFX = GameObject.FindGameObjectWithTag("crashSFX").GetComponent<AudioSource>();
        shootSFX = GameObject.FindGameObjectWithTag("shootSFX").GetComponent<AudioSource>();
        if (isEditor) player = GetComponent<ObjectComponent>().player;
        if (freeze) return;
        
        if (_cooldown != null) StopCoroutine(_cooldown);
        if (_offset != null) StopCoroutine(_offset);
        if (offset > 0)
        {
            _offset = StartCoroutine("Offset");
        }
        else
        {
            _cooldown = StartCoroutine("Cooldown");
        }
        
        
    }

    public Coroutine _cooldown;
    IEnumerator Cooldown()
    {
        if (freeze) yield return null;
        
         GameObject newobj = Instantiate(bullet, gunTip.transform.position, gunTip.transform.rotation).gameObject;
         
         Bullet newBullet = newobj.GetComponent<Bullet>();
         tipMat.SetColor("_EmissionColor", shot);
         DOTween.To(() => tipMat.GetColor("_EmissionColor"), x => tipMat.SetColor("_EmissionColor", x), origin, 0.1f).SetEase(Ease.InExpo)
             .OnComplete(() =>
             {
                 tipMat.SetColor("_EmissionColor", origin);
                 DOTween.To(() => tipMat.GetColor("_EmissionColor"), x => tipMat.SetColor("_EmissionColor", x), about, cooldown - 0.1f).SetEase(Ease.InExpo);
             });
         
         newBullet.speed = bulletSpeed;
         newBullet.isEditor = isEditor;
         newBullet.crashSFX = crashSFX;
         shootSFX.Play();
         if (!isEditor) newBullet.player = player;
         yield return new WaitForSeconds(cooldown);
         this.DOKill();
         
         
         
         
         if (freeze) yield return null;
         ;
         
         _cooldown = StartCoroutine("Cooldown");
         yield break;

    }
    
    public Coroutine _offset;
    IEnumerator Offset()
    {
        if (freeze) yield return null;
        DOTween.To(() => tipMat.GetColor("_EmissionColor"), x => tipMat.SetColor("_EmissionColor", x), about, offset).SetEase(Ease.InExpo);
        yield return new WaitForSeconds(offset);
        if (freeze) yield return null;
        if (_cooldown != null) StopCoroutine(_cooldown);
        
        _cooldown = StartCoroutine("Cooldown");
    }

    private void Update()
    {
        if (!freeze)
        {
            if (targetPlayer)
            {
                transform.LookAt(player.transform.position, Vector3.forward);
                transform.rotation *= Quaternion.Inverse(Quaternion.Euler(0, -90, 0));
            }
        }

    }
}
