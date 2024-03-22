// Copyright © bl4ck & XDev, 2022-2024
using EZCameraShake;
using UnityEngine;

public class DestroyEverything : MonoBehaviour
{
    public Vector3 force;
    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.GetComponent<Collider>().enabled = false;
        other.gameObject.AddComponent<Rigidbody>();
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (other.gameObject.transform.position.y > transform.position.y) rb.AddForce(new Vector3(force.x, force.y, 0), ForceMode.Impulse);
        else rb.AddForce(new Vector3(force.x, -force.y, 0), ForceMode.Impulse);
        CameraShaker.Instance.ShakeOnce(1, 1, 0.1f, 0.1f);
    }
}
