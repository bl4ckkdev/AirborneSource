// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;

public class CreateCampaign : MonoBehaviour
{
    public TMP_InputField campaignName;
    public EditCampaign ec;

    public GameObject editUI;

    public void Create()
    {
        string campPath = Path.Combine(Application.persistentDataPath, "Packs");
        if (!Directory.Exists(campPath))
            Directory.CreateDirectory(campPath);
        
        if (File.Exists(Path.Combine(campPath, campaignName.text + ".pack")))
        {
            for (int i = 1; i < Directory.GetFiles(campPath, "*.pack", SearchOption.TopDirectoryOnly).Length + 2; i++)
            {
                if (!File.Exists(Path.Combine(campPath, campaignName.text + " (" + i + ")" + ".pack")))
                {
                    campaignName.text = campaignName.text + " (" + i + ")";
                    break;
                }
            }
        }

        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        string outputString = "gameprototype12\n" +
            Application.version + "\n";

        string tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string tempFilePath = Path.Combine(tempFolderPath, "PACK.PROPERTIES");
        string zipFilePath = Path.Combine(Application.persistentDataPath, "Packs", campaignName.text + ".pack");

        Directory.CreateDirectory(tempFolderPath);
        File.WriteAllText(tempFilePath, outputString);

        ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath);

        Directory.Delete(tempFolderPath, true);
        
        gameObject.SetActive(false);
        editUI.SetActive(true);
        
        ec.OpenCampaign(zipFilePath);
    }

    public void ShowExplorer()
    {
        Application.OpenURL($"file://{Path.Combine(Application.persistentDataPath, "Packs").Replace(@"/", @"\")}");
    }
}
