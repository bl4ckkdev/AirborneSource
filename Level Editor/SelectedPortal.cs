// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// I LOVE THIS CODE :D

public class SelectedPortal : MonoBehaviour
{
    public EditorControls editorcontrols;
    public MainEditorComponent mainEditor;

    float add;

    private void OnMouseDown()
    {
        editorcontrols.cantContinue = true;
    }

    public void OnMouseDrag()
    {
        float y = Input.GetAxisRaw("Mouse Y") / 1.5f;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            add += Input.GetAxisRaw("Mouse Y") / 2;
            y = (add >= 1) ? 1 : (add <= -1) ? -1 : 0;
            add = (Mathf.Abs(add) >= 1) ? 0 : add;
        }

        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0.5f);

        editorcontrols.selectedPortal.transform.position += new Vector3(0, y, 0);
        transform.parent.position += new Vector3(0, y, 0);

        if (editorcontrols.selectedPortal.name == "PortalIn")
        {
            mainEditor.player.transform.localPosition += new Vector3(0, y, 0);
        }
    }

    public void OnMouseUp()
    {
        editorcontrols.cantContinue = false;
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1f);

        //mainEditor.Save();
    }
}
