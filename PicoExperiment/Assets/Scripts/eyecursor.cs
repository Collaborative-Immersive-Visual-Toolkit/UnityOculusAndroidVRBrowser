using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class eyecursor : MonoBehaviour
{
    public GameObject RightEye;
    public GameObject LeftEye;
    public GameObject Cursor;
    public LayerMask layerMask;
    public bool visible;

    public Vector3 currentPoint;
    public Vector2 currentPointUV;

    private Vector3 LastPoint;
    private Vector3 LastNormal;
    private bool hitted;

    void FixedUpdate()
    {
        if (RightEye == null) return;

        RaycastHit hit;

        if (Physics.Raycast(RightEye.transform.position, RightEye.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            hitted = true;
            LastPoint = hit.point;
            LastNormal = hit.normal;
            currentPoint = hit.point;
            currentPointUV = hit.textureCoord;

        }
        else {

            hitted = false;
            currentPoint = Vector3.zero;
            currentPointUV = Vector2.zero;
        }

    }

     void Update()
    {
      
        if (hitted)
        {
            object[] data = new object[] { LastPoint, LastNormal, visible, PhotonNetwork.NickName };

            if (visible)
            {
                Cursor.SetActive(true);
              
            }
            else 
            {
                Cursor.SetActive(false);

            }

            Cursor.transform.position = LastPoint;
            Cursor.transform.rotation = Quaternion.LookRotation(LastNormal, Vector3.up);

            gameObject.SendMessage("RaiseCursorUpdateEvent", data, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            object[] data = new object[] { Vector3.zero, Vector3.zero, false,PhotonNetwork.NickName };
            Cursor.SetActive(false);
            gameObject.SendMessage("RaiseCursorUpdateEvent", data, SendMessageOptions.DontRequireReceiver);
        }
    }
}
