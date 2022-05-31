using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.Events;

[System.Serializable]
public class PushElipsesPoints : UnityEvent<List<Vector3>> { }


public class DetectedPoints : MonoBehaviour
{

    public List<DetectedPoint> points = new List<DetectedPoint>();

    public float expirytime = 150000f;

    public int segmentsLenght = 20;

    public List<Vector3> ElipsePoints;

    public LineRenderer lineRenderer;

    private List<Vector3> oldlistID = new List<Vector3>();

    private MeshCollider ConeMeshSaved;

    public PushElipsesPoints pushPoints;

    public bool renderCoordinates = false;

    public bool renderElipse = false;

    public void AddAPoint(Vector3 point)
    {

        DetectedPoint newpoint = new DetectedPoint();

        newpoint.Assign(point);

        if (renderCoordinates)
        {
            newpoint.g = Instantiate(Resources.Load("sphere", typeof(GameObject))) as GameObject;

            newpoint.g.transform.position = point;
        }

        points.Add(newpoint);

        
    }

    public void clearExpiredPoints()
    {

        var rp = points.FindAll(r => Time.time - r.time > expirytime);

        for (int i = 0; i < rp.Count; i++)
        {
           if(rp[i].g!=null) Destroy(rp[i].g);
        }

        int removed = points.RemoveAll(r => Time.time - r.time > expirytime);

        if (removed > 0) Elipse();

    }

    public void clearPointNotInCone(MeshCollider ConeMesh) {


        for (int i = 0; i < points.Count; i++)
        {
            if (!checkIfInside(points[i].point, ConeMesh))
            {
                if (points[i].g != null) Destroy(points[i].g);

                points[i].time = -1f;

            }
        }

        int removed = points.RemoveAll(r =>  r.time == -1f);

        if (removed > 0) Elipse();
    }

    public bool checkIfInside(Vector3 point, MeshCollider ConeMesh)
    {
        if (ConeMesh == null) return false;
        else if (ConeMeshSaved == null) ConeMeshSaved = ConeMesh;

       Ray rayup = new Ray(point, Vector3.up);
        Ray raydown = new Ray(point, Vector3.down);
        RaycastHit outhitup;
        RaycastHit outhitdown;

        bool hitup = ConeMesh.Raycast(rayup, out outhitup, Mathf.Infinity);

        if (hitup)
        {
            bool hitdown = ConeMesh.Raycast(raydown, out outhitdown, Mathf.Infinity);
            if (hitdown) return true;
            else return false;
        }
        else return false;
    }

    public void GizmoElipse()
    {

        if (points.Count > 2)
        {
            List<Vector3> ps = points.Select(r => r.point).ToList<Vector3>();

            Vector3 origin;

            Vector3 direction = Vector3.zero;

            Fit.LineFast(ps, out origin, ref direction, 1, true);

            Vector3 n = Normal(ps[0], ps[1], ps[2]);

            Vector3 directionPerpendicular = Vector3.Cross(direction, n).normalized;

            Gizmos.DrawRay(origin, directionPerpendicular * 2f);
            Gizmos.DrawRay(origin, -directionPerpendicular * 2f);

            float rX = points.Select(r => Mathf.Abs(Vector3.Dot((r.point - origin), direction))).Max();

            float rY = points.Select(r => Mathf.Abs(Vector3.Dot((r.point - origin), directionPerpendicular))).Max();

            DrawEllipseDebug(origin, n, direction, rY, rX, 20, Color.red);
        }
    }

    public void Elipse()
    {   
        
        List<Vector3> ps = points.Select(r => r.point).ToList<Vector3>();

        if (points.Count > 2 && !oldlistID.SequenceEqual(ps))
        {

            oldlistID = ps;
                
            Vector3 origin;

            Vector3 direction = Vector3.zero;


            //principal component 

            Fit.LineFast(ps, out origin, ref direction, 5, false);

            Vector3 n = Normal(ps[0], ps[1], ps[2]);


            //ortogonal component 
            Vector3 directionPerpendicular = Vector3.Cross(direction, n).normalized;


            //radius of elipse  
            float rX = points.Select(r => Mathf.Abs(Vector3.Dot((r.point - origin), direction))).Max();

            float rY = points.Select(r => Mathf.Abs(Vector3.Dot((r.point - origin), directionPerpendicular))).Max();


            //better fit the elipses 
            //int maxIteration = 10;
            //(rX,rY,maxIteration) = Optimize(rX, rY, direction, directionPerpendicular, maxIteration);

            CalculateEllipsePoints(origin, n, direction, rY, rX, segmentsLenght);

            UpdateLineRenderer();

            if (ElipsePoints!=null) pushPoints.Invoke(ElipsePoints);

            //CreateSphereOnFirstPoint();

        }
        else {

            ClearLineRenderer();

            pushPoints.Invoke(new List<Vector3>());
        }
    }

    public void CreateSphereOnFirstPoint() {

        GameObject g = GameObject.Find("First Point");

        if (gameObject != null) Destroy(g);

        g = Instantiate(Resources.Load("sphere", typeof(GameObject))) as GameObject;

        g.transform.position = ElipsePoints[0];

        g.name = "First Point";


    }

    public (float rX, float rY, int maxIteration) Optimize(float rX, float rY,Vector3 direction, Vector3 directionPerpendicular, int maxIteration = 10) {

        if(maxIteration<1) return (rX,rY,maxIteration);

        maxIteration -= 1;

        //find focal points from radius         
        float focalDistance = Mathf.Sqrt(Mathf.Pow(rX, 2f) - Mathf.Pow(rY, 2f));

        Vector3 f1 = direction.normalized * focalDistance;

        Vector3 f2 = direction.normalized * focalDistance * -1;

        Vector3 furthestPoint = points.Select(r => new { distance = Vector3.Distance(r.point, f1) + Vector3.Distance(r.point, f2), point = r.point })
            .OrderBy(r => r.distance).Select(r => r.point).Last();

        if (furthestPoint.magnitude < rX)
        {
            Debug.Log("No points out anymore");
            return (rX, rY, maxIteration);
        }

        float Xc = Vector3.Dot(direction,furthestPoint);
        float Yc = Vector3.Dot(directionPerpendicular, furthestPoint);

        if (Xc / rX > Yc / rY)
        {
            float newrX = rX + rX / 30;
            float newrY = rY + rY / 60;

            if (checkIfInside(newrX * direction, ConeMeshSaved ) && checkIfInside(newrX * direction *-1, ConeMeshSaved)) rX = newrX;
            if (checkIfInside(newrY * directionPerpendicular, ConeMeshSaved) && checkIfInside(newrY * directionPerpendicular*-1, ConeMeshSaved)) rY = newrY;
            
            Debug.Log("Increased X");
            return Optimize(rX, rY, direction, directionPerpendicular, maxIteration);

        }
        else {

            float newrX = rX + rX / 60;
            float newrY = rY + rY / 30;

            if (checkIfInside(newrX * direction, ConeMeshSaved) && checkIfInside(newrX * direction * -1, ConeMeshSaved)) rX = newrX;
            if (checkIfInside(newrY * directionPerpendicular, ConeMeshSaved) && checkIfInside(newrY * directionPerpendicular * -1, ConeMeshSaved)) rY = newrY;

            Debug.Log("Increased Y");
            return Optimize(rX, rY, direction, directionPerpendicular, maxIteration);
        }



    }

    public Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
    {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

    public static void DrawEllipseDebug(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
    {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0)
            { 
                    Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration); 
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    public void CalculateEllipsePoints(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments)
    {
        forward = ChooseSign(pos, forward) * 0.01f;
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;
        ElipsePoints = new List<Vector3>();

        for (int i = 0; i < segments ; i++)
        {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            ElipsePoints.Add( (rot * thisPoint + pos) + forward);
            
            angle += 360f / segments;
        }


        //reorder
        Vector3 top = ElipsePoints.OrderBy(x => x.y).Last();
        int Index = ElipsePoints.FindIndex(x => x == top);
        List<Vector3> ElipsePointsCopy = new List<Vector3>();
        for (int i = 0; i < ElipsePoints.Count; i++)
        {

            int shifted = (i + Index + ElipsePoints.Count) % ElipsePoints.Count; // index rollover

            ElipsePointsCopy.Add(ElipsePoints[shifted]);

        }
        ElipsePoints = ElipsePointsCopy;
    }

    public Vector3 ChooseSign(Vector3 pos, Vector3 forward) {

        var NormalDirection1 = forward * 1f;
        var NormalDirection2 = forward * -1f;

        var pos1 = pos + NormalDirection1;
        var pos2 = pos + NormalDirection2;

        var distance1 = Vector3.Distance(pos1, Vector3.zero);
        var distance2 = Vector3.Distance(pos2, Vector3.zero);

        if (distance1 < distance2) return NormalDirection1.normalized;
        else return NormalDirection2.normalized;
    }

    public void UpdateLineRenderer() {

        if (renderElipse)
        {
            lineRenderer.positionCount = ElipsePoints.Count;
            lineRenderer.SetPositions(ElipsePoints.ToArray());
        }

    }

    public void ClearLineRenderer()
    {

        lineRenderer.positionCount = 0;

    }


}



public class DetectedPoint
{

    public Vector3 point;
    public float time;
    public GameObject g;

    public void Assign(Vector3 p)
    {

        point = p;
        time = Time.time;
      
    }

}