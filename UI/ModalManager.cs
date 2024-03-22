// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ModalManager : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text BodyText;

    public GameObject Modal;

    public Action PressedYes;

    public static ModalManager instance;
    public bool setInstance = true;

    private void Start()
    {
        if (!setInstance) return;
            
        if (instance != null) Destroy(instance);
        instance = this;
    }

    public void Cancel() => Modal.SetActive(false);
    public void Ok()
    {
        PressedYes?.Invoke();
        Modal.SetActive(false);
    }

    public void ShowModal(string Title, string BodyText)
    {
        this.Title.text = Title;
        this.BodyText.text = BodyText;

        Modal.SetActive(true);
    }


}
