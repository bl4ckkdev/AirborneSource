// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class LoadCampaign : MonoBehaviour
{
    public GameObject grid;
    public GameObject objButton;

    public GameObject editUI;
    public EditCampaign ec;

    public static LoadCampaign instance;

    private void Awake()
    {
        instance = this;
    }

    public void Sort(string search = "")
    {
        DestroyAllChildren(grid);
        string levelPath = Path.Combine(Application.persistentDataPath, "Packs");
        DirectoryInfo directoryInfo = new DirectoryInfo(levelPath);

        FileInfo[] levelFiles = directoryInfo.GetFiles("*.pack");
        Array.Sort(levelFiles, (a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));

        foreach (FileInfo info in levelFiles)
        {
            string fileName = info.Name;

            if (Path.GetFileNameWithoutExtension(fileName).ToLower().Contains(search.ToLower()))
            {
                GameObject gridbtn = Instantiate(objButton, grid.transform).gameObject;
                CampaignButton lb = gridbtn.GetComponent<CampaignButton>();
                lb.zipPath = info.FullName;
                lb.files = ReadZipContents(info.FullName);
                lb.campaignName = fileName;
                print(info.FullName);
                print(ReadZipContents(info.FullName).Length);
                int a = ZipFileCount(info.FullName) - 1;
                lb.levelCount.text = $"<font=bold>{a}<font=light> levels";
                lb.cName.text = fileName[..^5]; //Path.GetFileNameWithoutExtension();

                if (a < 1) lb.GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
        }
        
        string[] ReadZipContents(string zipFilePath)
        {
        
        
            if (!File.Exists(zipFilePath))
                return null;
            string tempDirectory = Path.Combine(Application.persistentDataPath, "TempZipExtract");
            ZipFile.ExtractToDirectory(zipFilePath, tempDirectory);
            return ListDirectoryContents(tempDirectory);
        }

        string[] ListDirectoryContents(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] files = di.GetFiles("*.level", System.IO.SearchOption.TopDirectoryOnly);
            

            string[] str = new string[files.Length];

            int i = 0;
            foreach (FileInfo f in files)
            {
                str[i] = File.ReadAllText(f.FullName);
                i++;
            }
            Directory.Delete(directory, true);
            return str;
        }
    }

    public void Load(string zipFilePath)
    {
        gameObject.SetActive(false);
        editUI.SetActive(true);
        
        ec.OpenCampaign(zipFilePath);
    }

    public void DestroyAllChildren(GameObject g)
    {
        foreach (Transform child in g.transform)
            Destroy(child.gameObject);
    }

    public static int ZipFileCount(string zipFileName)
    {
        using (ZipArchive archive = ZipFile.Open(zipFileName, ZipArchiveMode.Read))
        {
            int count = 0;

            foreach (var entry in archive.Entries)
                if (!string.IsNullOrEmpty(entry.Name))
                    count += 1;

            return count;
        }
    }
}
 