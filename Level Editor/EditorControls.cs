// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class EditorControls : MonoBehaviour
{
    public Camera mainCam;

    public Toggle[] buttons = new Toggle[11];
    public Image[] buttonsimg = new Image[11];

    public Sprite[] selectedSprites = new Sprite[11];
    public Sprite[] deselectedSprites = new Sprite[11];

    public Image[] playImg = new Image[2];
    public Toggle[] playButtons = new Toggle[2];
    public Sprite[] playSprites = new Sprite[4];

    public MainEditorComponent mainEditorComponent;

    public GameObject PlayCanvas;
    public GameObject BuildCanvas;
    public bool PauseTimer;
    public bool canNumbers = true;
    public bool UIStop;

    public Music music;

    float camFOV;
    Vector3 camPos;

    public int selectedButton;
    public PlayState pstate;

    public Material selectedMat;
    public Material errorMat;
    public Material blue, red;

    public PauseMenu pMenu;
    public LayerMask placeIgnore;
    public LayerMask deleteIgnore;
    public LayerMask selectIgnore;
    public LayerMask grid;
    public Vector3 mousePos;
    public Vector3 snapMousePos;

    public GameObject empty;

    public bool canObjectBePlaced;
    public bool selectedObject;
    public bool cantScroll;
    public bool nuhUh;
    
    public GameObject[] clipboard;
    
    public GameObject propertiesWindow, cycleMenu;
    public GameObject editorUI;
    public bool cantContinue;

    public GameObject cine;

    public ObjectProperties objProperties;

    public GameObject positionUI, rotationUI, scaleUI, portalPositionUI;
    public GameObject placeObject;



    [SerializeField] GameObject lastSelected;
    public GameObject[] lastSelectedArray;

    public uType UIType;

    public Toggle pos, scale, rot;

    public GameObject[] allObjects;

    public Quaternion smoothRot;

    public bool isPlayerPaused;
    public GameObject playerPauseUI;
    public UnityEngine.UI.Button upload;
    public GameObject copy;
    public enum PlayState
    {
        play = 0,
        pause = 1,
        stop = 2
    }     
    
    public enum uType
    {
        pos,
        rot,
        scale,
        none
    }

    public GameObject uploadPanel, settingsCanvas, canvas;
    public LevelSettings levelSettings;

    public GameObject selectedPortal;

    public GameObject[] walls;
    public Material border, transparent;
    public Material textSelectedMat, textErrorMat;

    public LayerMask env;

    public bool spawnPairObjects;

    public void HandleKeybinds()
    {
        // Insert comment here
        if (pstate == PlayState.stop && !cantContinue)
        {
            if (Input.GetKeyDown(KeyCode.U) && upload.interactable) uploadPanel.SetActive(!uploadPanel.activeSelf);
            else if (Input.GetKeyDown(KeyCode.M)) { levelSettings.Init(); settingsCanvas.SetActive(true); }
            else if (Input.GetKeyDown(KeyCode.P)) { playButtons[0].isOn = false; Play(); canvas.SetActive(false);  }

            // Camera move
            float moveSpeed = 20.0f;
            float deceleration = 20.0f;

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f);
            Vector3 moveVector = moveDirection * moveSpeed * Time.deltaTime;

            mainCam.transform.Translate(moveVector);

            // Decelerate the camera movement to make it stop faster
            if (moveVector.magnitude > 0.01f) moveVector -= moveVector * deceleration * Time.deltaTime;
            else moveVector = Vector3.zero;

            Vector3 clampedPosition = mainCam.transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -86.1f, 86.1f);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -78, 78);
            clampedPosition.z = -32;
            mainCam.transform.position = clampedPosition;

            // Camera zoom
            // I removed debugThing
            float intensity = 0.5f;

            if (Input.GetKeyDown(KeyCode.Equals)) mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView += -intensity * 15, 15, 120);
            else if (Input.GetKeyDown(KeyCode.Minus)) mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView += intensity * 15, 15, 120);
        }
        else if (pstate == PlayState.play)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause();
                playButtons[1].isOn = !playButtons[1].isOn;
            }
        }
        else if (pstate == PlayState.pause)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                playButtons[1].isOn = !playButtons[1].isOn;
                Pause();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleButton(0);
        
        if (Input.GetKey(KeyCode.LeftControl) && pstate == PlayState.stop && canNumbers)
        {
            //ToggleButton((selectedButton + Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 5)) % 9);
            
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0) ToggleButton((selectedButton + 1) % 10);
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                if (selectedButton - 1 < 0) ToggleButton(8);
                else ToggleButton((selectedButton - 1) % 10);
            }
        }


    }

    private void Awake()
    {
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<Music>();
        if (MainEditorComponent.isOfficial)
        {
            levelNum = int.Parse(mainEditorComponent.levelTitle.text.Substring(6));
            pool.override_ = true;
            mainEditorComponent.source.enabled = false;
            if (levelNum <= 10)
                pool.music = new[] { mainEditorComponent.track[0] };
            else if (levelNum <= 20)
                pool.music = new[] { mainEditorComponent.track[1] };
            else if (levelNum <= 30)
                pool.music = new[] { mainEditorComponent.track[2] };
            else if (levelNum <= 40)
                pool.music = new[] { mainEditorComponent.track[3] };
            else if (levelNum < 50)
                pool.music = new[] { mainEditorComponent.track[4] };


            //else if (levelNum == 50)
            //    pool.music = new[] { mainEditorComponent.track[5] };

            pool.Start();
            Music.instance.started = true;
            
        }
        pool.Start();
    }

    public Toggle spawn2ObjectsToggle;
    public void Spawn2ObjectsToggle(bool i)
    {
        PlayerPrefs.SetInt("Spawn2Objects", i ? 1 : 0);
        spawnPairObjects = i;
    }

    public Toggle showEditorTrail;
    public void ShowEditorTrail(bool i)
    {
        PlayerPrefs.SetInt("EditorTrail", i ? 1 : 0);
        mainEditorComponent.editorTrail.enabled = i;
    }

    public Toggle mute;
    public void Mute(bool i)
    {
        PlayerPrefs.SetInt("Mute", i ? 1 : 0);
        if (!MainEditorComponent.isOfficial) music.Mute(i);
    }
    public int levelNum;
    public MusicPool pool;
    public void Start()
    {
        if (levelNum - 1 == PlayerPrefs.GetInt("SpeedrunEnd", 0) && PlayerPrefs.GetInt("Speedrun", 0) == 1)
        {
            ActualSpeedrunTimer.instance.StopTimer();
        }
        if (mainEditorComponent.finale) return;

        if (PlayerPrefs.GetInt("EditorTrail", 0) == 0)
        {
            showEditorTrail.isOn = false;
        }
        if (PlayerPrefs.GetInt("Mute", 0) == 1) mute.isOn = true;
        if (PlayerPrefs.GetInt("Spawn2Objects", 1) == 1) spawn2ObjectsToggle.isOn = true;
        try
        {
            var editorOpened = new Steamworks.Data.Achievement("UNLEASH_YOUR_CREATIVITY");
            if (!editorOpened.State && MainEditorComponent.editable) editorOpened.Trigger();
        }
        catch { }
        ToggleButton(0);
        pstate = PlayState.stop;
        placeObject = Instantiate(empty);
        mainCam.gameObject.transform.position = new Vector3(0, 0, -32);
        SetUI(false);
        cine.SetActive(false);
        
        if (!MainEditorComponent.editable && pstate == PlayState.stop)
        {
            bottomWall.layer = 3;
            editorUI.SetActive(false);
            canObjectBePlaced = false;
            playImg[0].sprite = playSprites[1];
            playButtons[1].interactable = true;
            mainEditorComponent.Play();
            pstate = PlayState.play;
            PlayCanvas.SetActive(true);
            BuildCanvas.SetActive(false);
            cine.SetActive(mainEditorComponent.cineEnabled);
            placeObject.SetActive(false);
            PauseTimer = false;
            camFOV = mainCam.fieldOfView;
            if (!MainEditorComponent.isOfficial) music.Pause();
            camPos = mainCam.transform.position;
            if (!mainEditorComponent.cineEnabled)
            {
                mainCam.gameObject.transform.position = new Vector3(0, 0, -32);
                mainCam.fieldOfView = mainEditorComponent.fov;
            }
        }

    }
    public bool sta;
    public void Resume()
    {
        Time.timeScale = 1;
        isPlayerPaused = false;
        if (!mainEditorComponent.finale)
        {
            Music.instance.Resume();
        }
        else
        {
            print($"{sta} {Bossfight.Instance.cutscene}");
            if (sta && !Bossfight.Instance.cutscene)
            {
                Music.instance.Resume();
            }
            else Music.instance.Resume();
        }
        pauseMusic.Stop();
    }
    public AudioSource pauseMusic;
    public TMP_Text objectCount, levelVerified;
    private void Update()
    {
        if (mainEditorComponent.finale) return;

        
        objectCount.text = $"Objects: {mainEditorComponent.levelObjects.Length}/19000";

        upload.interactable = mainEditorComponent.beat;
        try
        {
            if (mainEditorComponent.beat)
            {
                levelVerified.text = "Verified";
            }
            else
            {
                levelVerified.text = "Unverified";
            }
        }
        catch { }


        if (!MainEditorComponent.editable && (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))))
        {
            if (!isPlayerPaused)
            {
                isPlayerPaused = true;
                playerPauseUI.SetActive(true);
                Music.instance.Pause();
                pauseMusic.Play();
                Time.timeScale = 0;
                mainEditorComponent.source.Pause();
            }
            else
            {
                isPlayerPaused = false;
                playerPauseUI.SetActive(false);
                Time.timeScale = 1;
                mainEditorComponent.source.UnPause();
                if (!mainEditorComponent.finale) Music.instance.Resume();
                pauseMusic.Stop();
            }
        }
        if (!MainEditorComponent.editable) return;
        HandleKeybinds();
        mousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 32));
        #region COLLISION DETECTION
        if (placeObject.transform.root.TryGetComponent(out TMP_Text tmep))
        {
            ChangeChildrenMaterials(placeObject.transform.root.gameObject, textSelectedMat, true, false, false);
        }
        if (placeObject.transform.root.TryGetComponent(out ColorComponent cc))
        {
            cc.Disable();
        }
        if (selectedButton > 0 && !pMenu.isPaused && pstate == PlayState.stop)

        {
            
            snapMousePos = new Vector3(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y), mousePos.z);
            RaycastHit rayHit;
            if (Input.GetKeyDown(KeyCode.E))
            {
                placeObject.transform.eulerAngles -= new Vector3(0, 0, 45);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                placeObject.transform.eulerAngles += new Vector3(0, 0, 45);
            }
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                placeObject.transform.position = snapMousePos;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                placeObject.transform.position = mousePos;
            }
            if (Physics.Raycast(new Vector3(mousePos.x, mousePos.y, -27), Vector3.forward, out rayHit, Mathf.Infinity, placeIgnore) || UIStop)
            {
                canObjectBePlaced = false;
                if (placeObject.transform.root.TryGetComponent(out TMP_Text tmp))
                {
                    ChangeChildrenMaterials(placeObject.transform.root.gameObject, textErrorMat, true, false, false);
                }
                else ChangeChildrenMaterials(placeObject.transform.root.gameObject, errorMat, true, false, false);
                mousePos.z = -2f;
            }
            else
            {
                canObjectBePlaced = true;
                if (placeObject.transform.root.TryGetComponent(out TMP_Text tmp))
                {
                    ChangeChildrenMaterials(placeObject.transform.root.gameObject, textSelectedMat, true, false, false);
                }
                else ChangeChildrenMaterials(placeObject.transform.root.gameObject, selectedMat, true, false, false);

            }
        }

        #endregion

        //this code fucking sucks
            #region OBJECT PLACEMENT
            if (Input.GetKey(KeyCode.LeftAlt))
            {
            if (Input.GetMouseButton(0) && canObjectBePlaced && selectedButton > 0 && selectedButton != 10)
            {
                mainEditorComponent.beat = false;
                if (selectedButton == 1)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|false,255,255,255,false,255,255,255");
                else if (selectedButton == 7)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|Sample Text,10,false,255,255,255,false,255,255,255,false,false");
                else if (selectedButton == 3)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|false,false,false");
                else if (selectedButton == 4)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|false,7,5,0");
                else if (selectedButton == 5)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|0,200");
                else if (selectedButton == 9)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|true,1,false");
                else if (selectedButton == 8)
                    if (spawnPairObjects)
                    {
                        int index = mainEditorComponent.blackHoles.Count / 2;
                        mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, $"false,false,0,0,0,0,0,false,0,false,1,false,false,false|false,{index},false,false");
                        mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x + 4,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, $"false,false,0,0,0,0,0,false,0,false,1,false,false,false|true,{index},false,false");
                    }
                    else
                    {
                        mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, "false,false,0,0,0,0,0,false,0,false,1,false,false,false|false,0,false,false");
                    }


                else mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, "false,0,0,0,0,0,0,false,0,false,1,false,false,false|");

            }
        }
            else
        {
            if (Input.GetMouseButtonDown(0) && canObjectBePlaced && selectedButton > 0 && selectedButton != 10)
            {
                mainEditorComponent.beat = false;
                if (selectedButton == 1)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|false,255,255,255,255,false,255,255,255,255");
                else if (selectedButton == 7)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|Sample Text,10,false,255,255,255,255,false,255,255,255,255,false,false");
                else if (selectedButton == 3)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|false,false,false");
                else if (selectedButton == 4)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|false,7,5,0");
                else if (selectedButton == 5)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|0,200");
                else if (selectedButton == 9)
                    mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                        placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                        placeObject.transform.localScale.x,
                        placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|true,1,false");
                else if (selectedButton == 8)
                    if (spawnPairObjects)
                    {
                        int index = mainEditorComponent.blackHoles.Count / 2;
                        mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, $"false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|false,{index},false,false");
                        mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x + 4,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, $"false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|true,{index},false,false");
                    }
                    else
                    {
                        mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|false,0,false,false");
                    }


                else mainEditorComponent.CreateObject(selectedButton, placeObject.transform.position.x,
                            placeObject.transform.position.y, placeObject.transform.localEulerAngles.z,
                            placeObject.transform.localScale.x,
                            placeObject.transform.localScale.y, true, "false,false,true,true,true,true,0!0!0!0!0~,false,0,false,1,false,false,false|");
            }
        }

        #endregion

        #region DELETION

        //if (Input.GetMouseButtonDown(0) && selectedButton == 8)
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(mousePos, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, deleteIgnore))
        //    {
        //        int s = Array.IndexOf(mainEditorComponent.levelObjects, hit.transform.root.gameObject);
        //        mainEditorComponent.levelObjects[s] = mainEditorComponent.levelObjects[mainEditorComponent.levelObjects.Length - 1];
        //        mainEditorComponent.levelObjects[mainEditorComponent.levelObjects.Length - 1] = null;
        //        Array.Resize(ref mainEditorComponent.levelObjects, mainEditorComponent.levelObjects.Length - 1);
        //        Destroy(hit.transform.root.gameObject);
        //    }
        //    
        //}

        if (Input.GetMouseButtonDown(0) && selectedButton == 10)
        {
            RaycastHit hit;
            if (Physics.Raycast(mousePos, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, deleteIgnore))
            {
                Delete(new [] {hit.transform.root.gameObject});
            }
        }

        else if (selectedButton == 10)
        {
            RaycastHit hit;
            if (Physics.Raycast(mousePos, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, deleteIgnore) && selectedButton == 10)
            {
                if (hit.transform.root.TryGetComponent(out TMP_Text tmp))
                {
                    ChangeChildrenMaterials(hit.transform.root.gameObject, textErrorMat, true, false, false);
                }
                else ChangeChildrenMaterials(hit.transform.root.gameObject, errorMat, true, false, false);
                if (lastSelected != null && hit.transform.root.gameObject != lastSelected) lastSelected.GetComponent<ObjectComponent>().ResetMaterials();
                lastSelected = hit.transform.root.gameObject;
                return;
            }
            else
            {
                if (lastSelected != null && lastSelected.GetComponent<ObjectComponent>() != null)
                {
                    lastSelected.GetComponent<ObjectComponent>().ResetMaterials();
                    lastSelected = null;
                }
            }
        }
		#endregion
#region SELECTION
		if ((pstate == PlayState.play || pstate == PlayState.pause) && MainEditorComponent.editable)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Stop();
                
			}
		}

        if (pstate == PlayState.stop)
        {
            if (Physics.Raycast(new Vector3(mousePos.x, mousePos.y, -27), transform.forward, Mathf.Infinity, env))
            {
                foreach (GameObject wall in walls)
                {
                    wall.GetComponent<Renderer>().material = transparent;
                }
            }
            else
            {
                foreach (GameObject wall in walls)
                {
                    wall.GetComponent<Renderer>().material = border;
                }
            }
        }

		if (!cantContinue && pstate == PlayState.stop)
        {

            

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.A) && pstate == PlayState.stop && selectedButton != 10 && !cantContinue)
            {
                Deselect();
                DeselectPortal();
                for (int i = 1; i < mainEditorComponent.levelObjects.Length; i++)
                {
                    Select(mainEditorComponent.levelObjects[i], true);
                }
            }
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.D) && selectedObject && !cantContinue) Deselect();
            if (selectedButton == 0 && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;



                if (Physics.Raycast(new Vector3(mousePos.x, mousePos.y, -27), transform.forward, out hit, Mathf.Infinity, selectIgnore))
                {

                if (Array.IndexOf(lastSelectedArray, hit.transform.root.gameObject) == -1)
                {
                    if (!Input.GetKey(KeyCode.LeftControl))
                    {
                        Deselect();
                            DeselectPortal();
                        Select(hit.transform.root.gameObject, false);
                    }
                    else Select(hit.transform.root.gameObject, true);
                    return;
                }
                else
                    {
                        List<GameObject> t = lastSelectedArray.ToList();
                        t.Remove(hit.transform.root.gameObject);
                        Deselect();
                        DeselectPortal();
                        for (int i = 0; i < t.Count; i++)
                        {
                            Select(t[i], true);
                        }

                    }
                    
                }
                else if (Physics.Raycast(new Vector3(mousePos.x, mousePos.y, -27), transform.forward, out hit, Mathf.Infinity) && hit.transform.root.gameObject.CompareTag("portal"))
                {
                    SelectPortal(hit.transform.root.gameObject);
                }
                else
                {
                    Deselect();
                    DeselectPortal();
                }
            }

            #region CAMERA CONTROLS

        }
        if (!cantContinue)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                UIType = uType.pos;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                UIType = uType.rot;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                UIType = uType.scale;
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                UIType = uType.none;
            }
        }
            

        if (selectedObject)
        {


            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl) && !cantContinue)
            {
                clipboard = new GameObject[lastSelectedArray.Length];
                clipboard = lastSelectedArray;
            }
            if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl) && !cantContinue)
            {
                Duplicate(clipboard);
            }
            SetUI(true);
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete(lastSelectedArray);
            }
        } else SetUI(false);
#endregion

        #region TOGGLE BUTTON WITH NUMBERS
        //if (pstate == PlayState.stop && canNumbers)
        //{
        //    try
        //    {
        //        ToggleButton(int.Parse(Input.inputString) - 1);
        //    }
        //    catch { }
        //
        //    //switch (Input.inputString)
        //    //{
        //    //    case "1":
        //    //        ToggleButton(0);
        //    //        if (selectedButton != 0) 
        //    //            Deselect();
        //    //        break;
        //    //    case "2":
        //    //        ToggleButton(1);
        //    //        Deselect();
        //    //        break;
        //    //    case "3":
        //    //        ToggleButton(2);
        //    //        Deselect();
        //    //        break;
        //    //    case "4":
        //    //        ToggleButton(3);
        //    //        Deselect();
        //    //        break;
        //    //    case "5":
        //    //        ToggleButton(4);
        //    //        Deselect();
        //    //        break;
        //    //    case "6":
        //    //        ToggleButton(5);
        //    //        Deselect();
        //    //        break;
        //    //    case "7":
        //    //        ToggleButton(6);
        //    //        Deselect();
        //    //        break;
        //    //    case "8":
        //    //        ToggleButton(7);
        //    //        Deselect();
        //    //        break;
        //    //    case "9":
        //    //        ToggleButton(8);
        //    //        Deselect();
        //    //        break;
        //    //}
        //}
        #endregion

        if (pstate == PlayState.stop)
        {
            if (Input.GetMouseButton(2))
            {
                Vector3 pos;
                pos.x = Mathf.Clamp(mainCam.transform.position.x - (Input.GetAxisRaw("Mouse X") / 1.5f), -86.1f, 86.1f);
                pos.y = Mathf.Clamp(mainCam.transform.position.y - (Input.GetAxisRaw("Mouse Y") / 1.5f), -78, 78);
                pos.z = -32;
                mainCam.transform.position = pos;
            }
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
            {
                mainCam.fieldOfView = 36;
                mainCam.transform.position = new Vector3(0, 0, -32);
            }
            if (!cantScroll && !Input.GetKey(KeyCode.LeftControl))
                mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView += -(Input.GetAxisRaw("Mouse ScrollWheel")) * 15, 15, 120);
        }
    }

    public void SetCantScroll(bool c) => cantScroll = c;

    public void SelectPortal(GameObject go)
    {
        if (!MainEditorComponent.editable) return;
        selectedPortal = go;

        portalPositionUI.transform.position = selectedPortal.transform.position;
        portalPositionUI.SetActive(true);

        ChangeChildrenMaterials(go.transform.root.gameObject, selectedMat, true, false, false);

        Deselect();
    }

    public void DeselectPortal()
    {
        if (selectedPortal == null) return;

        portalPositionUI.SetActive(false);
        selectedPortal.transform.gameObject.GetComponent<ObjectComponent>().ResetMaterials();
        selectedPortal = null;
    }

    public void Duplicate(GameObject[] clip)
    {
        if (!MainEditorComponent.editable) return;
        if (clip.Length == 0) return;
       
        List<GameObject> newClip = new List<GameObject>();
        
        int i = 0;
        
        List<int> skipIds = new List<int>();
        foreach (GameObject go in clip)
        {
            if (go.TryGetComponent(out TeleporterComponentScript tpc) && spawnPairObjects)
            {
                if (skipIds.Contains(tpc.RecieverID)) continue;
            }
            int index = mainEditorComponent.blackHoles.Count / 2;
            mainEditorComponent.CreateObject((int)go.GetComponent<ObjectComponent>().obj, go.transform.position.x + 1f, 
                go.transform.position.y + 1f, go.transform.localEulerAngles.z, go.transform.localScale.x, 
                go.transform.localScale.y, true, go.GetComponent<ObjectComponent>().misc);
            GameObject duplicated = mainEditorComponent.levelObjects[mainEditorComponent.levelObjects.Length - 1];
            newClip.Add(duplicated);
            if (spawnPairObjects && duplicated.TryGetComponent(out TeleporterComponentScript t)) t.RecieverID = index;
            
            if (go.TryGetComponent(out TeleporterComponentScript tp) && spawnPairObjects)
            {
                
                skipIds.Add(tp.RecieverID);
                mainEditorComponent.CreateObject((int)go.GetComponent<ObjectComponent>().obj, go.transform.position.x + 5f, 
                    go.transform.position.y + 1f, go.transform.localEulerAngles.z, go.transform.localScale.x, 
                    go.transform.localScale.y, true, go.GetComponent<ObjectComponent>().misc);
                GameObject newObject = mainEditorComponent.levelObjects[mainEditorComponent.levelObjects.Length - 1];
                newObject.GetComponent<TeleporterComponentScript>().WorkAsReciever = true;
                newObject.GetComponent<TeleporterComponentScript>().RecieverID = index;
                newClip.Add(newObject);
            }
            
            i++;
            
        }
        Deselect();
        DeselectPortal();
        if (clip.Length == 1) Select(newClip[0], false);
        
        foreach (GameObject go in newClip)
        {
            Select(go, true);
        }
    }

    private Vector3 GetAveragePosition(GameObject[] positions)
    {
        Vector3[] VectorArray = new Vector3[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            VectorArray[i] = positions[i].transform.position;
        }
        Vector3 meanVector = Vector3.zero;
        foreach (Vector3 pos in VectorArray)
        {
            meanVector += pos;
        }
        return (VectorArray.Length == 0 ? meanVector : meanVector / VectorArray.Length);
    }

    public void Delete(GameObject[] g, bool rec = false)
    {
        if (!MainEditorComponent.editable) return;
        cantContinue = false;
        DeselectPortal();
        Deselect();
        SetUI(false);

        for (int i = 0; i < g.Length; i++)
        {
            if (g[i].GetComponent<ObjectComponent>().obj == ObjectComponent.ObjectType.BlackHole && spawnPairObjects)
            {
                GameObject[] objects = mainEditorComponent.blackHoles.Where(x => x.GetComponent<TeleporterComponentScript>().RecieverID == g[i].GetComponent<TeleporterComponentScript>().RecieverID && x != g[i]).ToArray();
                if (objects.Length > 0 && Array.IndexOf(g, objects[0]) == -1) Delete(objects[0]);
            }
            Delete(g[i]);
        }
        Deselect();
    }
    public void Delete(GameObject j)
    {
        if (!MainEditorComponent.editable) return;
        GameObject[] g = new GameObject[1];
        g[0] = j;
        cantContinue = false;
        DeselectPortal();
        Deselect();
        SetUI(false);
        int[] s = new int[g.Length];
        for (int i = 0; i < s.Length; i++)
        {
            s[i] = Array.IndexOf(mainEditorComponent.levelObjects, g[i]);
            if (g[i].GetComponent<ObjectComponent>().obj == ObjectComponent.ObjectType.button)
            {
                mainEditorComponent.buttons[Array.IndexOf(mainEditorComponent.buttons, g[i])] = mainEditorComponent.buttons[mainEditorComponent.buttons.Length - 1];
                mainEditorComponent.buttons[mainEditorComponent.buttons.Length - 1] = null;
                Array.Resize(ref mainEditorComponent.buttons, mainEditorComponent.buttons.Length - 1);

                if (mainEditorComponent.buttons.Length < 1)
                {
                    mainEditorComponent.portalout.GetComponent<Portal>().isOpen = true;
                    mainEditorComponent.portalout.GetComponent<MeshRenderer>().material = mainEditorComponent.portalout.GetComponent<Portal>().door_enabled;
                    mainEditorComponent.portalout.GetComponent<Portal>().door_particles.material = mainEditorComponent.portalout.GetComponent<Portal>().button_enabled;
                }
            }

            if (g[0].GetComponent<ObjectComponent>().obj == ObjectComponent.ObjectType.BlackHole)
                mainEditorComponent.blackHoles.RemoveAt(mainEditorComponent.blackHoles.IndexOf(g[0]));
            
            if (g[0].GetComponent<ObjectComponent>().obj == ObjectComponent.ObjectType.Checkpoint)
                mainEditorComponent.checkpoints.RemoveAt(mainEditorComponent.checkpoints.IndexOf(g[0].GetComponent<Checkpoint>()));
            
            Destroy(g[i]);
            mainEditorComponent.levelObjects[s[i]] =
            mainEditorComponent.levelObjects[mainEditorComponent.levelObjects.Length - i - 1];
            mainEditorComponent.levelObjects[mainEditorComponent.levelObjects.Length - i - 1] = null;
        }


        foreach (GameObject o in lastSelectedArray)
        {
            //Debug.Log("AAAAAAAAAAA");
            o.transform.root.gameObject.GetComponent<ObjectComponent>().ResetMaterials();
        }

        Array.Resize(ref lastSelectedArray, 0);
        Array.Resize(ref mainEditorComponent.levelObjects, mainEditorComponent.levelObjects.Length - g.Length);
        foreach (GameObject o in lastSelectedArray)
        {
            Destroy(o);
        }
        Deselect();
    }
    public GameObject properties;
    public void Select(GameObject g, bool m)
    {
        if (!MainEditorComponent.editable) return;
        if (pstate != PlayState.stop || Array.IndexOf(lastSelectedArray, g) != -1) return;
        selectedObject = true;
        if (g.TryGetComponent(out ColorComponent cc))
        {
            cc.Disable();
        }
        if (!m)
        {
            if (lastSelectedArray.Length > 0)
            {
                foreach (GameObject o in lastSelectedArray)
                {
                    //Debug.Log("AAAAAAAAAAA");
                    o.transform.root.gameObject.GetComponent<ObjectComponent>().ResetMaterials();
                }
                Array.Resize(ref lastSelectedArray, 0);
            }

        }
        Array.Resize(ref lastSelectedArray, lastSelectedArray.Length + 1);
        lastSelectedArray[lastSelectedArray.Length - 1] = g.transform.root.gameObject;
        if (g.transform.root.TryGetComponent(out TMP_Text tmp))
        {
            ChangeChildrenMaterials(g.transform.root.gameObject, textSelectedMat, true, false, false);
        }
        else ChangeChildrenMaterials(g.transform.root.gameObject, selectedMat, true, false, false);
        objProperties.nuhUh = true;
        properties.SetActive(true);
        objProperties.InitializeFields(lastSelectedArray);
        objProperties.nuhUh = false;
        var movingS = g.GetComponent<Moving>();
        if (movingS != null)
        {
            movingS.canEnableLineRenderer = true;
        }

        var bhS = g.GetComponent<TeleporterComponentScript>();
        if (bhS != null)
        {
            bhS.canEnableLineRenderer = true;
        }
        return;
    }

    public void Deselect()
    {
        if (!MainEditorComponent.editable) return;
        if (cantContinue) return;
        properties.gameObject.SetActive(false);
        cycleMenu.gameObject.SetActive(false);
        if (lastSelectedArray.Length > 0)
        {
            foreach (GameObject o in lastSelectedArray)
            {
                if (o.TryGetComponent(out ColorComponent cc))
                {
                    cc.Enable();
                }
                //Debug.Log("AAAAAAAAAAA"); 
                o.transform.gameObject.GetComponent<ObjectComponent>().ResetMaterials();

                var movingS = o.GetComponent<Moving>();
                if (movingS != null)
                {
                    movingS.canEnableLineRenderer = false;
                }

                var bhS = o.GetComponent<TeleporterComponentScript>();
                if (bhS != null)
                {
                    bhS.canEnableLineRenderer = false;
                }
            }
            selectedObject = false;
            Array.Resize(ref lastSelectedArray, 0);
        }
        SetUI(false);
    }

    public Sprite[] activatedModes, deactivatedModes;
    public Image posSprite, scaleSprite, rotSprite;

    public void SetMode(int u)
    {
        if (u == 0 && !pos.isOn) return;
        if (u == 1 && !rot.isOn) return;
        if (u == 2 && !scale.isOn) return;
        UIType = (uType)u;
    }
    
    public void SetUI(bool enable)
    {
        if (!MainEditorComponent.editable) return;
        positionUI.transform.position = GetAveragePosition(lastSelectedArray);
        scaleUI.transform.position = GetAveragePosition(lastSelectedArray);
        rotationUI.transform.position = GetAveragePosition(lastSelectedArray);
        if (lastSelectedArray.Length > 0)
        {
            scaleUI.transform.rotation = lastSelectedArray[lastSelectedArray.Length - 1].transform.rotation;
        }
        if (!enable)
        {
            positionUI.SetActive(false);
            scaleUI.SetActive(false);
            rotationUI.SetActive(false);
        }

        switch (UIType)
        {
            case uType.pos:
                positionUI.SetActive(enable);
                scaleUI.SetActive(false);
                rotationUI.SetActive(false);
                cantContinue = true;
                pos.isOn = true;
                scale.isOn = false;
                rot.isOn = false;
                cantContinue = false;
                posSprite.sprite = activatedModes[0];
                scaleSprite.sprite = deactivatedModes[1];
                rotSprite.sprite = deactivatedModes[2];
                break;
            case uType.rot:    
               
                positionUI.SetActive(false);
                scaleUI.SetActive(false);
                rotationUI.SetActive(enable);
                pos.isOn = false;
                scale.isOn = false;
                rot.isOn = true;
                posSprite.sprite = deactivatedModes[0];
                scaleSprite.sprite = deactivatedModes[1];
                rotSprite.sprite = activatedModes[2];
                break;

            case uType.scale:
                positionUI.SetActive(false);
                scaleUI.SetActive(enable);
                rotationUI.SetActive(false);
                pos.isOn = false;
                scale.isOn = true;
                rot.isOn = false;
                posSprite.sprite = deactivatedModes[0];
                scaleSprite.sprite = activatedModes[1];
                rotSprite.sprite = deactivatedModes[2];
                break;

            case uType.none:
                positionUI.SetActive(false);
                scaleUI.SetActive(false);
                rotationUI.SetActive(false);
                pos.isOn = false;
                scale.isOn = false;
                rot.isOn = false;
                posSprite.sprite = deactivatedModes[0];
                scaleSprite.sprite = deactivatedModes[1];
                rotSprite.sprite = deactivatedModes[2];
                break;
        }
    }
    public void ChangeChildrenMaterials(GameObject g, Material mat, bool changeParent, bool visibility, bool visibilityf)
    {
        MeshRenderer[] mrenderer = g.GetComponentsInChildren<MeshRenderer>();
        if (mrenderer != null && mrenderer.Length >= 1)
        {
            for (int i = 0; i < mrenderer.Length; i++)
            {
                mrenderer[i].material = mat;
                if (visibility)
                {
                    mrenderer[i].enabled = visibilityf;
                }
            }
        }
        if (changeParent && g != null && g.GetComponent<MeshRenderer>() != null)
        {

            g.GetComponent<MeshRenderer>().material = mat;
            if (visibility)
            {
                g.GetComponent<MeshRenderer>().enabled = visibilityf;
            }
        }
    }
    public void ToggleButton(int buttonid)
    {
        if (!MainEditorComponent.editable) return;
        if (selectedButton == buttonid) 
            return;
        selectedButton = buttonid;

        for (int i = 0; i < buttons.Length; i++)
        {
            //if (i == buttonid) continue;
            buttons[i].isOn = false;
            buttonsimg[i].sprite = deselectedSprites[i];
        }
        buttonsimg[buttonid].sprite = selectedSprites[buttonid];
        Destroy(placeObject);
        
        if (buttonid == 0 || buttonid == 10) placeObject = Instantiate(empty);
        else placeObject = Instantiate(mainEditorComponent.placeableObjects[buttonid - 1]);
        SetUI(false);
        if (buttonid != 10)
        ChangeChildrenMaterials(placeObject, selectedMat, true, false, false);
        if (buttonid == 0) cantContinue = false;
        else
        {
            cantContinue = false;
            Deselect();
            cantContinue = true;
        }
        placeObject.layer = LayerMask.NameToLayer("place");
        for (int i = 0; i < placeObject.GetComponentsInChildren<Collider>().Length; i++)
        {
            placeObject.GetComponentsInChildren<Collider>()[i].gameObject.layer = 9;
        }
        
    }
    public GameObject bottomWall;
    public void Play()
    {
        
        cantContinue = false;
        Deselect();
        if (!playButtons[0].isOn)
        {
            firstPause = true;
            foreach (GameObject wall in walls)
            {
                wall.GetComponent<Renderer>().material = border;
            }
            bottomWall.layer = 3;
            canObjectBePlaced = false;
            playImg[0].sprite = playSprites[1];
            playButtons[1].interactable = true;
            mainEditorComponent.Play();
            pstate = PlayState.play;
            PlayCanvas.SetActive(true);
            BuildCanvas.SetActive(false);
            cine.SetActive(mainEditorComponent.cineEnabled);
            placeObject.SetActive(false);
            PauseTimer = false;
            camFOV = mainCam.fieldOfView;
            camPos = mainCam.transform.position;
            if (!mainEditorComponent.cineEnabled)
            { 
                mainCam.gameObject.transform.position = new Vector3(0, 0, -32);
                mainCam.fieldOfView = mainEditorComponent.fov;
            }
            if (!MainEditorComponent.isOfficial) music.Pause();
            
        }
        if (playButtons[0].isOn)
        {
            bottomWall.layer = 10;
            Stop();
            music.Resume();
        }

    }

    public GameObject cameraShaker;
    public void Stop()
    {
        if (!MainEditorComponent.editable) return;
        if (pstate == PlayState.stop) return;
        cameraShaker.transform.eulerAngles = Vector3.zero;
        canObjectBePlaced = true;
        placeObject.SetActive(true);
        cine.SetActive(false);
        mainCam.gameObject.transform.position = camPos;
        mainCam.fieldOfView = camFOV;
        playImg[0].sprite = playSprites[0];
        playButtons[1].isOn = false;
        playImg[1].sprite = playSprites[2];
        playButtons[1].interactable = false;
        mainEditorComponent.Stop();
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
        foreach (GameObject go in gos)
        {
            if (go.layer == 8)
            {
                Destroy(go);
            }
        }
        if (mainEditorComponent.buttons.Length >= 1)
        {
            mainEditorComponent.leavePortal.GetComponent<Portal>().isOpen = false;
            mainEditorComponent.leavePortal.GetComponent<MeshRenderer>().material = red;
            mainEditorComponent.leavePortal.GetComponentInChildren<ParticleSystemRenderer>().material = red;
        }
        foreach (GameObject b in mainEditorComponent.buttons)
        {
            b.transform.root.GetComponentInChildren<MeshRenderer>().material = red;
        }
        
        pstate = PlayState.stop;
        PlayCanvas.SetActive(false);
        BuildCanvas.SetActive(true);
    }

    private bool firstPause = true;
    public void Pause()
    {
        if (!playButtons[1].isOn || firstPause)
        {
            firstPause = false;
            Time.timeScale = 0;
            playImg[1].sprite = playSprites[3];
            mainEditorComponent.Pause(true);

            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
            foreach (GameObject go in gos)
            {
                if (go.layer == 8)
                {
                    go.GetComponent<Bullet>().Pause();

                }
            }
            pstate = PlayState.pause;
            PauseTimer = true;
        }
        else if (playButtons[1].isOn)
        {
            Time.timeScale = 1;
            playImg[1].sprite = playSprites[2];
            mainEditorComponent.Pause(false);
            GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
            foreach (GameObject go in gos)
            {
                if (go.layer == 8)
                {
                    go.GetComponent<Bullet>().Resume();

                }
            }
            pstate = PlayState.play;
            PauseTimer = false;
        }

    }
}
#endregion