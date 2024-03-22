// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectCampaignLevel : MonoBehaviour
{
    public static SelectCampaignLevel Instance;
    public Transform levels, grid;

    public List<CampaignSelectLevel> active;
    public string directory;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.SetActive(false);
        }
        
    }

    public void DestroyAllChildren(GameObject g)
    {
        foreach (Transform child in g.transform)
            Destroy(child.gameObject);
    }
    
    public void ListLevels()
    {
        DestroyAllChildren(grid.gameObject);
        if (Directory.Exists(Path.Combine(Application.persistentDataPath, "Levels")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        }
        string levelPath = Path.Combine(Application.persistentDataPath, "Levels");
        DirectoryInfo directoryInfo = new DirectoryInfo(levelPath);

        FileInfo[] levelFiles = directoryInfo.GetFiles("*.level");
        Array.Sort(levelFiles, (a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));

        List<string> files = EditCampaign.instance.campaignLevels.Select(a => a.GetComponent<CampaignLevel>().levelName.text).ToList();
        
        foreach (FileInfo info in levelFiles)
        {

            string fileName = info.Name;

            if (files.Contains(Path.GetFileNameWithoutExtension(fileName)))
            {
                //cl.Toggle(true);
                //cl.GetComponent<Toggle>().interactable = false;
                continue;
            }

            GameObject gridbtn = Instantiate(levels, grid.transform).gameObject;
            
            CampaignSelectLevel cl = gridbtn.GetComponent<CampaignSelectLevel>();
            cl.levelName = fileName;
            cl.Awake();
            cl.text.text = Path.GetFileNameWithoutExtension(fileName);
        }
    }

    public void AddToList()
    {
        foreach (CampaignSelectLevel campaignSelectLevel in active)
        {
            EditCampaign.instance.AddCampaignLevel(Path.GetFileNameWithoutExtension(campaignSelectLevel.GetComponent<CampaignSelectLevel>().levelName));
        }
        active.Clear();
    }

    public void Filter(string search)
    {
        foreach (Transform g in grid)
        {
            g.gameObject.SetActive(g.GetComponent<CampaignSelectLevel>().levelName.ToLower().Contains(search.ToLower()));
        }
    }

    public List<string> ReadZipContents(string zipFilePath)
    {
        try
        {
            if (!File.Exists(zipFilePath))
                return null;
            string tempDirectory = Path.Combine(Application.persistentDataPath, "TempZipExtract");
            ZipFile.ExtractToDirectory(zipFilePath, tempDirectory);
            return ListDirectoryContents(tempDirectory);
        }
        catch
        {
            Debug.LogError("whoops");
            return null;
        }
    }

    private List<string> ListDirectoryContents(string directory)
    {
        string[] files = Directory.GetFiles(directory, "*.level");
        Directory.Delete(directory, true);
        return files.ToList();
    }
}
