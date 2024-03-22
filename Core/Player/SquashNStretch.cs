// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;

public class SquashNStretch : MonoBehaviour
{
    public Rigidbody rigid;
    public float MinimizeMultiplierX = 0.005f;
    public float MinimizeMultiplierY = 0.005f;
    public float clamp;
    public float SmoothTime = 0.1f;
    Vector2 vel;
    Vector2 StartScale;

    void Start()
    {
        StartScale = transform.GetChild(1).localScale;
    }

    void Update()
    {
        Vector2 scale = new Vector2(StartScale.x + (rigid.velocity.y * MinimizeMultiplierX), StartScale.y + (rigid.velocity.y * -MinimizeMultiplierY));
        transform.GetChild(1).localScale = Vector2.SmoothDamp(transform.GetChild(1).localScale, new Vector2(Mathf.Clamp(scale.y, 0, clamp), Mathf.Clamp(scale.x, 0, clamp)), ref vel, SmoothTime);
        transform.GetChild(1).transform.localScale += Vector3.forward;
    }
}
