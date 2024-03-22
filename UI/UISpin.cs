// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;

public class UISpin : MonoBehaviour
{
    public float amount;

    private void Update()
    {
        transform.localEulerAngles += new Vector3(0, 0, amount * Time.deltaTime);
    }
}