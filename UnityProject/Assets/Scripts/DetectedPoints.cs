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