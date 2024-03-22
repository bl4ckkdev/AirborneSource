// Copyright © bl4ck & XDev, 2022-2024
using Unity.VisualScripting;
using UnityEngine;

public class Modeless : Events
{
    public Canvas cv;
    bool can;
    public EditorControls ec;

    public void Update()
    {
        UpdateEvents();
    }

    public override void Hold()
    {
        if (!can) return;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cv.transform as RectTransform, Input.mousePosition, cv.worldCamera, out pos);
        transform.parent.position = cv.transform.TransformPoint(pos);
    }

    public override void Down()
    {
        can = true;
    }

    public override void Up()
    {
        can = false;
    }

    public override void Over()
    {
        ec.cantContinue = true;
        ec.canNumbers = false;
        ec.UIStop = true;
        ec.cantScroll = true;
    }
    public override void Exit()
    {
        if (ec.pstate != EditorControls.PlayState.stop) return;
        ec.cantContinue = false;
        ec.canNumbers = true;
        ec.UIStop = false;
        ec.cantScroll = false;
    }
}
