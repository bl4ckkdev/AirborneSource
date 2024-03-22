// Copyright © bl4ck & XDev, 2022-2024
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public bool isEnabled;
    public Vector2 to, from;
    public float wait;
    public float speed;
    public bool restartWhenDeath;
    public bool instantlyStart;
    public bool useTime;
    public bool loop;
    public bool started;
    public bool goBack;
    public bool stickable;
    
    
    [SerializeField] public bool movingToB = true;

    public Coroutine stop;
    public bool finale;
    public bool cont;
    EditorControls ec;
    private bool busy;

    public bool goingBack;
    public LineRenderer lr;
    public bool canEnableLineRenderer = false;

    public List<Points> points = new List<Points>();
    public int currentPoint = -1;

    public Vector3 startPos;
    public bool finished;

    private void Start()
    {
        if (!finale)
            ec = GetComponent<ObjectComponent>().editorControls;
        try
        {
            lr = GetComponent<LineRenderer>();
            lr.positionCount = 0;
        }
        catch { }
        
        if (finale) OnStart();
    }

    public Vector3 lastPosition;
    public void Update()
    {
        
        if (MainEditorComponent.Instance.finale) return;
        if (isEnabled && ec.pstate != EditorControls.PlayState.play && canEnableLineRenderer)
        {
            lr.enabled = true;
            UpdateLineRenderer();
        }
        else
        {
            lr.enabled = false;
        }
    }

    private void LateUpdate()
    {
        //if (stickable)
        //{
        //    Vector3 position = transform.position;
//
        //    Vector3 distance = position - lastPosition;
        //    foreach (GameObject g in collidingObjects)
        //    {
        //        g.transform.position += distance;
        //    }
        //
        //    lastPosition = position;
        //}
    }

    public List<GameObject> collidingObjects;
    
    private void OnCollisionEnter(Collision other)
    {
        if (TryGetComponent(out Rigidbody rg) && collidingObjects.IndexOf(other.gameObject) == -1)
        {
            collidingObjects.Add(other.gameObject);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (TryGetComponent(out Rigidbody rg))
        {
            rg.interpolation = RigidbodyInterpolation.Interpolate;
            collidingObjects.Remove(other.gameObject);
        }
    }

    private void UpdateLineRenderer()
    {
        lr.positionCount = points.Count + 1;

        lr.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));

        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i + 1, new Vector3(points[i].to.x, points[i].to.y, 0));
        }
    }


    public void OnStart(bool resetPos = true)
    {
        finished = false;
        if (resetPos) startPos = transform.position;
        currentPoint = 1;
        if (isEnabled && points.Count > 0)
            nextPoint = StartCoroutine(NextPoint(false));
    }

    public Coroutine nextPoint;
    public IEnumerator NextPoint(bool olt)
    {

        if (cont || finished) yield return null;
        print($"testing!! nextpoint {gameObject.name} | {points[0].speedortime}");
        List<Points> tempPoints = points.ToList();

        Points point = new Points();
        point.to = startPos;
        point.speedortime = points[0].speedortime;
        point.offset = points[0].offset;
        point.ease = points[0].ease;
        tempPoints.Insert(0, point);

        
        
        yield return new WaitForSeconds(tempPoints[currentPoint].offset);

        if (cont || finished) yield return null;

        Points target = tempPoints[currentPoint];

        Tween ea;
        if (useTime)
        {
            ea = transform.DOMove(target.to, target.speedortime).SetEase(target.ease.CustomEase()).SetAutoKill(false);
        }
        else
        {
            Vector3 startPosition = transform.position;
            float distance = Vector3.Distance(startPosition, target.to);
            float duration = distance / target.speedortime;
            ea = transform.DOMove(target.to, duration).SetEase(target.ease.CustomEase()).SetAutoKill(false);
        }

        ea.OnComplete(() =>
        {
            if (cont) return;

            if (currentPoint + 1 >= tempPoints.Count)
            {
                if (goBack)
                {
                    goingBack = true;
                    currentPoint -= 1;
                }
                else if (loop)
                {
                    transform.position = startPos;
                    currentPoint = 1;
                }
                else StopCoroutine(NextPoint(false));
            }
            else if (goingBack)
            {
                currentPoint -= 1;
                if (currentPoint < 0)
                {
                    goingBack = false;
                    currentPoint = 1;
                }
            }
            else currentPoint += 1;
        });
        
        yield return new WaitUntil(() => ea.IsComplete());
        ea.Kill();
        
        if (!olt)
        {
            if (!loop && currentPoint == 0) StartCoroutine(NextPoint(true));
            else nextPoint = StartCoroutine(NextPoint(false));
        }
    }
}

[System.Serializable]
public class Points
{
    public Vector2 to;
    public float offset;
    public float speedortime;
    public CustomEases ease;

    public enum CustomEases
    {
        Linear,
        InQuart,
        OutQuart,
        InOutQuart,
        InBounce,
        OutBounce,
        InOutBounce
    }
}

public static class CustomEaseExtensions
{
    public static Ease CustomEase(this Points.CustomEases value)
    {
        switch (value)
        {
            case Points.CustomEases.Linear: return Ease.Linear;
            case Points.CustomEases.InQuart: return Ease.InQuart;
            case Points.CustomEases.OutQuart: return Ease.OutQuart;
            case Points.CustomEases.InOutQuart: return Ease.InOutQuart;
            case Points.CustomEases.InBounce: return Ease.InBounce;
            case Points.CustomEases.OutBounce: return Ease.OutBounce;
            case Points.CustomEases.InOutBounce: return Ease.InOutBounce;
        }

        return Ease.Linear;
    }
}
