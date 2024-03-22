// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;

public class FieldBlock : MonoBehaviour
{
    public EditorControls ec;

    public void Block()
    {
        ec.cantContinue = true;
    }

    public void Unblock()
    {
        ec.cantContinue = false;
    }
}
