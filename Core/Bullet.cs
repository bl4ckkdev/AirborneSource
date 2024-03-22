// Copyright © bl4ck & XDev, 2022-2024
using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Transform breakParticle;
    public GameObject player;
    public bool finale;
    
    public Vector3 startVel;
    public AudioSource crashSFX;
    
    
    public bool isEditor; // this does nothing
    
    private void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed * 100);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (finale && collision.gameObject.CompareTag("Boss"))
            {
                BigTurret.Instance.hit++;
                if (BigTurret.Instance.hit >= 5)
                {
                    Bossfight.Instance.StopAttacks();
                    Bossfight.Instance.DoSomething();
                }
                Bossfight.Instance.TakeDamage(2);
            
                
            }
        }

        catch { print("kill me"); }

        Instantiate(breakParticle, transform.position, Quaternion.identity);
        crashSFX.Play();
        Destroy(gameObject);
    }

    public void Pause()
    {
        startVel = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void Resume()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed * 100);
    }
}
