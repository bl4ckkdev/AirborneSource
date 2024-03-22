// Copyright © bl4ck & XDev, 2022-2024
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SetupCycle : MonoBehaviour
{
    public GameObject cycleObject;
    public GameObject contents;
    public GameObject buttonObject;

    public static SetupCycle instance;

    List<GameObject> cycleList = new List<GameObject>();

    private bool useTime;

    public void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void ClearCycles()
    {
       cycleList.ForEach(Destroy);
       cycleList.Clear();
    }
    public void UseTime(bool usequestionmark)
    {
        useTime = usequestionmark;
        if (usequestionmark)
        {
            foreach (GameObject go in cycleList) 
                go.GetComponent<Cycle>().speedortimetext.text = "Time";
        }
        else
        {
            foreach (GameObject go in cycleList)
                go.GetComponent<Cycle>().speedortimetext.text = "Speed";
        }
    }

    public void CreateCycle() => AddCycle();

    public Cycle AddCycle()
    {
        GameObject newCycle;
        newCycle = Instantiate(cycleObject, contents.transform);
        newCycle.GetComponent<Cycle>().speedortimetext.text = useTime ? "Time" : "Speed";

        cycleList.Add(newCycle);
        newCycle.GetComponent<Cycle>().cycleNumber = cycleList.IndexOf(newCycle) + 1;
        buttonObject.transform.SetAsLastSibling();
        
        return newCycle.GetComponent<Cycle>();
    }

    //public void CreateCycle()
    //{
    //    AddCycle();
    //}
    
    public void RemoveCycle(GameObject gobject)
    {
        cycleList.Remove(gobject);
        Destroy(gobject);

        UpdateList();
        buttonObject.transform.SetAsLastSibling();
    }

    public void MoveUp(GameObject gobject)
    {
        int index = cycleList.IndexOf(gobject);

        if (index > 0 && index < cycleList.Count)
        {
            GameObject temp = cycleList[index - 1];
            cycleList[index - 1] = gobject;
            cycleList[index] = temp;
            UpdateList();
        }
    }

    public void MoveDown(GameObject gobject)
    {
        int index = cycleList.IndexOf(gobject);

        if (index >= 0 && index < cycleList.Count - 1)
        {
            GameObject temp = cycleList[index + 1];
            cycleList[index + 1] = gobject;
            cycleList[index] = temp;
            UpdateList();
        }
    }

    private void UpdateList()
    {
        for (int i = 0; i < cycleList.Count; i++)
        {
            cycleList[i].transform.SetSiblingIndex(i);
            cycleList[i].GetComponent<Cycle>().cycleNumber = i + 1;
        }
    }

    public void FeedPoints(Points[] points)
    {
        foreach (var point in points)
        {
            Cycle cycle = AddCycle();
            cycle.to_x.text = point.to.x.ToString();
            cycle.to_y.text = point.to.y.ToString();
            cycle.offset.text = point.offset.ToString();
            cycle.speedortime.text = point.speedortime.ToString();
            cycle.ease.value = (int)point.ease;
        }
    }

    public List<Points> ConvertToPoints()
    {
        List<Points> points = new List<Points>();

        foreach (GameObject go in cycleList)
        {
            Cycle goss = go.GetComponent<Cycle>();
            points.Add(new Points
            {
                to = new Vector2(float.TryParse(goss.to_x.text, out float tox) ? tox : 0, float.TryParse(goss.to_y.text, out float toy) ? toy : 0),
                offset = float.TryParse(goss.offset.text, out float off) ? off : 0,
                speedortime = float.TryParse(goss.speedortime.text, out float sp) ? sp : 0,
                ease = (Points.CustomEases)goss.ease.value
            });
        }

        return points;
    }

    public void UpdateFields()
    {
        ObjectProperties.instance.UpdateFields(ObjectProperties.instance.plus);
    }
}