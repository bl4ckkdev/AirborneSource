// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    ObjectComponent obj;
    public bool isEnabled;

    public bool offOnRestart;
    public bool turnBackOff;
    public bool immediatelyActivate;
    [Space]
    public bool finale;
    public Material blue;
    private protected bool cd;

    public Renderer smallBit;

    IEnumerator Cooldown()
    {
        cd = true;
        yield return new WaitForSeconds(0.5f);
        cd = false;
    }


    private void Start()
    {
        if (!finale) obj = GetComponentInParent<ObjectComponent>();
        if (offOnRestart) smallBit.gameObject.SetActive(true);
    }
    public void Activate()
    {
        print("hi :)");
        if (cd) return;
        
        StartCoroutine(Cooldown());
        if (!finale) transform.root.gameObject.GetComponentInChildren<MeshRenderer>().material = obj.portalout.GetComponent<Portal>().button_enabled;
        else transform.parent.gameObject.GetComponentInChildren<MeshRenderer>().material = blue;
        if (!isEnabled && !finale) obj.portalout.GetComponent<Portal>().audios.Play();
        if (offOnRestart)
        {
            if (!isEnabled) smallBit.material = obj.portalout.GetComponent<Portal>().button_enabled;
            else smallBit.material = obj.portalout.GetComponent<Portal>().button_disabled;
        }

        if (finale) Bossfight.Instance.StopAttacks();
        if (finale) return;
        isEnabled = true;
        bool t = true;
        
        if (!finale)
        for (int i = 0; i < obj.editorControls.mainEditorComponent.buttons.Length; i++)
        {
            if (!obj.editorControls.mainEditorComponent.buttons[i].GetComponentInChildren<Button>().isEnabled)
            {
                t = false;
                break;
            }
        }
        
        if (!immediatelyActivate)
        {
            if (t)
            {
                obj.portalout.GetComponent<Portal>().isOpen = true;
                obj.portalout.GetComponent<MeshRenderer>().material = obj.portalout.GetComponent<Portal>().door_enabled;
                obj.portalout.GetComponent<Portal>().door_particles.material = obj.portalout.GetComponent<Portal>().button_enabled;
            }
        }
        else
        {
            obj.portalout.GetComponent<Portal>().isOpen = true;
            obj.portalout.GetComponent<MeshRenderer>().material = obj.portalout.GetComponent<Portal>().door_enabled;
            obj.portalout.GetComponent<Portal>().door_particles.material = obj.portalout.GetComponent<Portal>().button_enabled;
        }
    }

    public void Deactivate()
    {
        if (cd) return;
        StartCoroutine(Cooldown());
        transform.root.gameObject.GetComponentInChildren<MeshRenderer>().material = obj.portalout.GetComponent<Portal>().button_disabled;
        obj.portalout.GetComponent<Portal>().audios.Play();
        isEnabled = false;
        if (offOnRestart)
        {
            if (isEnabled) smallBit.material = obj.portalout.GetComponent<Portal>().button_enabled;
            else smallBit.material = obj.portalout.GetComponent<Portal>().button_disabled;
        }

        bool canDisable = true;
        foreach (GameObject i in obj.mainEditorComponent.buttons)
        {
            if (i.transform.GetChild(1).GetComponent<Button>().immediatelyActivate && i.transform.GetChild(1).GetInstanceID() != GetInstanceID() && i.transform.GetChild(1).GetComponent<Button>().isEnabled)
            {
                canDisable = false;
                break;
            }
        }
        if (canDisable)
        {
            obj.portalout.GetComponent<Portal>().isOpen = false;
            obj.portalout.GetComponent<MeshRenderer>().material = obj.portalout.GetComponent<Portal>().door_disabled;
            obj.portalout.GetComponent<Portal>().door_particles.material = obj.portalout.GetComponent<Portal>().button_disabled;
        }

    }
}
