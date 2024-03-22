// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    private Image img;

    public Color from, to;
    public float time;
    
    private void Start()
    {
        img = GetComponent<Image>();
        FlashImage();
    }
    public void FlashImage()
    {
        DOTween.To(() => img.color, x => img.color = x, to, time).SetEase(Ease.Linear).OnComplete(
            () => DOTween.To(() => img.color, x => img.color = x, from, time).SetEase(Ease.Linear).OnComplete(FlashImage));
    }
}
