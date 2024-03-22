// Copyright © bl4ck & XDev, 2022-2024
using TMPro;
using UnityEngine;

public class ClampField : MonoBehaviour
{
    public float min, max;

    public void Clamp(TMP_InputField field)
    {
        field.text = Mathf.Clamp(float.Parse(field.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture), min, max).ToString();
    }
}
