// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ProfanityFilter : MonoBehaviour
{
    public TMP_InputField inputField;
    public string[] swears = {   // PLEASE KILL ME
        "cock",
        "dick",
        "anal",
        "dickhead",
        "cockhead",
        "cocklicker",
        "cum",
        "cumming",
        "cumslut",
        "slut",
        "horny",
        "pussy",
        "anus",
        "ass",
        "tit",
        "tits",
        "buttplug",
        "dildo",
        "niga",
        "nigga",
        "nigger",
        "penis",
        "porn",
        "rape",
    };
    public bool enableProfanity = false;

    private void Start()
    {
        inputField.onValueChanged.AddListener(Filter);
    }

    public void Filter(string value)
    {
        string filteredText = value;
        if (!enableProfanity)
        {
            foreach (string swear in swears)
            {
                string pattern = "\\b" + swear + "\\b";
                filteredText = Regex.Replace(filteredText, pattern, match =>
                {
                    return new string('*', match.Length);
                }, RegexOptions.IgnoreCase);
            }
        }
        inputField.text = filteredText;
    }

}
