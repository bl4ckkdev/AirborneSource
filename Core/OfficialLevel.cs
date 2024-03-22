// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class OfficialLevel : MonoBehaviour
{
    public List<string> nextLevels;
    public int count;

    public bool final;
    
    public void LoadLevel()
    {
        if (final)
        {
            MainEditorComponent.isOfficial = true;
            MainMenu.Singleton.LoadSceneFancy("Level 50");
            return;
        }
        Debug.Log(nextLevels.Count);
        
        nextLevels.RemoveRange(0, count);
        MainEditorComponent.editable = false;
        MainEditorComponent.isWorkshop = false;
        MainEditorComponent.campaign = false;
        MainEditorComponent.LevelContents = File.ReadAllText(Path.Combine(Application.dataPath, "Assets", "Levels") + @$"\Level {name}.level");
        MainEditorComponent.LevelFile = name;
        MainEditorComponent.levelName = $"Level {name}";
        MainEditorComponent.isOfficial = true;
        MainEditorComponent.NextLevels = nextLevels;
        MainMenu.Singleton.LoadSceneFancy("ActualEditor");
    }
}
