// Copyright © bl4ck & XDev, 2022-2024
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CampaignLevel : MonoBehaviour
{
    public TMP_Text num;
    public TMP_Text levelName;

    public void MoveUp()
    {
        EditCampaign.instance.MoveUp(gameObject);
    }

    public void MoveDown()
    {
        EditCampaign.instance.MoveDown(gameObject);
    }

    public void Delete()
    {
        EditCampaign.instance.RemoveLevel(gameObject);
    }
}
