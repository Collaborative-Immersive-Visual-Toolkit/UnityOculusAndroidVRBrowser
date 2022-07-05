using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRayCasting : MonoBehaviour
{

    public GameObject RightEye;
    public GameObject LeftEye;
    public LayerMask layerMask;
    public float FixationTresholdBelow;
    public float SaccadeTresholdAbove;
    public LineRenderer lr;

    private Vector3[] m_ClearArray = new[] { Vector3.zero, Vector3.zero };
    private Queue<Vector3> points = new Queue<Vector3>();
    private Vector3 LastPoint;

    void FixedUpdate()
    {

            RaycastHit hit;

            if (Physics.Raycast(RightEye.transform.position, RightEye.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("hit");
                ProcessHitPoint(hit.point);
            }

    }

    private void Update()
    {
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

            if (angle > FixationTresholdBelow && angle < SaccadeTresholdAbove) //add point
            {
                AddPonint(point);
            } 
            else if (angle > SaccadeTresholdAbove) //clear quee
            {
                Debug.Log("Cleared");
                points.Clear();
            }

        }

    }

    void AddPonint(Vector3 point) {

        Debug.Log("Point Added ->" + points.Count);
        LastPoint = point;
        points.Enqueue(point);

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
        lr.material.color = Color.red;
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());

    }

    private void ClearLineRenderer()
    {
        
        lr.positionCount = 0;
    }
}
