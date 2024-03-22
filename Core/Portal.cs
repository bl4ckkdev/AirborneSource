// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class Portal : MonoBehaviour
{
    public bool isOpen;
    public GameObject button;
    public MainEditorComponent mainEditor;
    public EditorControls ec;
    public ParticleSystemRenderer door_particles;
    public GameObject player;
    public Material button_enabled;
    public Material button_disabled;
    public Material door_enabled;
    public Material door_disabled;
    public bool StartWithOpenPortal;
    public string Level;
    public GameObject door;
    public AudioSource audios;
    public AudioSource win;
    public Collision playerCollider;
    public bool finale;
    public GameObject completeMenu;
    

    private void Start()
    {
        isOpen = StartWithOpenPortal;
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && !finale)
        {
            if (isOpen)
            {
                isOpen = false;

                //MainEditorComponent.Instance.editorTrail.enabled = false; // Disable the trail
                SpawnToNextLevel();
                //MainEditorComponent.Instance.editorTrail.enabled = true; // Re-enable the trail after spawning
            }
        }



    }
    public void SpawnToNextLevel()
    {
        win.Play();
        
        if (MainEditorComponent.isOfficial)
        {
            
            if (MainEditorComponent.NextLevels.Count == 1)
            {
                AchievementManager.SetLevels(50);
                Fancy.Singleton.StartCoroutine(Fancy.FancyScene("Level 50"));
                return;
            }

            List<string> nextLevels = MainEditorComponent.NextLevels;

            if (AchievementManager.GetLevels() < int.Parse(nextLevels[0].Substring(6)) && PlayerPrefs.GetInt("Exploration", 0) == 0)
            {
                AchievementManager.SetLevels(int.Parse(nextLevels[0].Substring(6)));
            }
            nextLevels.RemoveAt(0);
            MainEditorComponent.editable = false;
            MainEditorComponent.isWorkshop = false;
            MainEditorComponent.LevelContents = File.ReadAllText(Path.Combine(Application.dataPath, "Assets", "Levels") + @"\" + nextLevels[0] + ".level");
            MainEditorComponent.LevelFile = nextLevels[0];
            MainEditorComponent.levelName = $"Level {nextLevels[0].Substring(6)}";
            MainEditorComponent.isOfficial = true;
            MainEditorComponent.NextLevels = nextLevels;
            Fancy.Singleton.StartCoroutine(Fancy.FancyScene("ActualEditor"));
            if (PlayerPrefs.GetInt("SpeedrunStop", 0) == int.Parse(nextLevels[0].Substring(6)))
            {
                ActualSpeedrunTimer.instance.StopTimer();
            }
        }
        else if (!MainEditorComponent.editable)
        {
            if (MainEditorComponent.campaign && MainEditorComponent.NextLevels.Count > 1)
            {
                List<string> nextLevels = MainEditorComponent.NextLevels;
                
                nextLevels.RemoveAt(0);
                MainEditorComponent.editable = false;
                MainEditorComponent.isWorkshop = false;
                //MainEditorComponent.LevelContents = nextLevels[0];
                //MainEditorComponent.LevelFile = nextLevels[0];
                //MainEditorComponent.levelName = $"nextLevel";
                MainEditorComponent.isOfficial = false;
                MainEditorComponent.NextLevels = nextLevels;
                
                Fancy.Singleton.StartCoroutine(Fancy.FancyScene("ActualEditor"));
            }
            else
            {
                if (MainEditorComponent.campaign) MainEditorComponent.Instance.levelComplete.text = "Pack Complete";
                completeMenu.SetActive(true);
                Time.timeScale = 0;
            }

        }
        else
        {
            mainEditor.beat = true;
        }
        ec.Stop();
    }
}
