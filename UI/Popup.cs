// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public TextMeshProUGUI ptext;
    public string contents;

    private void Start()
    {
        ptext.text = contents;
    }
}
