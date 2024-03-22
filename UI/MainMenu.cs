// Copyright © bl4ck & XDev, 2022-2024
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks.Data;
using System;
using Random = System.Random;
public class MainMenu : MonoBehaviour
{
    public Animator exit;
    public Animator fade;
    public static MainMenu Singleton;
    public bool isMenu;
    public void Play() { SceneManager.LoadScene("LevelSelector"); }
    public void GotoMain() { SceneManager.LoadScene("MainMenu"); }
    private void Awake()
    {
        Scroll.toggleBefore = false;
        Singleton = this;
    }
    public void Exit() 
    {
        StartCoroutine("ex");     
    }
    
    
    string scene;

    public void Start()
    {
        if (isMenu) Time.timeScale = 1;
    }
    //Loadlevel // awesome comment xdev
    public void LoadLevel(string button)
    {
        SceneManager.LoadScene(button);
    }

    public void LoadSceneFancy(string scenes)
    {
        scene = scenes;
        StartCoroutine("FancyScene");
    }
    
    IEnumerator FancyScene()
    {
        exit.gameObject.SetActive(true);
        exit.Play("Exit");
        yield return new WaitForSeconds(0.33f);
        SceneManager.LoadSceneAsync(scene);
    }


    IEnumerator ex()
    {
        exit.gameObject.SetActive(true);
        exit.Play("Exit");
        yield return new WaitForSeconds(0.33f);
        Application.Quit();
    }

    // i hate this code 
    public void FixButton(UnityEngine.UI.Button button)
    {
        button.GetComponent<Animator>().StopPlayback();
        button.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = 0;
        button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = UnityEngine.Color.white;
    }
    
    public void LoadSecretLevel()
    {   
        MainEditorComponent.editable = false;
        MainEditorComponent.isOfficial = false;
        MainEditorComponent.LevelFile = File.ReadAllText(Path.Combine(Application.dataPath, "Assets", "Levels", "c2VjcmV0IGxldmVs") + @"\Secret Level.level");
        MainEditorComponent.LevelContents = File.ReadAllText(Path.Combine(Application.dataPath, "Assets", "Levels", "c2VjcmV0IGxldmVs") + @"\Secret Level.level");
        MainEditorComponent.levelName = "Secret Level";
        MainEditorComponent.isWorkshop = true;
        Fancy.Singleton.StartCoroutine(Fancy.FancyScene("ActualEditor"));
    }
}


