// Copyright © bl4ck & XDev, 2022-2024
using TMPro;
using UnityEngine;

public class MinusField : MonoBehaviour
{
    public void Check(TMP_InputField ipf)
    {
        if (ipf.text.Contains('-')) ipf.characterLimit = 3;
        else ipf.characterLimit = 2;
    }
}
