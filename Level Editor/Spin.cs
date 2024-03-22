// Copyright © bl4ck & XDev, 2022-2024
// i fucking hate everything this script is not bad though

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spin : MonoBehaviour
{
    public float amount;
    public bool isEnabled;
    EditorControls controls;

    private void Start()
    {
        controls = GetComponent<ObjectComponent>().editorControls;
    }

    private void Update()
    {
        if (!MainEditorComponent.Instance.player.GetComponent<Die>().isEditor || 
            controls.pstate == EditorControls.PlayState.play && isEnabled)
        transform.localEulerAngles += new Vector3(0, 0, amount) * Time.deltaTime;
    }


}
