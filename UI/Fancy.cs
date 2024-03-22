// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fancy : MonoBehaviour
{
    public static Fancy Singleton;
    public Animator exit;

    public void Awake()
    {
        Singleton = this;
    }

    public static IEnumerator FancyScene(string scene)
    {
        Singleton.exit.gameObject.SetActive(true);
        Singleton.exit.Play("Exit");
        yield return new WaitForSeconds(0.33f);
        SceneManager.LoadSceneAsync(scene);
    }
}
