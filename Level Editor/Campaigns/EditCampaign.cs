// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using System;
using TMPro;
using UnityEngine.Rendering;

public class EditCampaign : MonoBehaviour
{
    public bool debug = false;

    public string selectedZipPath;
    public string openedCampaignFolder;

    public List<GameObject> campaignLevels;
    public List<string> deletedLevels;

    public string campaignPropertiesFile;

    public GameObject contents;
    public GameObject campaignGameObject;
    public GameObject buttonObject;
    public GameObject uploadCanvas;
    public Transform plus;

    public ModalManager modalManager;

    public static EditCampaign instance;

    public string campaignName;
    public string unverifiedLevels = "";
    public string campaignPath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
    }

    public bool verified;

    public void OpenCampaign(string zipPath)
    {
        campaignLevels.Clear();
        void DestroyAllChildren(Transform t)
        {
            foreach (Transform tr in t) if (tr.name != "plus") Destroy(tr.gameObject);
        }
        
        DestroyAllChildren(contents.transform);
        plus.SetAsLastSibling();
        campaignName = Path.GetFileNameWithoutExtension(zipPath);
        campaignPath = zipPath;
        string tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        
        Directory.CreateDirectory(tempFolderPath);
        ZipFile.ExtractToDirectory(zipPath, tempFolderPath);

        selectedZipPath = zipPath;
        openedCampaignFolder = tempFolderPath;
        //campaignLevels = ListDirectoryLevels(tempFolderPath);
        
        campaignPropertiesFile = File.ReadAllText(Path.Combine(tempFolderPath, "PACK.PROPERTIES"));
        print(campaignPropertiesFile);
        ReadPropetiesFile();
    }

    public string Combine(params string[] path)
    {
        string str = string.Empty;
        foreach (string p in path)
        {
            str += p + @"\";
        }

        return str.Remove(str.Length - 1, 1).Replace('/', '\\');
    }

    public GameObject error;
    public TMP_Text log;
    public void Throw(string trace)
    {
        error.SetActive(true);
        uploadCanvas.SetActive(false);
        log.text = trace;
    }

    public void Save(string pName = null)
    {
        if (!string.IsNullOrEmpty(pName)) campaignName = pName;
        foreach (GameObject c in campaignLevels)
        {
            string levelName = $"{c.GetComponent<CampaignLevel>().levelName.text}.level";

            
            if (bool.Parse(File.ReadAllText(Path.Combine(Application.persistentDataPath, "Levels", levelName)).Split('\n')[2]))
            {
                unverifiedLevels += levelName + "\n";
            }


        }

        //if (unverifiedLevels.Length > 1)
        //{
        //    string errorLog = $"Unverified Level(s):\n";
        //    foreach (string a in unverifiedLevels.Split('\n'))
        //    {
        //        errorLog += a + "\n";
        //    }
        //    errorLog += "Verify the level(s) first, then upload.";
        //    Throw(errorLog);
        //    verified = false;
        //    return;
        //}
        //verified = true;

        static bool FilePathHasInvalidChars(string path)
        {
            return (!string.IsNullOrEmpty(path) && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0);
        }
        print(openedCampaignFolder);
        foreach (string d in deletedLevels)
        {
            File.Delete(Path.Combine(openedCampaignFolder, d));
        }

        string newString = "gameprototype12\n" +
            Application.version + "\n\n";

        foreach (GameObject c in campaignLevels)
        {
            
            
            string dddwetgergtrt = $"{c.GetComponent<CampaignLevel>().levelName.text}.level";
            newString += dddwetgergtrt + '\n';
            
            
            if (!File.Exists(Path.Combine(openedCampaignFolder, dddwetgergtrt)))
            {
                File.Copy(Path.Combine(Application.persistentDataPath, "Levels", dddwetgergtrt),
                Path.Combine(openedCampaignFolder, dddwetgergtrt));
            }
        }
        File.WriteAllText(Path.Combine(openedCampaignFolder, "PACK.PROPERTIES"), newString);
        print("Saved...");

        //string zipFileDestinationPath = Path.Combine(Application.persistentDataPath, "Campaigns", $"{}.campaign");
        if (File.Exists(selectedZipPath))
        {
            File.Delete(selectedZipPath);
        }
        
        selectedZipPath = Path.Combine(Application.persistentDataPath, "Packs", $"{campaignName}.pack");
        campaignPath = selectedZipPath;
        ZipFile.CreateFromDirectory(openedCampaignFolder, selectedZipPath);
    }

    public void RestartScene()
    {
        if (!error.activeSelf) SceneManager.LoadScene("Editor");
    }

    public void RemoveLevel(GameObject gobject)
    {
        campaignLevels.Remove(gobject);
        Destroy(gobject);

        UpdateList();
        buttonObject.transform.SetAsLastSibling();
        deletedLevels.Add($"{gobject.GetComponent<CampaignLevel>().levelName.text}.level");
    }

    public void ReadPropetiesFile()
    {
        if (MainEditorComponent.ReadLine(campaignPropertiesFile, 1) != "gameprototype12")
        {
            print("no gameprototype12");

            modalManager.ShowModal("Invalid File", "This pack is not a valid Airborne file.");
            //Time.timeScale = 0;

            modalManager.PressedYes = () =>
            {
                //Time.timeScale = 1;
                SceneManager.LoadScene(1);
                return;
            };

        }

        else if (int.Parse(MainEditorComponent.ReadLine(campaignPropertiesFile, 2).Replace(".", String.Empty)) > int.Parse(Application.version.Replace(".", String.Empty)))
        {
            print("wrong version");

            modalManager.ShowModal("Unsupported File", "Please update your game in order to play this pack.");
            //Time.timeScale = 0;

            modalManager.PressedYes = () =>
            {
                //Time.timeScale = 1;
                SceneManager.LoadScene(1);
                return;
            };
        }

        for (int i = 4; i < MainEditorComponent.CountLines(campaignPropertiesFile) + 1; i++)
        {
            if (File.Exists(Path.Combine(openedCampaignFolder, MainEditorComponent.ReadLine(campaignPropertiesFile, i))))
            {
                print(Path.Combine(openedCampaignFolder, MainEditorComponent.ReadLine(campaignPropertiesFile, i)));
                AddCampaignLevel(Path.GetFileNameWithoutExtension(MainEditorComponent.ReadLine(campaignPropertiesFile, i)));
            }
            else
            {


                print($"INVALID FILE {Path.Combine(openedCampaignFolder, MainEditorComponent.ReadLine(campaignPropertiesFile, i))}");
            }
        }
    }

    public void MoveUp(GameObject gobject)
    {
        int index = campaignLevels.IndexOf(gobject);

        if (index > 0 && index < campaignLevels.Count)
        {
            GameObject temp = campaignLevels[index - 1];
            campaignLevels[index - 1] = gobject;

            campaignLevels[index] = temp;
            UpdateList();
        }
    }

    public void MoveDown(GameObject gobject)
    {
        int index = campaignLevels.IndexOf(gobject);

        if (index >= 0 && index < campaignLevels.Count - 1)
        {
            GameObject temp = campaignLevels[index + 1];
            campaignLevels[index + 1] = gobject;
            campaignLevels[index] = temp;
            UpdateList();
        }
    }

    private void UpdateList()
    {
        for (int i = 0; i < campaignLevels.Count; i++)
        {
            campaignLevels[i].transform.SetSiblingIndex(i);
            campaignLevels[i].GetComponent<CampaignLevel>().num.text = (i + 1).ToString();
        }
    }

    public void AddCampaignLevel(string levelName)
    {
        GameObject c;

        c = Instantiate(campaignGameObject, contents.transform);
        c.GetComponent<CampaignLevel>().levelName.text = levelName;
        c.GetComponent<CampaignLevel>().num.text = Convert.ToString(campaignLevels.IndexOf(c) + 1);
        campaignLevels.Add(c); 
        buttonObject.transform.SetAsLastSibling();
        UpdateList();
    }

    private List<string> ListDirectoryLevels(string directory)
    {
        string[] files = Directory.GetFiles(directory, "*.level");
        Directory.Delete(directory, true);
        return files.ToList();

    }

    private void OnApplicationQuit()
    {
        if (!debug)
            Directory.Delete(openedCampaignFolder, true);
    }
}