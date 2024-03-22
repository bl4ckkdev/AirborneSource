// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    public bool isEditor;
    public Music music;
    public MainEditorComponent mainEditorComponent;
    public GameObject PauseUI;
    public GameObject exit;

    public string sceneToLoad;
    public bool finale;
    public GameObject skip, exploration;

    public static PauseMenu Instance;

    public GameObject speedrun;

    public bool dont;
    
    private void Start()
    {
        Instance = this;
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>();

        if (PlayerPrefs.GetInt("Exploration") == 1 && finale)
        {
            skip.SetActive(true);
            exploration.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Beat") == 1 && finale)
        {
            skip.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Speedrun", 0) == 1 && MainEditorComponent.isOfficial)
        {
            speedrun.SetActive(true);
        }
    }
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))) && (mainEditorComponent.editorControls.pstate == EditorControls.PlayState.stop) && !dont)
        {
			if (isPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
    }
    public AudioSource pauseMusic;
    public void Resume()
    {
        if (!finale)
        {
            mainEditorComponent.editorControls.cantScroll = false;
            mainEditorComponent.editorControls.canNumbers = true;
        }
        if (!mainEditorComponent.finale) Music.instance.Resume();
        pauseMusic.Stop();
        PauseUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;

        if (mainEditorComponent.finale)
        {
            if (MainEditorComponent.Instance.editorControls.sta && !Bossfight.Instance.cutscene)
            {
                Music.instance.Resume();
            }
            else if (MainEditorComponent.Instance.editorControls.sta)
            {
                if (!Bossfight.Instance.cutscene)
                Music.instance.Resume();
            }
            else Music.instance.Resume();
        }
        
    }
    public void Pause()
    {
        Music.instance.Pause();
        pauseMusic.Play();
        Cursor.visible = true;
        exit.SetActive(false);
        PauseUI.SetActive(true);
        Time.timeScale = 0f;
        
        isPaused = true;
    }
    public void Quit() 
    {
        Cursor.visible = true;
        PauseUI.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
        SceneManager.LoadScene(sceneToLoad);  
    }

    public void SkipToBoss()
    {
        Die.Instance.deathCounter = 5;
        Die.Instance.ec.mainEditorComponent.portalout.GetComponent<Portal>().isOpen = true;
        Die.Instance.transform.position = new Vector3(37, 0, 0);
    }
}
