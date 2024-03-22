// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything


using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    float TimeC;                          //Create a variable currenttime

    public bool isEditor;

    public TMP_Text Timertext;              //Get the text object
    public EditorControls.PlayState ps; 
    public EditorControls editorcontrols;

    private void Start()
    {
        if (isEditor) return;
        TimeC = 0;
    }
    void OnEnable()
    {
        if (!isEditor) return;
        TimeC = 0;
    }
    void Update()
    {
        if (!editorcontrols.PauseTimer || !isEditor) 
        { 
            TimeC += Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(TimeC);

            Timertext.text = t.ToString(@"mm\:ss");
        }
    }
}
