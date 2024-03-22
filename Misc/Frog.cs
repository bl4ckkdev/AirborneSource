// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Frog : MonoBehaviour
{

    public Vector2 dir = new Vector2(-7.4f, -8.6f);
    private int count = 0;
    
    public void Pop()
    {
        if (count == 1) MainMenu.Singleton.LoadSecretLevel();
        count++;
            
        transform.DOMoveY(dir.x, 0.5f);
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        transform.DOMoveY(dir.y, 0.5f);
    }
}
