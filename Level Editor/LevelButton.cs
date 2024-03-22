// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class LevelButton : MonoBehaviour
{
    public string levelName, contents;
    public TMP_Text cName;
    
    public void LoadLevel()
    {
        MainEditorComponent.isOfficial = false;
        MainEditorComponent.editable = true;
        MainEditorComponent.isWorkshop = false;
        MainEditorComponent.levelName = levelName;
        MainEditorComponent.LevelFile = null;
        MainEditorComponent.LevelContents = null;
        MainEditorComponent.campaign = false;
        MainEditorComponent.levelName = levelName;
        MainEditorComponent.NextLevels = null;
        SceneManager.LoadScene("ActualEditor");
    }
    public void Delete()
    {
        ModalManager.instance.ShowModal("Delete Level?", "This action cannot be reverted.");

        ModalManager.instance.PressedYes = () =>
        {
            File.Delete(Path.Combine(Application.persistentDataPath, "Levels") + @$"\{levelName}");
            Destroy(gameObject);
        }; 
        
        
    }
}
