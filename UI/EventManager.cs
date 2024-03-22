// Copyright © bl4ck & XDev, 2022-2024
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour
{

    public static bool IsPointerOverUIElement(GameObject g)
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults(), g);
    }

    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, GameObject g)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.name == g.name)
                return true;
        }
        return false;
    }
    
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
