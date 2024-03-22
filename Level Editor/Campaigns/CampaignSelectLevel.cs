// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignSelectLevel : MonoBehaviour
{
    public TMP_Text text;
    public string levelName;
    public Image img;

    public void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        img = GetComponent<Image>();
    }

    public void Toggle(bool on)
    {
        
        
        img.DOKill();
        text.DOKill();

        if (!on)
        {
            SelectCampaignLevel.Instance.active.Remove(this);
            text.DOColor(Color.white, .1f);
            img.DOColor(new Color(0, 0, 0, 0.18f), .1f);
        }
        else
        {
            SelectCampaignLevel.Instance.active.Add(this);
            text.DOColor(Color.black, .1f);
            img.DOColor(Color.white, .1f);
        }
    }
}
