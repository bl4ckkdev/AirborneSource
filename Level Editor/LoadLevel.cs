// Copyright © bl4ck & XDev, 2022-2024
using System.IO;
using CI.QuickSave;
// i fucking hate everything

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadLevel : MonoBehaviour
{
    public bool isEditor;
    public GameObject grid;
    public Transform objButton;
    public void SearchThroughInput(TMP_InputField input) { SortLevels(input.text); }
    
    
    
    //public void SortLevels(string search = "")
    //{
    //    DestroyAllChildren(grid);
    //    string LevelPath = Path.Combine(Application.persistentDataPath, "Levels");
    //    string[] Levels = Directory.GetFiles(LevelPath);
    //    string[] AllLevelContents = new string[Levels.Length];
    //    float[] TimeSpan = new float[Levels.Length];
    //    float[] dummy = new float[Levels.Length] ;
    //    string[] levelNames = new string[Levels.Length];
    //    int i = 0;
    //    int j = 0;
    //    print("b");
    //    foreach (string file in Levels)
    //    {
    //        //print("a");
    //        var sections = file.Split('\\');
    //        var fileName = sections[sections.Length - 1];
    //        levelNames[i] = fileName;
    //        AllLevelContents[i] = QuickSaveRaw.LoadString(fileName);
    //        try
    //        {
    //            TimeSpan[i] = float.Parse(ReadLine(AllLevelContents[i], 6).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
    //        }
    //        catch
    //        {
    //            Debug.Log(ReadLine(AllLevelContents[i], 6).Replace(',', '.'));
    //        }
    //            
    //        dummy[i] = TimeSpan[i];
	//		i++;
	//	}
    //    Array.Sort(TimeSpan, AllLevelContents);
    //    Array.Sort(dummy, levelNames);
    //    Array.Reverse(AllLevelContents);
    //    Array.Reverse(Levels);
    //    Array.Reverse(levelNames);
    //    Array.Reverse(TimeSpan);
    //    foreach (long a in TimeSpan)
    //    {
    //        string filename = AllLevelContents[j];
	//		if (ReadLine(AllLevelContents[j], 1) == "gameprototype12" && ReadLine(AllLevelContents[j], 5).Length <= 40 )
	//		{
    //            //print("c");
	//			if (isEditor && ReadLine(AllLevelContents[j], 3) == "false") return;
    //            var sections = Levels[j].Split('\\');
    //            var fileName = sections[sections.Length - 1];
//
    //            if (levelNames[j][..^6].ToLower().Contains(search.ToLower()))
    //            {
    //                Debug.Log(levelNames[j][..^6].ToLower());
    //                GameObject gridbtn = Instantiate(objButton, grid.transform).gameObject;
    //                gridbtn.name = filename;
    //                LevelButton lb = gridbtn.GetComponent<LevelButton>();
    //                lb.levelName = levelNames[j];
    //                lb.cName.text = lb.levelName[..^6];
    //            }
	//		}
    //        j++;
	//	}
    //}
    public void SortLevels(string search)
    {
        Sort(search);
    }
    
    public void Sort(string search = "")
    {
        DestroyAllChildren(grid);
        string levelPath = Path.Combine(Application.persistentDataPath, "Levels");
        DirectoryInfo directoryInfo = new DirectoryInfo(levelPath);

        FileInfo[] levelFiles = directoryInfo.GetFiles("*.level");
        Array.Sort(levelFiles, (a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime)); // Sort by last edit time

        //List<Task<(string, string)>> loadTasks = new List<Task<(string, string)>>();

        //foreach (FileInfo file in levelFiles)
        //{
        //    loadTasks.Add(LoadAndProcessFileAsync(file));
        //}
//
        //await Task.WhenAll(loadTasks);

        foreach (FileInfo info in levelFiles) //(var result in loadTasks.Select(t => t.Result))
        {
            string fileName = info.Name;

            //if (ReadLine(levelContent, 1) == "gameprototype12" && ReadLine(levelContent, 5).Length <= 40)
            //{
                //if (isEditor && ReadLine(levelContent, 3) == "false") continue;

                if (fileName.ToLower().Contains(search.ToLower()))
                {
                    GameObject gridbtn = Instantiate(objButton, grid.transform).gameObject;
                    LevelButton lb = gridbtn.GetComponent<LevelButton>();
                    lb.levelName = fileName;
                    lb.cName.text = fileName[..^6];
                }
            //}
        }
    }
    //public void SortLevels(string search = "")
    //{
    //    DestroyAllChildren(grid);
    //    string levelPath = Path.Combine(Application.persistentDataPath, "Levels");
    //    DirectoryInfo directoryInfo = new DirectoryInfo(levelPath);
//
    //    FileInfo[] levelFiles = directoryInfo.GetFiles();
    //    Array.Sort(levelFiles, (a, b) => CompareFilesByFifthLine(b, a));
//
    //    foreach (FileInfo file in levelFiles)
    //    {
    //        string fileName = file.Name;
    //        string levelContent = QuickSaveRaw.LoadString(fileName);
    //
    //        if (ReadLine(levelContent, 1) == "gameprototype12" && ReadLine(levelContent, 5).Length <= 40)
    //        {
    //            if (isEditor && ReadLine(levelContent, 3) == "false") continue;
    //    
    //            string levelName = fileName[..^6].ToLower();
    //    
    //            if (levelName.ToLower().Contains(search.ToLower()))
    //            {
    //                GameObject gridbtn = Instantiate(objButton, grid.transform).gameObject;
    //                
    //                LevelButton lb = gridbtn.GetComponent<LevelButton>();
    //                lb.contents = levelContent;
    //                lb.levelName = fileName;
    //                lb.cName.text = levelName;
    //            }
    //        }
    //    }
    //}

    public void DestroyAllChildren(GameObject g)
    {
        foreach (Transform child in g.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void EnterScene(GameObject t)
    {
        MainEditorComponent.editable = true;
        MainEditorComponent.isWorkshop = false;
        MainEditorComponent.LevelFile = t.GetComponent<LevelButton>().levelName;
        
        SceneManager.LoadScene("ActualEditor");
        MainEditorComponent.LevelFile = t.name;
        MainEditorComponent.isOfficial = false;
        //SceneManager.LoadScene("ActualEditor");
    }

    private int CountLines(string content)
    {
        int count = 1;

        // Iterating the string from left to right
        for (int i = 0; i < content.Length; i++)
        {

            // Checking if the character encountered is
            // a newline character if yes then increment
            // the value of count variable
            if (content[i] == '\n')
                count++;
        }

        return count - 1;
    }

    private string ReadLine(string text, int lineNumber)
    {
        var reader = new StringReader(text);

        string line;
        int currentLineNumber = 0;

        do
        {
            currentLineNumber += 1;
            line = reader.ReadLine();
        }
        while (line != null && currentLineNumber < lineNumber);

        return (currentLineNumber == lineNumber) ? line :
                                                   string.Empty;
    }


}
