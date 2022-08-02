using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class RayCasterCustom : MonoBehaviour
{
    public LineRenderer lr;

    Vector3[] m_ClearArray = new[] { Vector3.zero, Vector3.zero };

    public LayerMask layerMask;

    public XRNode controller;

    InputDevice device;
    bool triggerValue;

    public EventSystem m_EventSystem;

    public Vector3 point;
    public Vector2 pointUV;

    enum ControllerLeftRight : ushort
    {
        Left = 0,
        Right = 1,
    }

    private void Start()
    {

            var Devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(controller, Devices);
            if (Devices.Count == 1)
            {
                device = Devices[0];
                Debug.Log(string.Format("Device name '{0}' with role '{1}'",  device.name, device.role.ToString()));
            }
            else if (Devices.Count > 1)
            {
                Debug.Log("Found more than one left/right hand!");
            }


    }

    void FixedUpdate()
    {

        if (device.TryGetFeatureValue(CommonUsages.triggerButton,  out triggerValue) && triggerValue){

            RaycastHit hit;
        
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                UpdateLineRenderer(transform.position+transform.forward*0.1f, hit.point);
                SetUpPointerEvent(hit);
                point = hit.point;
                pointUV = hit.textureCoord;
            }
            else
            {
                ClearLineRenderer();
                point = Vector3.zero;
                pointUV = Vector2.zero;
            }

        } 
        else
        {
            ClearLineRenderer();
            point = Vector3.zero;
            pointUV = Vector2.zero;
        }
       
    }
    
    private void SetUpPointerEvent(RaycastHit hit) {

       

    }

    private void UpdateLineRenderer(Vector3 start,Vector3 end)
    {
        lr.material.color = Color.red;
        Vector3[] Array = new[] { start, end };
        lr.SetPositions(Array);
        lr.positionCount = 2;

        object[] data = new object[] { start, end, true, controller, PhotonNetwork.NickName };
        gameObject.SendMessage("RaiseLaserPointerChange", data, SendMessageOptions.DontRequireReceiver);
    }

    private void ClearLineRenderer()
    {
        if (lr.positionCount == 0) return;

        object[] data = new object[] { Vector3.zero, Vector3.zero, false, controller, PhotonNetwork.NickName };
        gameObject.SendMessage("RaiseLaserPointerChange", data, SendMessageOptions.DontRequireReceiver);

        lr.SetPositions(m_ClearArray);
        lr.positionCount = 0;

    }

}


