// Copyright © bl4ck & XDev, 2022-2024
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : bl4ckdev.Events
{
    Image img;
    TMP_Text text;

    private void Start()
    {
        img = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        UpdateEvents();
    }

    public override void Over()
    {
        text.color = Color.black;
    }

    public override void Exit()
    {
        text.color = Color.white;
    }
}
