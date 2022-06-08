using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{

    public MeshCollider ConeMesh;


    public void Start()
    {
        Debug.Log(checkIfInside(gameObject.transform.position));

    }

    public bool checkIfInside(Vector3 point)
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
