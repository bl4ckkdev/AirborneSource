// Copyright © bl4ck & XDev, 2022-2024
using System;
using UnityEngine;

public class ChangelogManager : MonoBehaviour
{
    public GameObject changelog, menu;
    private void Awake()
    {
        changelog.SetActive(PlayerPrefs.GetInt("ShownChangelog" + Application.version, 0) == 0);
        
        menu.SetActive(!changelog.activeSelf);
    }

    public void SetChangelog() => PlayerPrefs.SetInt("ShownChangelog" + Application.version, 1);
}
