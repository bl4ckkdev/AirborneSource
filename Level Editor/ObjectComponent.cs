// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything

using DG.Tweening;
using UnityEngine;

public class ObjectComponent : MonoBehaviour
{
    public GameObject     player;
    public GameObject  portalout;
    public GameObject    spawner;
    public GameObject   portalin;

    public MainEditorComponent mainEditorComponent;
    public EditorControls     editorControls;

    public Vector3       originalPos;
    public Quaternion    originalRot;
    public Vector3     originalScale;
    public Material originalMaterial;

    public Vector3 originalVelocity;

    public string misc;
    public string[] miscSplit;


    [SerializeField] public Material[] savedMaterial;
    [SerializeField] Material baseMaterial;

    public enum ObjectType
    {
        player = 11,
        platform = 1,
        portal = 10,
        turret = 4,
        launchPad = 5,
        spike = 2,
        button = 3,
        laser = 6,
        text = 7,
        BlackHole = 8,
        Checkpoint = 9
    }

    public ObjectType obj;
    public bool finale;

    private void Awake()
    {
        if (!finale)
        {
            mainEditorComponent = GameObject.FindGameObjectWithTag("Editor Component").GetComponent<MainEditorComponent>();
            player = mainEditorComponent.player;
            portalout = mainEditorComponent.portalout;
            spawner = mainEditorComponent.spawner;
            portalin = mainEditorComponent.portalin;
            editorControls = mainEditorComponent.editorControls;

            SaveMaterials();

            originalPos = transform.position;
            originalRot = transform.rotation;
            originalScale = transform.lossyScale;

            if (player.GetComponent<Die>().isEditor) Stop();
            

        }
        else player = Bossfight.Instance.player;


    }
    
    public void SaveMaterials()
    {
        MeshRenderer[] mrenderer = GetComponentsInChildren<MeshRenderer>();
        if (GetComponent<MeshRenderer>() != null)
        {
            baseMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        }
        if (mrenderer != null)
        {
            savedMaterial = new Material[mrenderer.Length];
            for (int i = 0; i < mrenderer.Length; i++)
            {
                savedMaterial[i] = mrenderer[i].sharedMaterial;
            }
        }

    }

    public void ResetMaterials()
    {
        MeshRenderer[] mrenderer = GetComponentsInChildren<MeshRenderer>();
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().sharedMaterial = baseMaterial;
        }
        if (mrenderer != null)
        {
            for (int i = 0; i < mrenderer.Length; i++)
            {
                try
                {
                    mrenderer[i].sharedMaterial = savedMaterial[i];
                }
                catch
                {
                    mrenderer[i].sharedMaterial = portalout.GetComponent<Portal>().button_disabled;
                }
            }
        }
    }

    public void Stop()
    {
        if (TryGetComponent(out Moving m) && m.nextPoint != null)
        {
            m.cont = true;
            m.StopCoroutine(m.nextPoint);
            m.currentPoint = 1;
            m.transform.DOKill();
        }
        
        transform.SetPositionAndRotation(originalPos, originalRot);
        transform.localScale = originalScale;


        if (TryGetComponent(out goddamnit.Physics phy))
        {
            phy.Init();
            phy.rigid.isKinematic = true;
        }

        switch (obj)
        {
            case ObjectType.player:
                GetComponent<PlayerMovement>().enabled = false;
                GetComponent<Rigidbody>().detectCollisions = false;
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Die>().enabled = false;
                //GetComponent<BoxCollider>().enabled = false;
                GetComponent<TrailRenderer>().enabled = false;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
                transform.GetChild(1).localScale = Vector3.one;
                transform.GetChild(1).GetComponent<MeshRenderer>().material = mainEditorComponent.playerMats[1];
                break;

            case ObjectType.platform:
                //GetComponent<BoxCollider>().enabled = false;
                break;

            case ObjectType.portal:
                // todo
                break;

            case ObjectType.turret:
                //for (int i = 0; i > GetComponentsInChildren<BoxCollider>().Length; i++)
                //    GetComponentsInChildren<BoxCollider>()[i].enabled = false;
                Turret t = GetComponent<Turret>();
                if (t._cooldown != null) t.StopCoroutine(t._cooldown);
                if (t._offset != null) t.StopCoroutine(t._offset);
                t.freeze = true;
                t.DOKill();
                t.tipMat.SetColor("_EmissionColor", t.origin);
                

                break;

            case ObjectType.launchPad:
                break;

            case ObjectType.spike:
                //GetComponentInChildren<MeshCollider>().enabled = false;

                break;

            case ObjectType.button:
                GetComponent<Button>().isEnabled = false;
                ResetMaterials();
                break;

            case ObjectType.laser:
                // todo
                break;

            case ObjectType.text:
                GetComponent<BoxCollider>().enabled = true;
                break;

            case ObjectType.BlackHole:
                GetComponent<TeleporterComponentScript>().Update();
                break;
            case ObjectType.Checkpoint:
                ResetMaterials();
                mainEditorComponent.activeCheckpoint = null;
                break;
        }
    }

    public void Play()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
        originalScale = transform.lossyScale;
        if (TryGetComponent(out goddamnit.Physics phy))
        {
            if (phy.isEnabled) phy.rigid.isKinematic = false;
            phy.Init();
        }
        
        if (TryGetComponent(out Moving m))
        {
            if (m.stop != null) m.StopCoroutine(m.stop);
            m.movingToB = true;
            m.cont = false;
            m.from = transform.position;
            if (m.instantlyStart)
            m.OnStart();
        }
        switch (obj)
        {
            case ObjectType.player:
                GetComponent<PlayerMovement>().enabled = true;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().detectCollisions = true;
                GetComponent<Die>().enabled = true;
                GetComponent<Die>().savedButtons.Clear();
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<TrailRenderer>().enabled = true;
                transform.GetChild(1).GetComponent<Renderer>().material = mainEditorComponent.playerMats[0];
                break;
            
            case ObjectType.platform:
                GetComponent<BoxCollider>().enabled = true;
                break;

            case ObjectType.portal:
                // todo
                break;

            case ObjectType.turret:
                for (int i = 0; i > GetComponentsInChildren<BoxCollider>().Length; i++)
                    GetComponentsInChildren<BoxCollider>()[i].enabled = true;
                
                Turret t = GetComponent<Turret>();
                if (t._cooldown != null) t.StopCoroutine(t._cooldown);
                if (t._offset != null) t.StopCoroutine(t._offset);
                t.freeze = false;
                t.DOKill();
                t.tipMat.SetColor("_EmissionColor", t.origin);
                t.Start();
                

                break;

            case ObjectType.launchPad:
                break;

            case ObjectType.spike:
                GetComponentInChildren<MeshCollider>().enabled = true;

                break;

            case ObjectType.button:
                // todo
                SaveMaterials();
                break;

            case ObjectType.laser:
                // todo
                break;

            case ObjectType.text:
                GetComponent<BoxCollider>().enabled = false;
                break;

            case ObjectType.BlackHole:
                break;
            case ObjectType.Checkpoint:
                Checkpoint ch = GetComponent<Checkpoint>();
                if (ch.isEnabled && ch.startPos && MainEditorComponent.Instance.activeCheckpoint == null || ch.isEnabled && ch.startPos && ch.priority >= MainEditorComponent.Instance.activeCheckpoint.priority)
                {
                    mainEditorComponent.activeCheckpoint = ch;
                }
                break;


        }
        transform.SetPositionAndRotation(originalPos, originalRot);
        if (obj == ObjectType.player && MainEditorComponent.Instance.activeCheckpoint != null) transform.position = mainEditorComponent.activeCheckpoint.transform.position;
        transform.localScale = originalScale;
    }

    public void Unpause()
    {
        if (TryGetComponent(out goddamnit.Physics phy))
        {
            if (phy.isEnabled) phy.rigid.isKinematic = false;
        }
        switch (obj)
        {
            case ObjectType.player:
                GetComponent<PlayerMovement>().enabled = true;
                GetComponent<Rigidbody>().detectCollisions = true;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().velocity = originalVelocity; 
                GetComponent<Die>().enabled = true;
                GetComponent<BoxCollider>().enabled = true;
                GetComponent<TrailRenderer>().enabled = true;
                
                break;

            case ObjectType.platform:
                GetComponent<BoxCollider>().enabled = true;
                break;

            case ObjectType.portal:
                // todo
                break;

            case ObjectType.turret:
                for (int i = 0; i > GetComponentsInChildren<BoxCollider>().Length; i++)
                    GetComponentsInChildren<BoxCollider>()[i].enabled = true;
                GetComponent<Turret>().freeze = false;
                GetComponent<Turret>().DOPlay();
                break;

            case ObjectType.launchPad:
                break;

            case ObjectType.spike:
                GetComponentInChildren<MeshCollider>().enabled = true;

                break;

            case ObjectType.button:
                // todo

                break;

            case ObjectType.laser:
                // todo
                break;

			case ObjectType.text:
				break;

            case ObjectType.BlackHole:
                break;

		}
    }

    public void Pause()
    {
        if (TryGetComponent(out goddamnit.Physics phy))
        {
             phy.rigid.isKinematic = true;
        }
        switch (obj)
        {
            case ObjectType.player:
                GetComponent<PlayerMovement>().enabled = false;
                GetComponent<Rigidbody>().detectCollisions = false;
                GetComponent<Rigidbody>().useGravity = false;
                originalVelocity = GetComponent<Rigidbody>().velocity;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
                GetComponent<Die>().enabled = false;
                //GetComponent<BoxCollider>().enabled = false;
                GetComponent<TrailRenderer>().enabled = false;
                break;

            case ObjectType.platform:
                //GetComponent<BoxCollider>().enabled = false;
                break;

            case ObjectType.portal:
                // todo
                break;

            case ObjectType.turret:
                //for (int i = 0; i > GetComponentsInChildren<BoxCollider>().Length; i++)
                //    GetComponentsInChildren<BoxCollider>()[i].enabled = false;
                GetComponent<Turret>().freeze = true;
                GetComponent<Turret>().DOPause();
                break;

            case ObjectType.launchPad:
                break;

            case ObjectType.spike:
                //GetComponentInChildren<MeshCollider>().enabled = false;

                break;

            case ObjectType.button:
                // todo

                break;

            case ObjectType.laser:
                // todo
                break;

			case ObjectType.text:
				break;

            case ObjectType.BlackHole:
                break;

        }
    }
}
