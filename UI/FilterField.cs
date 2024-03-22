// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FilterField : MonoBehaviour
{
    public string[] stuffToFilter;

    public void Filter(TMP_InputField field)
    {
        for (int i = 0; i < stuffToFilter.Length; i++)
        {
            field.text = field.text.Replace(stuffToFilter[i], string.Empty);
        }
    }
}
