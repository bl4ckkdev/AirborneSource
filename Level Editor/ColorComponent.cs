// Copyright © bl4ck & XDev, 2022-2024
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorComponent : MonoBehaviour
{
    public bool enabled, g_enabled;
    
    public int r;
    public int g;
    public int b;
    public int a;

    public int g_r;
    public int g_g;
    public int g_b;
    public int g_a;

    public void Disable()
    {
        base.enabled = false;
    }

    public void Enable() => base.enabled = true;

    [SerializeField] private bool isText;
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Color originalColor;
    private Color g_originalColor = new Color(0, 0, 0, 0);
    
    void Update()
    {
        if (enabled) 
        {
            if (!isText && objectRenderer != null)
            {
                objectRenderer.enabled = true;
                if (a == 0) objectRenderer.enabled = false;
                else if (a < 255)
                {
                    ChangeMode(objectRenderer.material, BlendMode.Transparent); 
                }
                else ChangeMode(objectRenderer.material, BlendMode.Opaque); 
                objectRenderer.material.color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            }
            else if (text != null) text.color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
        else
        {
            if (objectRenderer != null && objectRenderer.material.color != originalColor) objectRenderer.material.color = originalColor;
            else if (text != null && text.color != originalColor) text.color = originalColor;
        }

        if (g_enabled) 
        {
            if (!isText && objectRenderer != null)
                objectRenderer.material.SetColor("_EmissionColor", new Color(g_r / 255f, g_g / 255f, g_b / 255f, g_a / 255f) * 2);
            else if (text != null) text.fontMaterial.SetColor("_GlowColor" , new Color(g_r / 255f, g_g / 255f, g_b / 255f, g_a / 255f) * Mathf.Pow(2, 2));
        }
        else
        {
            if (objectRenderer != null && objectRenderer.material.GetColor("_EmissionColor") != g_originalColor) objectRenderer.material.SetColor("_EmissionColor", g_originalColor);
            else if (text != null && text.fontMaterial.GetColor("_GlowColor") != g_originalColor) text.fontMaterial.SetColor("_GlowColor", g_originalColor);
        }
    }
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }
    
    public static void ChangeMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }
}

public static class MaterialExtensions
{
    public static void ToOpaqueMode(this Material material)
    {
        //material.SetOverrideTag("RenderType", "");
        //material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
        //material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
        //material.SetInt("_ZWrite", 1);
        //material.DisableKeyword("_ALPHATEST_ON");
        //material.DisableKeyword("_ALPHABLEND_ON");
        //material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        
        material.SetFloat("_Mode", 0);
        material.renderQueue = -1;
    }
   
    public static void ToFadeMode(this Material material)
    {
        //material.SetOverrideTag("RenderType", "Transparent");
        //material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        //material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //material.SetInt("_ZWrite", 0);
        //material.DisableKeyword("_ALPHATEST_ON");
        //material.EnableKeyword("_ALPHABLEND_ON");
        //material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.SetFloat("_Mode", 3);
        material.renderQueue = 3999;
    }
}


