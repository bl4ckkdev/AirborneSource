// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using System.Collections;
using System.IO;
using UnityEngine;
using CI.QuickSave;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using Object = System.Object;
public class MainEditorComponent : MonoBehaviour
{
    [Header("DEBUG MODE")]
    public bool debug;



    [Header("MAIN")]
    public bool finale;
    
    public static string LevelFile;
    public static string levelName;
    public static string LevelContents;
    public static bool isWorkshop;
    public static bool editable = true;
    public static bool isOfficial = false;
    public static bool workshopEditable;
    public static bool campaign;
    
    public int autoSaveSeconds = 240; //I will maybe find a use for this
    public GameObject checkpoint;
    public bool beat;
    private string levelVersion;
    

    [HideInInspector] public static Steamworks.Ugc.Item workshopLevel;

    public string LevelPath;
    public GameObject left, right, up, down, enterPortal, leavePortal, lazers;

    public GameObject[] levelObjects = new GameObject[1];
    public GameObject[] buttons;
    public List<Checkpoint> checkpoints;
    public List<GameObject> blackHoles;
    public Checkpoint activeCheckpoint;
    public PopupManager popups;
    
    public bool playing;
    public bool paused;

    [Header("PREFABS")]
    public GameObject[] placeableObjects;

    public Material[] playerMats = new Material[2];

    public GameObject player;
    public GameObject portalout;
    public GameObject spawner;
    public GameObject portalin;
    public PauseMenu pauseMenu;
    public TMP_Text levelTitle;
    public TMP_Text levelComplete;

    public bool enableTrail;
    public bool cineEnabled;
    public int fov;

    public EditorControls editorControls;

    public float levelWidth, levelHeight;

    [Header("Music")]
    public AudioClip[] track;
    public AudioSource source;

    public TrailRenderer editorTrail;

    public static List<string> NextLevels;
    

    //[Header("Discord controller")]
    //public Discord_Controller dcontroller;


    enum EditorState
    {
        build,
        preview,
        play,
        pause
    }
    public static MainEditorComponent Instance;
    private void Awake()
    {
        Instance = this;
        if (isWorkshop || isOfficial || !editable)
            editorTrail.enabled = false;
        
        if (finale) return;
        Physics.gravity = new Vector3(0, -10f, 0);
		StartCoroutine(AutoSave());
        print($"{NextLevels == null} | {campaign}");
        if (campaign)
        {
            
            if (NextLevels.Count < 1)
            {
                Throw("Pack has no levels.");
                return;
            }
            string contents = NextLevels[0];
            print(contents);

            levelName = ReadLine(contents, 5);
            LevelContents = NextLevels[0];
            LevelFile = NextLevels[0];
        }
        
        if (levelName == null)
        {
            Throw("Level not found.");
            return;
        }
        if (!isOfficial)
            LevelPath = Path.Combine(Application.persistentDataPath, "Levels", levelName);


        
        try
        {
            if (LevelContents != null) {  }
            else
            {
                LevelContents = QuickSaveRaw.LoadString(levelName);
            }
        }
        catch
        {
            Throw("Level not found.");
        }

        LoadLevel(LevelContents);
        StartCoroutine(AutoSave());
        
    }


    private void Update()
    {
        if (finale) return;
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S) && editorControls.pstate == EditorControls.PlayState.stop && !editorControls.cantContinue) Save();
    }

    public void Copy()
    {
        string output = File.ReadAllText(workshopLevel.Directory + @"\" + workshopLevel.Title + ".level");
        QuickSaveRaw.SaveString(workshopLevel.Title + ".level", output);
        editable = true;
        isWorkshop = false;
        LevelFile = output;
        LevelContents = output;
        levelName = workshopLevel.Title + ".level";
        isOfficial = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("ActualEditor");
    }

    public void CleanSave() => Save();

    [Header("Error Log")]
    public GameObject canvas;
    public GameObject copiedCanvas;
    public TMP_Text logInfo;
    
    public void Throw(string trace)
    {
        Time.timeScale = 0;
        canvas.SetActive(true);
        logInfo.text = string.Empty;
        if (levelVersion == null) levelVersion = "Unknown";
        if (string.IsNullOrEmpty(levelName)) levelName = "Unknown"; 
        logInfo.text += $"Level Name: {levelName}\n\n";
        logInfo.text += $"Level Version: {levelVersion}\nYour Version: {Application.version}\n\n";
        logInfo.text += trace;
    }

    public void CopyLog()
    {
        GUIUtility.systemCopyBuffer = logInfo.text;
        copiedCanvas.SetActive(true);
    }

    public void OpenFile()
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Levels"))) Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
        //string itemPath = Path.Combine(Application.persistentDataPath, @"Levels\").Replace(@"/", @"\");
        string itemPath = Path.Combine(Application.persistentDataPath, "Levels").Replace(@"/", @"\");
        System.Diagnostics.Process.Start("explorer.exe", itemPath); // good luck running on mac lmao
    }
    
    public void Save(LevelProperties? lp = null, bool reload = false)
    {
        if (!editable) return;
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        string finalOutput = "gameprototype12\n" +
            Application.version + "\n" +
            $"{!MainEditorComponent.Instance.beat}\n" +
            (!lp.HasValue ? Array.IndexOf(track, source.clip) : lp.Value.Music) + "\n" +
            (!lp.HasValue ? ReplaceInvalidChars(levelName) : lp.Value.Name) + "\n" +
            timeSpan.TotalSeconds.ToString().Replace(',', '.') + "\n" +
            "0\n" +
            (!lp.HasValue ? (levelWidth * 2).ToString().Replace(',', '.') : lp.Value.Width) + "\n" +
            (!lp.HasValue ? (levelHeight * 2).ToString().Replace(',', '.') : lp.Value.Height) + "\n" +
            (!lp.HasValue ? enterPortal.transform.position.y.ToString().Replace(',', '.') : lp.Value.LHeight) + "\n" +
            (!lp.HasValue ? leavePortal.transform.position.y.ToString().Replace(',', '.') : lp.Value.RHeight) + "\n" +
            (!lp.HasValue ? lazers.activeSelf : lp.Value.Lasers) + "\n" +
            "\n" +
            "\n" +
            "\n";
        
        for (int i = 1; i < levelObjects.Length; i++)
        {
            finalOutput += (int)levelObjects[i].GetComponent<ObjectComponent>().obj + "\n" +
                levelObjects[i].transform.position.x.ToString().Replace(',', '.') + "\n" +
                levelObjects[i].transform.position.y.ToString().Replace(',', '.') + "\n" +
                "0\n" +
                levelObjects[i].transform.localEulerAngles.z.ToString().Replace(',', '.') + "\n" +
                levelObjects[i].transform.localScale.x.ToString().Replace(',', '.') + "\n" +
                levelObjects[i].transform.localScale.y.ToString().Replace(',', '.') + "\n" +
                "true\n";
            
            //levelObjects[i].GetComponent<Moving>().isEnabled + "," + levelObjects[i].GetComponent<Moving>().restartWhenDeath + "," + levelObjects[i].GetComponent<Moving>().from.y.ToString().Replace(',', '.') + "," + levelObjects[i].GetComponent<Moving>().to.x.ToString().Replace(',', '.') + "," + levelObjects[i].GetComponent<Moving>().to.y.ToString().Replace(',', '.') + "," + levelObjects[i].GetComponent<Moving>().wait.ToString().Replace(',', '.') + "," + levelObjects[i].GetComponent<Moving>().speed.ToString().Replace(',', '.') + "," + 
            Moving moving = levelObjects[i].GetComponent<Moving>();
            finalOutput += $"{moving.isEnabled},{moving.restartWhenDeath},{moving.instantlyStart},{moving.loop},{moving.goBack},{moving.useTime},";
            foreach (var point in moving.points)
            {
                finalOutput += $"{point.to.x}!{point.to.y}!{point.offset}!{point.speedortime}!{(int)point.ease}~";
            }
            finalOutput += "," + levelObjects[i].GetComponent<Spin>().isEnabled + "," + levelObjects[i].GetComponent<Spin>().amount.ToString().Replace(',', '.') + "," + levelObjects[i].GetComponent<goddamnit.Physics>().isEnabled + "," + levelObjects[i].GetComponent<goddamnit.Physics>().mass.ToString().Replace(',', '.') + "," + levelObjects[i].GetComponent<goddamnit.Physics>().lockX + "," + levelObjects[i].GetComponent<goddamnit.Physics>().lockY + "," + levelObjects[i].GetComponent<goddamnit.Physics>().lockZ + "," + levelObjects[i].GetComponent<goddamnit.Physics>().restartOnDeath + "|";
            switch ((int)levelObjects[i].GetComponent<ObjectComponent>().obj)
            {
                case 1:
                    ColorComponent cc = levelObjects[i].GetComponent<ColorComponent>();
                    finalOutput += $"{cc.enabled},{cc.r},{cc.g},{cc.b},{cc.a},{cc.g_enabled},{cc.g_r},{cc.g_g},{cc.g_b},{cc.g_a}\n";
                    break;
                case 3:
                    Button b = levelObjects[i].GetComponent<Button>();
                    finalOutput += b.offOnRestart + "," + b.turnBackOff + "," + b.immediatelyActivate + "\n";
                    break;
                case 4:
                    Turret t = levelObjects[i].GetComponent<Turret>();
                    finalOutput += t.targetPlayer.ToString() + "," + t.cooldown.ToString().Replace(',', '.') + "," + t.bulletSpeed.ToString().Replace(',', '.') + "," + t.offset.ToString().Replace(',', '.') + "\n";
                    break;
                case 5:
                    Launchpad p = levelObjects[i].GetComponent<Launchpad>();
                    finalOutput += p.x.ToString().Replace(',', '.') + "," + p.y.ToString().Replace(',', '.') + "\n";
                    break;
                case 7:
                    TextComponent tc = levelObjects[i].GetComponent<TextComponent>();
                    ColorComponent cc1 = levelObjects[i].GetComponent<ColorComponent>();
                    finalOutput += $"{tc.contents.ToString().Replace('\n', '\t')},{tc.fontSize.ToString().Replace(',', '.')},{cc1.enabled},{cc1.r},{cc1.g},{cc1.b},{cc1.a},{cc1.g_enabled},{cc1.g_r},{cc1.g_g},{cc1.g_b},{cc1.g_a},{tc.bold},{tc.italic}\n";
                    break;
                case 8:
                    var tp = levelObjects[i].GetComponent<TeleporterComponentScript>();
                    finalOutput += $"{tp.WorkAsReciever},{tp.RecieverID},{tp.RecieverTPBack},{tp.KeepYVelocity},{tp.force.x},{tp.force.y},{tp.teleportObjects}\n";
                    break;
                case 9:
                    var ch = levelObjects[i].GetComponent<Checkpoint>();
                    finalOutput += $"{ch.isEnabled},{ch.priority},{ch.startPos}\n";
                    break;
                default:
                    finalOutput += "\n";
                    break;
            }
        }
        popups.InstantiatePopup("Saved!");

        if (reload && lp.HasValue)
        {
            if (!(lp.Value.Name + ".level").Equals(MainEditorComponent.levelName, StringComparison.InvariantCulture))
            {
                QuickSaveRaw.Delete(MainEditorComponent.levelName);
            }
            levelName = lp.Value.Name;
            isOfficial = false;
            LevelFile = finalOutput;
            LevelContents = finalOutput;
        }
        
        QuickSaveRaw.SaveString(levelName + ".level", finalOutput);
        if (reload) SceneManager.LoadScene("ActualEditor");
        
    }
    IEnumerator AutoSave()
    {
        yield return new WaitForSeconds(autoSaveSeconds);

        if (editorControls.pstate != EditorControls.PlayState.stop)
        {
            Debug.Log("Waiting until pause..");
            yield return new WaitUntil(() => editorControls.pstate == EditorControls.PlayState.stop);

        }
        else Save();

        StartCoroutine(AutoSave());
    }

    public void Restart()
    {
        isWorkshop = true;
        LevelFile = workshopLevel.Title;
        levelName = workshopLevel.Title + ".level";
        LevelContents = File.ReadAllText(workshopLevel.Directory + @"\" + workshopLevel.Title + ".level");
        isOfficial = false;
        Time.timeScale = 1;
        int index = LevelSearch.levelList.IndexOf(gameObject);
        AchievementManager.AddWorkshop();

        SceneManager.LoadScene("ActualEditor");
    }
    private protected void LoadLevel(string level)
    {
        #region Checks
        player.GetComponent<CharacterController2D>().slideParticlesLeft.gameObject.SetActive(false);
        player.GetComponent<CharacterController2D>().slideParticlesRight.gameObject.SetActive(false);
        beat = !bool.Parse(ReadLine(level, 3));
        try
        {
            levelVersion = ReadLine(level, 2).Replace(".", String.Empty);
        }
        catch { Throw("Level version is invalid. (Line 2)");
            return;
        }
        // Verification Token Check
        if (ReadLine(level, 1) != "gameprototype12")
        {
            Throw("Verification Token Invalid. (Line 1)");
            return;
        }

        // Application Version Check
        else if (int.Parse(ReadLine(level, 2).Replace(".", String.Empty)) > int.Parse(Application.version.Replace(".", String.Empty)) && !debug)
        {
            Debug.LogError($"Incompatible Level: Level version ({ReadLine(level, 2)}) is not compatible with your game version");
            Throw("Incompatible Level: Level version is higher than your version.\nTry updating the game. (Line 2)");
            return;
        }

        // Title Length Check
        else if (ReadLine(level, 5).Length > 32)
        {
            Debug.LogError("Incompatible Level: Name cannot be more than 32 characters");
            Throw("Level name is over the character limit. (32) (Line 4)");
            return;
        }


        

        #endregion

        #region Basic Settings
        
        if (workshopEditable && isWorkshop) editorControls.copy.SetActive(true);
        
        //Play the music
        int Music = Mathf.Clamp(int.Parse(ReadLine(level, 4), NumberStyles.Any, CultureInfo.InvariantCulture), 0, track.Length - 1);
        source.clip = track[Music];
        //Get the level name
        if (!isOfficial)
        levelName = Path.GetFileNameWithoutExtension(LevelPath);
        levelTitle.text = levelName;

		//Update discord
        try
        {
			Discord_Controller dcontroller = GameObject.Find("DiscordController")?.GetComponent<Discord_Controller>();
			if (editable) dcontroller.details = $"Editing {levelName}";
			else dcontroller.details = $"Playing {levelName}";
        }
        catch
        {

        }

		//Place the walls
		left.transform.position = new Vector3(-(float.Parse(ReadLine(level, 8).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2 + (left.transform.localScale.x / 2)), 0, 0);
        right.transform.position = new Vector3(float.Parse(ReadLine(level, 8).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2 + (right.transform.localScale.x / 2), 0, 0);
        down.transform.position = new Vector3(0, -(float.Parse(ReadLine(level, 9).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2 + (up.transform.localScale.y / 2)), 0);
        up.transform.position = new Vector3(0, float.Parse(ReadLine(level, 9).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2 + (down.transform.localScale.y / 2), 0);
        levelWidth = float.Parse(ReadLine(level, 8).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2;
        levelHeight = float.Parse(ReadLine(level, 9).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2;
        //Place the portals
        enterPortal.transform.position = new Vector3(-(float.Parse(ReadLine(level, 8).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2), Mathf.Clamp(float.Parse(ReadLine(level, 10).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture), -levelHeight + 2, levelHeight - 2), 0);
        leavePortal.transform.position = new Vector3(float.Parse(ReadLine(level, 8).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture) / 2, Mathf.Clamp(float.Parse(ReadLine(level, 11).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture), -levelHeight + 2, levelHeight - 2), 0);
        //leavePortal.GetComponent<Portal>().isOpen = bool.Parse(ReadLine(level, 13));

        if (levelHeight > 200 || levelWidth > 200)
        {
            Throw("Level width or height is over the limit. (200) (Line 8/9)");
            return;
        }
        //Spawn the player in the spawner position
        player.transform.position = spawner.transform.position;
        cineEnabled = true;

        //Deactivate lasers if the line 12 of the file says false
        if (ReadLine(level, 12).ToLower() == "false") lazers.SetActive(false);
        else 
        {
            lazers.transform.position = new Vector3(0, -(float.Parse(ReadLine(level, 9).Replace(',', '.')) / 2 - 0.3f), 0);
            lazers.transform.localScale = new Vector3(float.Parse(ReadLine(level, 8).Replace(',', '.')), 1, 1);
        }


        #endregion

        #region Object Recognition
        for (int i = 16; i < CountLines(level); i++)
        {
            int obj;
            float xpos;
            float ypos;
            float rotx;
            float rotz;
            float scalex;
            float scalez;
            bool col;
            string misc;
            
            obj = int.Parse(ReadLine(level, i).Replace(',', '.'));
            if (obj > placeableObjects.Length)
            {
                Throw($"Invalid Object ID. (Line {i})");
                return;
            }
            i++;
            xpos = float.Parse(ReadLine(level, i), NumberStyles.Any, CultureInfo.InvariantCulture);
            i++;
            ypos = float.Parse(ReadLine(level, i), NumberStyles.Any, CultureInfo.InvariantCulture);
            if (xpos > 5000 || ypos > 5000)
            {
                Throw($"Object position x or y is over the limit. (5000) (Line {i})");
                return;
            }
            i++;
            rotx = float.Parse(ReadLine(level, i), NumberStyles.Any, CultureInfo.InvariantCulture);
            i++;
            rotz = float.Parse(ReadLine(level, i), NumberStyles.Any, CultureInfo.InvariantCulture);
            i++;
            scalex = float.Parse(ReadLine(level, i), NumberStyles.Any, CultureInfo.InvariantCulture);
            i++;
            scalez = float.Parse(ReadLine(level, i), NumberStyles.Any, CultureInfo.InvariantCulture);
            if (scalex > 200 || scalez > 200)
            {
                Throw($"Object scale x or y is over the limit. (200) (Line {i})");
                return;
            }
            i++;
            col = Convert.ToBoolean(ReadLine(level, i));
            i++;
            misc = ReadLine(level, i);


            i -= 8;

            CreateObject(obj, xpos, ypos, rotz, scalex, scalez, col, misc);
            i += 8;
            
            if (buttons.Length < 1)
            {
                portalout.GetComponent<Portal>().isOpen = true;
                portalout.GetComponent<MeshRenderer>().material = portalout.GetComponent<Portal>().door_enabled;
                portalout.GetComponent<Portal>().door_particles.material = portalout.GetComponent<Portal>().button_enabled;
            }
            else
            {
                portalout.GetComponent<Portal>().isOpen = false;
                portalout.GetComponent<MeshRenderer>().material = portalout.GetComponent<Portal>().door_disabled;
                portalout.GetComponent<Portal>().door_particles.material = portalout.GetComponent<Portal>().button_disabled ;
            }

            //Debug.Log("Done");
            if (editable) Save();
        }
        #endregion

        Debug.Log("Editor load finished");
    }
    
    #region Object Creation
    public void CreateObject(int id, 
                             float xpos, 
                             float ypos, 
                             float rotz, 
                             float scalex, 
                             float scalez, 
                             bool col, 
                             string misc) 
    {
        
        GameObject newObject;
        string[] miscSplit;
        string miscNorm;
        string customMisc;
        miscSplit = misc.Split("|");
        miscNorm = miscSplit[0];
        customMisc = miscSplit[1];

        customMisc = customMisc.Replace(',', '\n');
        miscNorm = miscNorm.Replace(',', '\n');

        string backup = miscNorm;

        
        
        if (int.Parse(ReadLine(LevelContents, 2).Replace(".", "")) < 110)
        {
            print(ReadLine(miscNorm, 2));
            if (float.TryParse(ReadLine(miscNorm, 2), out float a)) miscNorm = ReplaceLine(miscNorm, 2, "false");
            miscNorm = ReplaceLine(miscNorm, 3, "true");
            miscNorm = ReplaceLine(miscNorm, 4, "true");
            miscNorm = ReplaceLine(miscNorm, 5, "true");
            miscNorm = ReplaceLine(miscNorm, 6, "false");
        }


        newObject = Instantiate(placeableObjects[id - 1], new Vector3(xpos, ypos, 0), Quaternion.Euler(0, 0, rotz));
        newObject.GetComponent<Moving>().isEnabled = bool.Parse(ReadLine(miscNorm, 1));
        Moving m = newObject.GetComponent<Moving>();
        if (int.Parse(ReadLine(LevelContents, 2).Replace(".", "")) < 110)
        {
            miscNorm = ReplaceLine(miscNorm, 7, $"{float.Parse(ReadLine(backup, 4).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture)}!" +
            $"{float.Parse(ReadLine(backup, 5).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture)}!" +
            $"{float.Parse(ReadLine(backup, 6).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture)}!" +
            $"{float.Parse(ReadLine(backup, 7).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture)}!0~");
        }

        m.isEnabled = bool.Parse(ReadLine(miscNorm, 1).ToLower());
        m.restartWhenDeath = bool.Parse(ReadLine(miscNorm, 2).ToLower());
        m.instantlyStart = bool.Parse(ReadLine(miscNorm, 3).ToLower());
        m.loop = bool.Parse(ReadLine(miscNorm, 4).ToLower());
        m.goBack = bool.Parse(ReadLine(miscNorm, 5).ToLower());
        m.useTime = bool.Parse(ReadLine(miscNorm, 6).ToLower());

        string[] pointProperties = ReadLine(miscNorm, 7).Split('~');

        foreach (string pointProperty in pointProperties)
        {
            
            if (pointProperty == String.Empty)
            {
                continue;
            }

            Points point = new Points();
            string[] properties = pointProperty.Split('!');
            

            point.to.x = float.Parse(properties[0]);
            point.to.y = float.Parse(properties[1]);
            point.offset = float.Parse(properties[2]);
            point.speedortime = float.Parse(properties[3]);
            point.ease = (Points.CustomEases)int.Parse(properties[4]);
            m.points.Add(point);
        }


        //m.to.x = float.Parse(ReadLine(miscNorm, 4).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture); 
        //m.to.y = float.Parse(ReadLine(miscNorm, 5).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture); 
        //m.wait = float.Parse(ReadLine(miscNorm, 6).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
        //m.speed = float.Parse(ReadLine(miscNorm, 7).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);

        Spin s = newObject.GetComponent<Spin>();
        s.isEnabled = bool.Parse(ReadLine(miscNorm, 8));
        s.amount = float.Parse(ReadLine(miscNorm, 9).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);

        goddamnit.Physics p = newObject.GetComponent<goddamnit.Physics>();
        try
        {
            p.isEnabled = bool.Parse(ReadLine(miscNorm, 10));
            p.mass = float.Parse(ReadLine(miscNorm, 11).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture);
            p.lockX = bool.Parse(ReadLine(miscNorm, 12));
            p.lockY = bool.Parse(ReadLine(miscNorm, 13));
            p.lockZ = bool.Parse(ReadLine(miscNorm, 14));
        }
        catch
        {
            p.isEnabled = false;
            p.mass = 0;
            p.lockX = false;
            p.lockY = false;
            p.lockZ = false;
        }

        try
        {
            p.restartOnDeath = bool.Parse(ReadLine(miscNorm, 15));
        }
        catch { p.restartOnDeath = false; }

        newObject.transform.localScale = new Vector3(scalex, scalez, 1f);
        switch (id)
        {
            case 1 or 7:
                int offset = 0;
                if (id == 7)
                {
                    offset = 2;
                    TextComponent tc = newObject.GetComponent<TextComponent>();

                    tc.ChangeText(ReadLine(customMisc, 1).Replace('\t', '\n'));
                    tc.ChangeFontSize(float.Parse(ReadLine(customMisc, 2).Replace(',', '.')));
                    try
                    {
                        tc.bold = bool.Parse(ReadLine(customMisc, 11));
                        tc.italic = bool.Parse(ReadLine(customMisc, 12));
                    }
                    catch
                    {
                        print("FUCKKKKKKKKKKKKKKKKKKKKKKKKK");
                        tc.bold = false;
                        tc.italic = false;
                    }
                }
                ColorComponent cc = newObject.GetComponent<ColorComponent>();
                cc.Enable();
                
                try
                {
                    cc.enabled = bool.Parse(ReadLine(customMisc, 1 + offset));
                    cc.r = int.Parse(ReadLine(customMisc, 2 + offset));
                    cc.g = int.Parse(ReadLine(customMisc, 3 + offset));
                    cc.b = int.Parse(ReadLine(customMisc, 4 + offset));
                    cc.a = int.Parse(ReadLine(customMisc, 5 + offset));
                    cc.g_enabled = bool.Parse(ReadLine(customMisc, 6 + offset));
                    cc.g_r = int.Parse(ReadLine(customMisc, 7 + offset));
                    cc.g_g = int.Parse(ReadLine(customMisc, 8 + offset));
                    cc.g_b = int.Parse(ReadLine(customMisc, 9 + offset));
                    cc.g_a = int.Parse(ReadLine(customMisc, 10 + offset));
                }
                catch
                {
                    print("EPIC FAIL!!!");
                    cc.enabled = false;
                    cc.r = 255;
                    cc.g = 255;
                    cc.b = 255;
                    cc.a = 255;
                    cc.g_enabled = false;
                    cc.g_r = 255;
                    cc.g_g = 255;
                    cc.g_b = 255;
                    cc.g_a = 255;
                }
                break;
            case 3:
                Button b = newObject.GetComponent<Button>();
    
                portalout.GetComponent<Portal>().isOpen = false;
                portalout.GetComponent<MeshRenderer>().material = portalout.GetComponent<Portal>().door_disabled;
                portalout.GetComponent<Portal>().door_particles.material = portalout.GetComponent<Portal>().button_disabled;
                b.offOnRestart = bool.Parse(ReadLine(customMisc, 1));
                b.turnBackOff = bool.Parse(ReadLine(customMisc, 2));
                b.immediatelyActivate = bool.Parse(ReadLine(customMisc, 3));
                Array.Resize(ref buttons, buttons.Length + 1);
                buttons[buttons.Length - 1] = newObject;
                break;
            case 4:
                Turret t = newObject.GetComponent<Turret>();
                t.targetPlayer = bool.Parse(ReadLine(customMisc, 1));
                t.cooldown = float.Parse(ReadLine(customMisc, 2).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                t.bulletSpeed = float.Parse(ReadLine(customMisc, 3).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                t.offset = float.Parse(ReadLine(customMisc, 4).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                break;
            case 5:
                Launchpad lp = newObject.GetComponentInChildren<Launchpad>();
                lp.x = float.Parse(ReadLine(customMisc, 1).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                lp.y = float.Parse(ReadLine(customMisc, 2).Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                break;
            case 8:
                try
                {
                    TeleporterComponentScript tps = newObject.GetComponent<TeleporterComponentScript>();
                    tps.WorkAsReciever = bool.Parse(ReadLine(customMisc, 1));
                    tps.RecieverID = int.Parse(ReadLine(customMisc, 2));
                    tps.RecieverTPBack = bool.Parse(ReadLine(customMisc, 3));
                    tps.KeepYVelocity = bool.Parse(ReadLine(customMisc, 4));
                    tps.force.x = float.Parse(ReadLine(customMisc, 5));
                    tps.force.y = float.Parse(ReadLine(customMisc, 6));
                    tps.teleportObjects = bool.Parse(ReadLine(customMisc, 7));
                    tps.Update();
                }
                catch
                {
                    
                }
                blackHoles.Add(newObject);
                break;
            case 9:
                Checkpoint ch = newObject.GetComponent<Checkpoint>();
                ch.isEnabled = bool.Parse(ReadLine(customMisc, 1));
                ch.priority = int.Parse(ReadLine(customMisc, 2));
                ch.startPos = bool.Parse(ReadLine(customMisc, 3));
                checkpoints.Add(ch);
                break;
            
            
        }
        Array.Resize(ref levelObjects, levelObjects.Length + 1);
        newObject.GetComponent<ObjectComponent>().misc = misc;
        levelObjects[levelObjects.Length - 1] = newObject;
    }
    #endregion

    #region Utilities

    public static string RemoveLine(string text, int lineNumber)
    {
        string[] lines = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lineNumber < 1 || lineNumber > lines.Length)
        {
            Debug.LogError("Line number out of range");
            return text;
        }
        var modifiedLines = lines.Where((line, index) => index != lineNumber - 1);
        return string.Join(System.Environment.NewLine, modifiedLines);
    }

    public static string ReplaceLine(string text, int lineNumber, string replacement)
    {
        string[] lines = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lineNumber < 1 || lineNumber > lines.Length)
        {
            Debug.LogError("Line number out of range");
            return text;
        }
        lines[lineNumber - 1] = replacement;
        return string.Join(System.Environment.NewLine, lines);
    }


    public static int CountLines(string content)
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
    public string ReplaceInvalidChars(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
    public static string ReadLine(string text, int lineNumber)
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

    //public static string ReadLine(string text, int lineNumber)
    //{
//
    //    //if (!text.Equals(prevText)) Instance.splitString = text.Split('\n');
    //    //
    //    //prevText = text;
    //    int a = 0;
    //    int b = 0;
    //    foreach (char i in text)
    //    {
    //        if (i == '\n') b++;
    //        
    //        a++;
    //    }
    //    return text.Split('\n')[lineNumber - 1];
    //}
    #endregion

    #region Editor State
    public void Play()
    {
        editorTrail.Clear();

        source.Play();
        player.GetComponent<CharacterController2D>().slideParticlesLeft.gameObject.SetActive(true);
        player.GetComponent<CharacterController2D>().slideParticlesRight.gameObject.SetActive(true);
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].GetComponent<ObjectComponent>().Play();
        }
        if (buttons.Length < 1)
        {
            portalout.GetComponent<Portal>().isOpen = true;
            portalout.GetComponent<MeshRenderer>().material = portalout.GetComponent<Portal>().door_enabled;
            portalout.GetComponent<Portal>().door_particles.material = portalout.GetComponent<Portal>().button_enabled;
        }
        try
        {
            Discord_Controller dcontroller = GameObject.Find("DiscordController").GetComponent<Discord_Controller>();
            dcontroller.state = "Attempt: 0";
        }
		catch
        {

        }
	}
    public void Pause(bool b)
    {
        
        for (int i = 0; i < levelObjects.Length; i++)
        {
            if (b)
            {

                levelObjects[i].GetComponent<ObjectComponent>().Pause();
            }
            else
            {
                levelObjects[i].GetComponent<ObjectComponent>().Unpause();
            }
            
        }
        if (!b) source.UnPause();
        else source.Pause();
	}
    public bool boss;
    public void Death()
    {
        for (int i = 0; i < levelObjects.Length; i++)
        {
            if (levelObjects[i].TryGetComponent(out goddamnit.Physics phy))
            {
                if (phy.restartOnDeath)
                {
                    phy.rigid.isKinematic = true;
                    levelObjects[i].transform.eulerAngles = phy.startRot;
                    levelObjects[i].transform.position = phy.startPos;
                    phy.rigid.velocity = Vector3.zero;
                    phy.rigid.isKinematic = false;

                }
                
            }
            if (levelObjects[i].GetComponent<Moving>())
            {
                Moving m = levelObjects[i].GetComponent<Moving>();
                if (m.restartWhenDeath)
                {
                    //if (m.stop != null) StopCoroutine(m.stop);
                    //m.cont = false;
                    //m.movingToB = true;
                    levelObjects[i].transform.position = m.startPos;
                    if (m.nextPoint != null) m.StopCoroutine(m.nextPoint);
                    m.transform.DOKill();
                    m.OnStart(false);
                }
            }
        }
    }

    public void Stop()
    {
        source.Stop();
        player.GetComponent<Die>().deathCounter = 1;
        player.GetComponent<Die>().attempts.text = "Attempt " + 1;
        player.GetComponent<CharacterController2D>().slideParticlesLeft.gameObject.SetActive(false);
        player.GetComponent<CharacterController2D>().slideParticlesRight.gameObject.SetActive(false);
        for (int i = 0; i < levelObjects.Length; i++)
        {
            levelObjects[i].GetComponent<ObjectComponent>().Stop();
        }
        pauseMenu.Resume();
        try
        {
            Discord_Controller dcontroller = GameObject.Find("DiscordController").GetComponent<Discord_Controller>();
            dcontroller.state = "";
        }

        catch
        {

        }
	}
    #endregion



    public void OnApplicationQuit()
    {
        if (debug) 
            QuickSaveRaw.Delete(LevelFile);
    }
}
