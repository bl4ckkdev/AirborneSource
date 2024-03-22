// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public List<string> LevelNames;
    public List<GameObject> LevelUI;
    public bool exploration;
    public Transform Level;
    public Transform grid;

    public GameObject expText;
    
    public void Start()
    {
        LevelUI.ForEach(Destroy); // this code is so unoptimized!!
        LevelUI.Clear();
        string[] levelArray = Directory.GetFiles(Path.Combine(Application.dataPath, "Assets", "Levels"), "*.level");
        
        int[] levelNamesToInt = new int[levelArray.Length];
        for (int i = 0; i < levelArray.Length; i++)
        {
            levelArray[i] = Path.GetFileNameWithoutExtension(levelArray[i]);
        }
        for (int i = 0; i < levelArray.Length; i++) {
            levelNamesToInt[i] = int.Parse(levelArray[i].Substring(6));
        }
        Array.Sort(levelNamesToInt, levelArray);
        LevelNames = levelArray.ToList();
        
        
        

        
        for (int i = 0; i < levelArray.Length; i++)
        {
            Transform t = Instantiate(Level, grid);
            LevelUI.Add(t.gameObject);
            t.name = LevelNames[i].Substring(6);
            t.GetComponentInChildren<TMP_Text>().text = LevelNames[i].Substring(6);
            t.GetComponent<OfficialLevel>().nextLevels = LevelNames;
            t.GetComponent<OfficialLevel>().count = i;
            if (int.Parse(t.name) - 1 > AchievementManager.GetLevels() && !exploration) t.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        Transform e = Instantiate(Level, grid);
        LevelUI.Add(e.gameObject);
        e.name = "50";
        e.GetComponentInChildren<TMP_Text>().text = "50";
        e.GetComponent<OfficialLevel>().final = true;
        if (49 > AchievementManager.GetLevels() && !exploration) e.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    public void ExplorationMode(bool yes)
    {
        exploration = yes;
        SettingsMenu.SetExploration(yes);
        expText.SetActive(yes);

        foreach (var level in LevelUI)
        {
            if (!yes) Start();
            else
            level.GetComponent<UnityEngine.UI.Button>().interactable = yes;
        }
    }
    

}
