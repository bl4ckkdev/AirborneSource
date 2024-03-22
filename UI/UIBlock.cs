// Copyright © bl4ck & XDev, 2022-2024
using UnityEngine;

public class UIBlock : Events
{
    public EditorControls ec;

    private bool tooltip = true;
    public bool nuhUh;
    private GameObject tip;

    private void Start()
    {
        try
        {
            tip = transform.Find("Tooltip").gameObject;
            tip.SetActive(false);
        }
        catch { tooltip = false; }
        
        
    }

    private void Update()
    {
        UpdateEvents();
    }

    public override void Enter()
    {
        
    }
    public override void Over()
    {
        if (nuhUh) ec.nuhUh = true;
        ec.cantContinue = true;
        ec.canNumbers = false;
        ec.UIStop = true;
        ec.cantScroll = true;
        
        if (tooltip) tip.SetActive(true);
    }

    public override void Exit()
    {
        if (nuhUh) ec.nuhUh = false;
        ec.cantContinue = false;
        ec.canNumbers = true;
        ec.UIStop = false;
        ec.cantScroll = false;
        if (tooltip) tip.SetActive(false);
    }
}