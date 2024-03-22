// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cycle : MonoBehaviour
{
    public int cycleNumber;
    public TMP_InputField to_x, to_y, speedortime, offset;
    public TMP_Text cycleNumberText, speedortimetext;
    public TMP_Dropdown ease;

    private void Update()
    {
        cycleNumberText.text = cycleNumber.ToString();
    }

    public void Delete()
    {
        SetupCycle.instance.RemoveCycle(gameObject);
    }

    public void MoveUp()
    {
        SetupCycle.instance.MoveUp(gameObject);
    }

    public void MoveDown()
    {
        SetupCycle.instance.MoveDown(gameObject);
    }

    public void UpdateEditorFields()
    {
        SetupCycle.instance.UpdateFields();
    }
}
