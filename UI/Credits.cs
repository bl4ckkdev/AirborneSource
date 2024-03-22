// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public void OpenLink(string link)
    {
        Application.OpenURL("https://"+link);
    }
}
