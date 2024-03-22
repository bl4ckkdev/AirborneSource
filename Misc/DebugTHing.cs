// Copyright © bl4ck & XDev, 2022-2024
// this script is not capitalized right

using UnityEngine;

public class DebugTHing : MonoBehaviour
{
    public bool isEnabled;
    public GameObject[] stuffToDisable;

    private protected bool on;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Equals) && isEnabled)
        {
            on = !on;
            for (int i = 0; i < stuffToDisable.Length; i++)
            {
                stuffToDisable[i].SetActive(!on);
            }
        }
    }
}
