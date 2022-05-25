using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DetectedPoints : MonoBehaviour
{

    public List<DetectedPoint> points = new List<DetectedPoint>();

    public float expirytime = 5f;

    public void AddAPoint(Vector3 point)
    {

        DetectedPoint newpoint = new DetectedPoint();

        newpoint.Assign(point);

        newpoint.g = Instantiate(Resources.Load("sphere", typeof(GameObject))) as GameObject;

        newpoint.g.transform.position = point;

        points.Add(newpoint);


    }

    public void clearExpiredPoints()
    {
        var rp = points.FindAll(r => Time.time - r.time > expirytime);

        for (int i = 0; i < rp.Count; i++)
        {
           Destroy(rp[i].g);
        }

        points.RemoveAll(r => Time.time - r.time > expirytime);

    }

    public void clearPointNotInCone(MeshCollider ConeMesh) {


        for (int i = 0; i < points.Count; i++)
        {
            if (!checkIfInside(points[i].point, ConeMesh))
            {
                Destroy(points[i].g);

                points[i].time = -1f;

            }
        }

        points.RemoveAll(r =>  r.time == -1f);

    }

    public bool checkIfInside(Vector3 point, MeshCollider ConeMesh)
    {

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

    public void Line()
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

            DrawEllipse(origin, n, direction, rY, rX, 21, Color.red);
        }
    }


    Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
    {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

    private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
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