using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerNewList : UnityEvent<Vector3[]>{}

public class EyeRayCasting : MonoBehaviour
{

    public GameObject RightEye;
    public GameObject LeftEye;
    public LayerMask layerMask;
    
    public float TresholdBelow;
    public float TresholdAbove;    
    public int MaxPoints;

    public LineRenderer lr;

    private Queue<Vector3> points = new Queue<Vector3>();
    private Vector3 LastPoint;

    public TriggerNewList trigger;
    public UnityEvent deleteList;
    public bool ShowPath;


    void FixedUpdate()
    {
            if (RightEye == null) return;

            RaycastHit hit;

            if (Physics.Raycast(RightEye.transform.position, RightEye.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log("hit");
                ProcessHitPoint(hit.point);
            }

    }

    private void Update()
    {
        if (points.Count > MaxPoints) points.Dequeue();

        if (points.Count > 0) //add first point
        {
            UpdateLineRenderer();
        }
        else 
        {
            ClearLineRenderer();
        }
    }

    void ProcessHitPoint(Vector3 point) {

        if (points.Count == 0) //add first point
        {
            AddPonint(point);
        }
        else //check if you need to add first point 
        {

            float angle = calculateAngle(point);

            if (angle > TresholdBelow && angle < TresholdAbove) //add point
            {
                AddPonint(point);
            } 
            else if (angle > TresholdAbove) //clear quee
            {
                //Debug.Log("Cleared");
                points.Clear();
                deleteList.Invoke();
            }

        }

    }

    void AddPonint(Vector3 point) {

        //Debug.Log("Point Added ->" + points.Count);
        LastPoint = point;
        points.Enqueue(point);
        trigger.Invoke(points.ToArray());
    }

    //this output degree*second
    float calculateAngle(Vector3 point) {

        Vector3 vectorA = RightEye.transform.position - LastPoint;
        Vector3 vectorB = RightEye.transform.position - point;

        float angle = Vector3.Angle(vectorA, vectorB);

        return angle / Time.fixedDeltaTime;
    }

    private void UpdateLineRenderer()
    {
        if (!ShowPath) {

            ClearLineRenderer();
            return;
        }

        lr.material.color = Color.red;
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());

    }

    private void ClearLineRenderer()
    {
        
        lr.positionCount = 0;
    }




}
