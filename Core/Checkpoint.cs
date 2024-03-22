// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isEnabled = true, startPos;
    public int priority;

    public Material disabled, achieved, start;
    public ParticleSystem particles;
    private MeshRenderer mr;

    public bool isEditor = true;

    public bool Active() => MainEditorComponent.Instance.activeCheckpoint == this;
    
    private void Update()
    {
        if (MainEditorComponent.Instance.editorControls.pstate == EditorControls.PlayState.stop)
        {
            if (startPos && isEnabled)
            {
                SetMaterials(start);
                MainEditorComponent.Instance.activeCheckpoint = this;
            }

            if (isEditor)
            if ((!startPos || !isEnabled) && Active())
            {
                MainEditorComponent.Instance.activeCheckpoint = null;
            }
            
        }
    }

    private void OnDestroy()
    {
        if (Active()) MainEditorComponent.Instance.activeCheckpoint = null;
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.TryGetComponent(out CharacterController2D cc) && isEnabled && !Active())
        {
            if (MainEditorComponent.Instance.activeCheckpoint == null || priority >= MainEditorComponent.Instance.activeCheckpoint.priority)
            {
                
                foreach (Checkpoint cp in MainEditorComponent.Instance.checkpoints)
                {
                    cp.SetMaterials(disabled);
                }
                MainEditorComponent.Instance.activeCheckpoint = this;
                
                foreach (GameObject b in MainEditorComponent.Instance.buttons)
                {
                    if (b.transform.GetChild(1).TryGetComponent(out Button biton) && biton.isEnabled)
                    {
                        print("db");
                        Die.Instance.savedButtons.Add(b);
                    }
                }
                cc.checkpointPriority = priority;
                SetMaterials(achieved);
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                particles.Play();
                transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutQuart);
            }
        }
    }

    public void SetMaterials(Material mat)
    {
        MeshRenderer[] mrenderer = GetComponentsInChildren<MeshRenderer>();
        if (TryGetComponent(out MeshRenderer mr))
        {
            mr.sharedMaterial = mat;
        }
        if (mrenderer != null)
        {
            for (int i = 0; i < mrenderer.Length; i++)
            {
                mrenderer[i].sharedMaterial = mat;
            }
        }
    }
}
