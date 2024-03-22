// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scroll : MonoBehaviour
{
    public float seconds;
    public float to;

    public CanvasGroup before, after;

    public static bool toggleBefore;
    
    public void Start()
    {
        if (toggleBefore)
        {
            before.gameObject.SetActive(true);
            DOTween.To(() => before.alpha, x => before.alpha = x, 0f, 0.5f);
        }
        Music.instance.source.volume = 1;
        transform.DOLocalMoveY(to, seconds).SetEase(Ease.Linear).OnComplete(() => DOTween.To(() => after.alpha, x => after.alpha = x, 1f, 0.5f));
    }

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) SceneManager.LoadScene("MainMenu");
    }
}
