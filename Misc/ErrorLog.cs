// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorLog : MonoBehaviour
{
    public void SendToMainScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(1);
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/SVPYwn3hSZ");
    }

    public void CopyLog()
    {

    }
}
