// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;

public class Bop : MonoBehaviour
{
    public float speed, strength = 1f;
    
    private void Update()
    {
        transform.localPosition = new Vector3(0, Mathf.Sin(Time.time * speed) * strength, 0);
    }
}
