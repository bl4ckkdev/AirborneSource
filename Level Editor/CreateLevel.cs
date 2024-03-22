// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CI.QuickSave;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.IO;

public class CreateLevel : MonoBehaviour
{
   
    public TMP_InputField levelName;
    public TMP_InputField width;
    public TMP_InputField height, lpheight, rpheight;
    public Toggle lazers;
    public TMP_Dropdown musictrack;
    public string[] musicName;

    public Sprite music, stop;
    public Image musicImg;

    public AudioSource trackPreview;

    public AudioClip[] tracks;

    public string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
	public void Start()
	{
        List<string> musicList = new List<string>();
        for (int i = 0; i < musicName.Length; i++)
        {
            musicList.Add(musicName[i]);
        }
        //Add to dropdown because its cool
        musictrack.ClearOptions();
		musictrack.AddOptions(musicList);
	}
	public void Create()
    {
        
        if (QuickSaveRaw.Exists(levelName.text + ".level"))
        {
            
            for (int i = 1; i < Directory.GetFiles(Path.Combine(Application.persistentDataPath, "Levels"), "*", SearchOption.TopDirectoryOnly).Length + 1; i++)
            {
                if (!QuickSaveRaw.Exists(levelName.text + " (" + i + ")" + ".level"))
                {
                    levelName.text = levelName.text + " (" + i + ")";
                    break;
                }
            }
        }
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        string outputString;
        outputString =
            "gameprototype12\n" +
            Application.version + "\n" +
            "true\n" +
            musictrack.value + "\n" +
            ReplaceInvalidChars(levelName.text) + "\n" +
            timeSpan.TotalSeconds.ToString() + "\n" +
            "0\n" +
            width.text + "\n" +
            height.text + "\n" +
            Mathf.Clamp(float.Parse(lpheight.text, NumberStyles.Any, CultureInfo.InvariantCulture), -float.Parse(height.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2, float.Parse(height.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2) + "\n" +
            Mathf.Clamp(float.Parse(rpheight.text, NumberStyles.Any, CultureInfo.InvariantCulture), -float.Parse(height.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2, float.Parse(height.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2) + "\n" +
            lazers.isOn.ToString() + "\n" +
            "\n" +
            "\n" +
            "\n" +
            "1\n" +
            (-float.Parse(width.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2 + 1.5f).ToString().Replace(',', '.') + "\n" +
            (float.Parse(lpheight.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2 - 3).ToString().Replace(',', '.')  + "\n" +
            "0\n" +
            "0\n" +
            "4\n" +
            "1\n" +
            "true\n" +
            "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,0,false,false,false|false,255,255,255,255,false,255,255,255,255\n" +
            "1\n" +
            (float.Parse(width.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2 - 1.5f).ToString().Replace(',', '.')  + "\n" +
            (float.Parse(rpheight.text, NumberStyles.Any, CultureInfo.InvariantCulture) / 2  - 3).ToString().Replace(',', '.')  + "\n" +
            "0\n" +
            "0\n" +
            "4\n" +
            "1\n" +
            "true\n" +
            "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,0,false,false,false|false,255,255,255,255,false,255,255,255,255\n";


        QuickSaveRaw.SaveString(levelName.text + ".level", outputString);
        MainEditorComponent.editable = true;
        MainEditorComponent.isWorkshop = false;
        MainEditorComponent.LevelFile = outputString;
        MainEditorComponent.LevelContents = outputString;
        MainEditorComponent.levelName = levelName.text + ".level";
        MainEditorComponent.isOfficial = false;
        SceneManager.LoadScene("ActualEditor");
    }

    public void PreviewTrack(bool pr)
    {
        if (pr)
        {
            trackPreview.clip = tracks[musictrack.value];
            Music.instance.Mute(true);
            trackPreview.Play();
            musicImg.sprite = stop;
        }

        else StopPreview();
    }

    public void StopPreview()
    {
        musicImg.sprite = music;
        Music.instance.Mute(false);
        trackPreview.Stop();
    }
    
    public void ShowExplorer()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Levels"))) Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        Application.OpenURL($"file://{Path.Combine(Application.persistentDataPath, "Levels").Replace(@"/", @"\")}");
        //StartExternalProcess.Start(Path.Combine(Application.persistentDataPath, "Levels").Replace(@"/", @"\"), Path.Combine(Application.persistentDataPath, "Levels").Replace(@"/", @"\"));
        // Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        ////string itemPath = Path.Combine(Application.persistentDataPath, @"Levels\").Replace(@"/", @"\");
        //
        //System.Diagnostics.Process.Start(new ProcessStartInfo()
        //{
        //    FileName = itemPath,
        //    UseShellExecute = true,
        //    Verb = "open"
        //}); // good luck running on mac lmao
    }

}
