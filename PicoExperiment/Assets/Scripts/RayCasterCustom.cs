using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class RayCasterCustom : MonoBehaviour
{
    public LineRenderer lr;

    Vector3[] m_ClearArray = new[] { Vector3.zero, Vector3.zero };

    public LayerMask layerMask;

    public XRNode controller;

    InputDevice device;
    bool triggerValue;

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

                //hit.collider.gameObject.GetComponent<GraphicRaycaster>().Raycast()
                //https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.GraphicRaycaster.Raycast.html
            }
            else
            {

                ClearLineRenderer();
            }

        } 
        else
        {
           
            ClearLineRenderer();
        }
       
    }

    private void UpdateLineRenderer(Vector3 start,Vector3 end)
    {
        lr.material.color = Color.red;
        Vector3[] Array = new[] { start, end };
        lr.SetPositions(Array);
        lr.positionCount = 2;
    }

    private void ClearLineRenderer()
    {
        lr.SetPositions(m_ClearArray);
        lr.positionCount = 0;
    }

}


