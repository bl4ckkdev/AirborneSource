// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using TMPro;
using UnityEngine;
using EZCameraShake;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Die : MonoBehaviour
{
    public GameObject player;
    public Transform spawner;
    public AudioSource paudio;
    public EditorControls ec;

    public TextMeshProUGUI attempts;
    public int deathCounter = 0;
    public Transform playerDie;
    protected private bool cdd;
    public static Die Instance;
    public bool finale;
    public bool isEditor = true;
    public bool dead, invincible;
    
    [Space]
    public GameObject deadMenu;
    private void OnCollisionEnter(Collision col)
    {
        if ((col.gameObject.CompareTag("enemy") || col.gameObject.CompareTag("Boss")) && !invincible)
        {
            //call function // NAHH XDEV WHAT IS THIS COMMENT
            PDie();
        }


    }
    public AudioSource launch;
    private void Awake()
    {
        Instance = this;
    }
    public void Update()
	{
        if (transform.position.y < -30 && finale)
        {
            PDie();
        }
        if (Input.GetKeyUp(KeyCode.R) && MainEditorComponent.isOfficial)
        {
            
            if (PlayerPrefs.GetInt("Speedrun", 0) == 1)
            {
                ActualSpeedrunTimer.instance.ResetTimer();

                if (PlayerPrefs.GetInt("SpeedrunStart", 1) == 50)
                {
                    MainMenu.Singleton.LoadSceneFancy("Level 50");
                    return;
                }
                    
                List<string> LevelNames;
                string[] levelArray = Directory.GetFiles(Path.Combine(Application.dataPath, "Assets", "Levels"), "*.level");
                
                int[] levelNamesToInt = new int[levelArray.Length];
                for (int i = 0; i < levelArray.Length; i++)
                {
                    levelArray[i] = Path.GetFileNameWithoutExtension(levelArray[i]);
                }
                for (int i = 0; i < levelArray.Length; i++) {
                    levelNamesToInt[i] = int.Parse(levelArray[i].Substring(6));
                }
                Array.Sort(levelNamesToInt, levelArray);
                LevelNames = levelArray.ToList();

                MainEditorComponent.editable = false;
                MainEditorComponent.isWorkshop = false;
                MainEditorComponent.LevelContents = File.ReadAllText(Path.Combine(Application.dataPath, "Assets", "Levels") + @$"\Level {PlayerPrefs.GetInt("SpeedrunStart", 1)}.level");
                MainEditorComponent.LevelFile = $"Level {PlayerPrefs.GetInt("SpeedrunStart", 1)}";
                MainEditorComponent.levelName = $"Level {PlayerPrefs.GetInt("SpeedrunStart", 1)}";
                MainEditorComponent.isOfficial = true;
                if (PlayerPrefs.GetInt("SpeedrunStart") - 1 >= 0) LevelNames.RemoveRange(0, PlayerPrefs.GetInt("SpeedrunStart") - 1);
                
                MainEditorComponent.NextLevels = LevelNames;
                
                MainMenu.Singleton.LoadSceneFancy("ActualEditor");
            }
            PDie(false);
        }
        else if (Input.GetKeyUp(KeyCode.R)) PDie(true);
        if (Input.GetKeyDown(KeyCode.T))
        {
            PDie(false);
        }
			
	}

    public bool dont;
    public List<GameObject> savedButtons;
	public void PDie(bool sound = true) // C# doesn't like it when a function has the same name as its class
    {
        if (cdd) return;
        if (deathCounter != 0)
        {
                Instantiate(playerDie, transform.position, Quaternion.identity);
                AchievementManager.AddDeath();
                CameraShaker.Instance.ShakeOnce(3, 1, 0f, 0.3f);
                GetComponent<AudioSource>().Play();

            if (!MainEditorComponent.Instance.finale && MainEditorComponent.Instance.editorTrail.enabled)
            {
                MainEditorComponent.Instance.editorTrail.enabled = false;
                MainEditorComponent.Instance.editorTrail.Clear();
                MainEditorComponent.Instance.editorTrail.enabled = true;
            }

            if (isEditor) ec.mainEditorComponent.Death();
            
            if (finale && !dont)
            {
                dont = true;
                DOTween.To(() => Music.instance.source.pitch, x => Music.instance.source.pitch = x, 0.1f, 0.5f);
                PauseMenu.Instance.dont = true;
                Bossfight.Instance.bossAttemptCount.text = $"Attempt {deathCounter}";
                Bossfight.Instance.higherFOV.Follow = transform;
                GetComponentInChildren<MeshRenderer>().enabled = false;
                player.GetComponent<PlayerMovement>().enabled = false;
                DOTween.To(() => Bossfight.Instance.higherFOV.m_Lens.FieldOfView, x => Bossfight.Instance.higherFOV.m_Lens.FieldOfView = x, 20, 0.3f);
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.1f, 0.5f).OnComplete(() =>
                {
                    DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.5f).OnComplete(() =>
                    {
                        Music.instance.source.pitch = 1;
                        Music.instance.source.Stop();
                        Music.instance.source.Play();
                        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        Bossfight.Instance.StopAttacks();
                        Bossfight.Instance.DoNothing();
                        player.transform.position = new Vector3(0, -10, 0);
                        GetComponentInChildren<MeshRenderer>().enabled = true;
                        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        transform.GetChild(1).transform.localScale = Vector3.zero;
                        player.GetComponent<PlayerMovement>().enabled = true;
                        DOTween.To(() => Bossfight.Instance.higherFOV.m_Lens.FieldOfView, x => Bossfight.Instance.higherFOV.m_Lens.FieldOfView = x, 42, 0.3f);
                        
                        transform.GetChild(1).transform.DOScale(1, 0.5f).OnComplete(
                            () =>
                            {
                                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                                
                            });
                        Bossfight.Instance.DoSomething();
                        Bossfight.Instance._attackCount = 0;
                        PauseMenu.Instance.dont = false;
                        Bossfight.Instance.health = 65;
                        dont = false;
                        DOTween.To(() => Bossfight.Instance.healthbar.value, x => Bossfight.Instance.healthbar.value = x, 65, 1f);
                    });
                });
            }

        }
        
        
        
        StartCoroutine(cd());
        if (isEditor) for (int i = 0; i < ec.mainEditorComponent.buttons.Length; i++)
        {
            
            if (!savedButtons.Contains(ec.mainEditorComponent.buttons[i]) && ec.mainEditorComponent.buttons[i].GetComponentInChildren<Button>().isEnabled && ec.mainEditorComponent.buttons[i].GetComponentInChildren<Button>().offOnRestart) 
                ec.mainEditorComponent.buttons[i].GetComponentInChildren<Button>().Deactivate();
        }
        
        deathCounter++;
		try
		{
            
            Discord_Controller dcontroller = GameObject.Find("DiscordController").GetComponent<Discord_Controller>();
			dcontroller.state = "Attempt: " + deathCounter;
		}
		catch { } // those gosh darn discord sdks

        
        
		attempts.text = "Attempt " + deathCounter;
            if (MainEditorComponent.Instance.activeCheckpoint != null)
            {
                player.transform.position = MainEditorComponent.Instance.activeCheckpoint.transform.position;
            }
            else
            player.transform.position = spawner.position;
	}

    IEnumerator cd()
    {
        cdd = true;
        yield return new WaitForSeconds(0.1f);
        cdd = false;
    }
}
