// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CI.QuickSave;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelSettings : MonoBehaviour
{
    public TMP_InputField lname, width, height, lpHeight, rpHeight;
    public Toggle lasers, openPortal, cinemachine;

    public TMP_Dropdown music;

    public MainEditorComponent mec;
    public string[] musicName;
    public void Init()
    {
        List<string> musicList = new List<string>();
        for (int i = 0; i < musicName.Length; i++)
        {
            musicList.Add(musicName[i]);
        }

        music.ClearOptions();
        music.AddOptions(musicList);

        music.value = Array.IndexOf(mec.track, mec.source.clip);
        lname.text = MainEditorComponent.levelName;
        width.text = mec.levelWidth * 2 + string.Empty;
        height.text = mec.levelHeight * 2 + string.Empty;
        lpHeight.text = mec.portalin.transform.position.y.ToString();
        rpHeight.text = mec.portalout.transform.position.y.ToString();
        lasers.isOn = mec.lazers.activeSelf;
    }
    public string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
    public void Save()
    {
        LevelProperties lp = new LevelProperties();
        lp.Music = music.value;
        lp.Name = ReplaceInvalidChars(lname.text);
        lp.Width = width.text.Replace(',', '.');
        lp.Height = height.text.Replace(',', '.');
        lp.LHeight = lpHeight.text.Replace(',', '.');
        lp.RHeight = rpHeight.text.Replace(',', '.');
        lp.Lasers = lasers.isOn;
        
        mec.Save(lp, true);

        //mec.popups.InstantiatePopup("Saved!");

        //if (mec.editorControls.pstate != EditorControls.PlayState.stop) return;
        //    var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        //    string finalOutput = "gameprototype12\n" +
        //        Application.version + "\n" +
        //        "true\n" +
        //        music.value + "\n" +
        //        ReplaceInvalidChars(lname.text) + "\n" +
        //        timeSpan.TotalSeconds.ToString().Replace(',', '.') + "\n" +
        //        "0\n" +
        //        width.text.Replace(',', '.') + "\n" +
        //        height.text.Replace(',', '.') + "\n" +
        //        lpHeight.text.Replace(',', '.') + "\n" +
        //        rpHeight.text.Replace(',', '.') + "\n" +
        //        lasers.isOn + "\n" +
        //        "\n" +
        //        "\n" +
        //        "\n";
        //
        //for (int i = 1; i < mec.levelObjects.Length; i++)
        //{
        //    finalOutput += (int)mec.levelObjects[i].GetComponent<ObjectComponent>().obj + "\n" +
        //    mec.levelObjects[i].transform.position.x.ToString().Replace(',', '.') + "\n" +
        //    mec.levelObjects[i].transform.position.y.ToString().Replace(',', '.') + "\n" +
        //    "0\n" +
        //    mec.levelObjects[i].transform.localEulerAngles.z.ToString().Replace(',', '.') + "\n" +
        //    mec.levelObjects[i].transform.localScale.x.ToString().Replace(',', '.') + "\n" +
        //    mec.levelObjects[i].transform.localScale.y.ToString().Replace(',', '.') + "\n" +
        //    "true\n" +
        //    mec.levelObjects[i].GetComponent<Moving>().isEnabled + "," + mec.levelObjects[i].GetComponent<Moving>().restartWhenDeath + "," + mec.levelObjects[i].GetComponent<Moving>().from.y.ToString().Replace(',', '.') + "," + mec.levelObjects[i].GetComponent<Moving>().to.x.ToString().Replace(',', '.') + "," + mec.levelObjects[i].GetComponent<Moving>().to.y.ToString().Replace(',', '.') + "," + mec.levelObjects[i].GetComponent<Moving>().wait.ToString().Replace(',', '.') + "," + mec.levelObjects[i].GetComponent<Moving>().speed.ToString().Replace(',', '.') + "," + mec.levelObjects[i].GetComponent<Spin>().isEnabled + "," + mec.levelObjects[i].GetComponent<Spin>().amount.ToString().Replace(',', '.') + "|";
        //
        //    switch ((int)mec.levelObjects[i].GetComponent<ObjectComponent>().obj)
        //    {
        //        case 3:
        //            Button b = mec.levelObjects[i].GetComponentInChildren<Button>();
        //            finalOutput += b.offOnRestart + "," + b.turnBackOff + "," + b.immediatelyActivate + "\n";
        //            break;
        //        case 4:
        //            Turret t = mec.levelObjects[i].GetComponent<Turret>();
        //            finalOutput += t.targetPlayer.ToString() + "," + t.cooldown.ToString().Replace(',', '.') + "," + t.bulletSpeed.ToString().Replace(',', '.') + "," + t.offset.ToString().Replace(',', '.') + "\n";
        //            break;
        //        case 5:
        //            Launchpad p = mec.levelObjects[i].GetComponent<Launchpad>();
        //            finalOutput += p.x.ToString().Replace(',', '.') + "," + p.y.ToString().Replace(',', '.') + "\n";
        //            break;
        //        case 7:
        //            TextComponent tc = mec.levelObjects[i].GetComponent<TextComponent>();
        //            finalOutput += tc.contents.ToString().Replace('\n', '\t') + "," + tc.fontSize.ToString().Replace(',', '.') + "\n";
        //            break;
        //        case 8:
        //            var tp = mec.levelObjects[i].GetComponent<TeleporterComponentScript>();
        //            finalOutput += $"{tp.WorkAsReciever},{tp.RecieverID},{tp.RecieverTPBack},{tp.KeepYVelocity}\n";
        //            break;
        //        default:
        //            finalOutput += "\n";
        //            break;
        //    }
        //}
        //mec.popups.InstantiatePopup("Saved!");
        //QuickSaveRaw.SaveString(lname.text + ".level", finalOutput);
        //
        //if (!(lname.text + ".level").Equals(MainEditorComponent.levelName, StringComparison.InvariantCulture))
        //{
        //    QuickSaveRaw.Delete(MainEditorComponent.levelName);
        //}
        //MainEditorComponent.levelName = lname.text + ".level";
        //MainEditorComponent.isOfficial = false;
        //MainEditorComponent.LevelFile = finalOutput;
        //MainEditorComponent.LevelContents = finalOutput;
        //SceneManager.LoadScene("ActualEditor");
    }
}
