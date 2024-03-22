// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

public class ActualSpeedrunTimer : MonoBehaviour
{
    public string Format = @"hh\:mm\:ss\.fff";
    public TMP_Text timerText;
    public float startTime = 0f;
    public string SceneToAppear = "ActualEditor";
    public GameObject speedrunCanvas;
    
    public static ActualSpeedrunTimer instance;
    public bool isRunning;

    void Start()
    {
        isRunning = true;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(speedrunCanvas);
        }
        else Destroy(gameObject);
        if (!isRunning) StartTimer();
        SceneManager.sceneLoaded += CheckScene;
    }
    public float time;

    public void CheckScene(Scene s, LoadSceneMode l)
    {
        if ((SceneManager.GetActiveScene().name == SceneToAppear && MainEditorComponent.isOfficial) || SceneManager.GetActiveScene().name == "Level 50")
        {
            isRunning = true;
        }
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("Speedrun", 0) == 0)
        {
            speedrunCanvas.SetActive(false);
            return;
        }
        if (((SceneManager.GetActiveScene().name == SceneToAppear && MainEditorComponent.isOfficial) || SceneManager.GetActiveScene().name == "Level 50") && isRunning)
        {
            speedrunCanvas.SetActive(true);
            if (isRunning)
            {
                time += Time.deltaTime;
                TimeSpan t = TimeSpan.FromSeconds(time);
                timerText.text = t.ToString(Format);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                ResetTimer();
                StartTimer();
            }
        }
        else if ((SceneManager.GetActiveScene().name != SceneToAppear && SceneManager.GetActiveScene().name != "Level 50") || !MainEditorComponent.isOfficial)
        {
            ResetTimer();
        }
        if ((SceneManager.GetActiveScene().name != SceneToAppear && SceneManager.GetActiveScene().name != "Level 50") || !MainEditorComponent.isOfficial) speedrunCanvas.SetActive(false);
    }

    public void StartTimer()
    {
        isRunning = true;
        time = 0;
    }
    public void StopTimer() => isRunning = false;
    public void ResetTimer()
    {
        time = 0;
        isRunning = false;
        timerText.text = "00:00:00.000";
    }
}