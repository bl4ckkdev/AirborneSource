// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextComponent : MonoBehaviour
{
    public TMP_Text text;
    public string contents;
    public float fontSize;

    public bool bold;
    public bool italic;

    public string Text;
    public int size;

    private void Update()
    {
        text.fontStyle = (bold ? FontStyles.Bold : FontStyles.Normal) | (italic ? FontStyles.Italic : FontStyles.Normal);
    }

    public void ChangeText(string Text)
    {
        contents = Text;
        text.text = Text;
    }
    public void ChangeFontSize(float size)
    {
        fontSize = size;
		text.fontSize = size;
	}

    public void UpdateText()
    {
        text.text = contents;
        text.fontSize = fontSize;
    }
}
