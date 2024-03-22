// Copyright © bl4ck & XDev, 2022-2024
using System;
using TMPro;
using UnityEngine;

public class SpeedrunSettings : MonoBehaviour
{
    public TMP_InputField start, end;

    private void Start()
    {
        start.text = PlayerPrefs.GetInt("SpeedrunStart", 1).ToString();
        end.text = PlayerPrefs.GetInt("SpeedrunEnd", 1).ToString();
    }

    public void SetStart(string set)
    {
        PlayerPrefs.SetInt("SpeedrunStart", int.Parse(set));
    }
    
    public void SetEnd(string set)
    {
        PlayerPrefs.SetInt("SpeedrunEnd", int.Parse(set));
    }
}
