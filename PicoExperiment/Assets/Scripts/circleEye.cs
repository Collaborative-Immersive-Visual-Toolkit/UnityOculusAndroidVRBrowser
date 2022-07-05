using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class circleEye : MonoBehaviour
{
    public LineRenderer circlelr;
    private Vector3[] pointlist;
    private Vector3 center;
    float[] pos = { 0f, 0.125f, 0.25f, 0.375f, 0.5f, 0.625f, 0.75f, 0.875f, 1f, 1.125f, 1.25f, 1.375f, 1.5f, 1.625f, 1.75f, 1.875f };
    public Vector3[] _circlePos = new Vector3[16];
    public Vector3[] circlePos
    {

        get
        {
            return _circlePos;
        }
        set
        {

            _circlePos = value;
            _circlePos = new Vector3[16];
        }
    }

    public void GenerateCircle(Vector3[] points)
    {
        pointlist = points;

        circlePos = new Vector3[16];

        if (pointlist.Length < 3) return;

        center = GetAveragePoint();
        //float radius = StdDev(center);
        float radius = Max();
        Vector3 normal = Normal(pointlist[0], pointlist[1], pointlist[2]);

        //create one ortogonal vector to the normal vector i 
        //  (0,z,−y) 
        Vector3 i = new Vector3(0f, normal.z, -normal.y);
        // (−z,0,x) 
        if (i.magnitude == 0) i = new Vector3(-normal.z, 0f, normal.x);
        // (y,−x, 0)
        if (i.magnitude == 0) i = new Vector3(normal.y, -normal.x, 0f);

        //normalize ortogonal vectors
        i = i.normalized;

        //create vector j ortogonal to both normal and i 
        Vector3 j = Vector3.Cross(normal, i).normalized;

        //create points for line renderer
        //cosθi +sinθj
        for (int c = 0; c < 16; c++)
        {
            circlePos[c] = center + i * radius * (float)Math.Cos(pos[c] * Math.PI) + j * radius * (float)Math.Sin(pos[c] * Math.PI);
        }

        updateCircleLineRender();
    }

    Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
    {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

    private float Max() {

       return pointlist.Max(p => Vector3.Distance(p,center));
    
    }

    
    
    private Vector3 GetAveragePoint()
    {

        Vector3 average = Vector3.zero;

        foreach (Vector3 p in pointlist) average += p;

        return average / pointlist.Length;

    }

    
    
    private float Mean(float[] list)
    {

        float mean = 0f;

        foreach (float p in list) mean += p;

        return mean / list.Length;

    }

    
    
    private float StdDev(Vector3 center)
    {
 

        float[] values = new float[pointlist.Length];

        for (int i = 0; i < pointlist.Length; i++)
        {
            Vector3 distance = center - pointlist[i];

            values[i] = distance.magnitude;
        }

        // Get the mean.
        float mean = Mean(values);

        // Get the sum of the squares of the differences
        // between the values and the mean.
        //var sum_of_squares = SumOfSquares(values, mean);

        //return (float)Math.Sqrt(sum_of_squares / values.Length);

        return mean;
    }

    
    
    private void updateCircleLineRender()
    {

        circlelr.positionCount = circlePos.Length;
        circlelr.SetPositions(circlePos);

    }

    public void clearCircleLineRender()
    {

        circlelr.positionCount = 0;

    }
}
