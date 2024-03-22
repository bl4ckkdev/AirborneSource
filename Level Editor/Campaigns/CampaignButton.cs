// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignButton : MonoBehaviour
{
    public string campaignName, contents, zipPath;
    public string[] files;
    public TMP_Text cName, levelCount;

    public void Delete()
    {
        ModalManager.instance.ShowModal("Delete Pack?", "This action cannot be reverted.");

        ModalManager.instance.PressedYes = () =>
        {
            File.Delete(Path.Combine(Application.persistentDataPath, "Packs") + @$"\{campaignName}");
            Destroy(gameObject);
        };
    }

    public void Edit()
    {
        LoadCampaign.instance.Load(Path.Combine(Application.persistentDataPath, "Packs", $"{cName.text}.pack"));
    }

    public void Play()
    {
        MainEditorComponent.NextLevels = files.ToList();
        MainEditorComponent.campaign = true;
        MainEditorComponent.isOfficial = false;
        MainEditorComponent.editable = false;
        MainEditorComponent.isWorkshop = false;

        SceneManager.LoadSceneAsync("ActualEditor");
    }
}
