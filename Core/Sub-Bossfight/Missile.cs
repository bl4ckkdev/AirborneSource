// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed;
    public float turningTime;
    public Transform explosionParticles;
    public AxisConstraint turningConstraint;

    public float acceleration = 1;
    public float accuracy = 1;
    private void Start()
    {
        StartCoroutine(Countdown());
        transform.LookAt(MainEditorComponent.Instance.player.transform.position);
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(9);
        Explode();
    }
    
    public void Explode()
    {
        transform.DOKill();
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        transform.DOLookAt(MainEditorComponent.Instance.player.transform.position, turningTime, turningConstraint);
    }
    private void Update()
    {
        
        if (Bossfight.Instance.attackPattern != Bossfight.Attackpatterns.Shoot) Explode();
        speed += acceleration * Time.deltaTime;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        turningTime -= accuracy * Time.deltaTime;
    }

    private void LateUpdate()
    {
        transform.GetChild(0).position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Boss")) other.gameObject.GetComponent<Bossfight>().TakeDamage(5);
        if (other.gameObject.CompareTag("Player")) other.gameObject.GetComponent<Die>().PDie();
        Explode();
    }
}