using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Positioning : MonoBehaviour
{
    InputDevice deviceleft;
    InputDevice deviceright;
    bool triggerValue;

    public XRNode controllerleft;
    public XRNode controllerright;
    bool pressedleft = false;
    bool pressedright = false;

    public GameObject player;
    private void Start()
    {

        var DevicesLeft = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(controllerright, DevicesLeft);
        if (DevicesLeft.Count == 1)
        {
            deviceleft = DevicesLeft[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", deviceleft.name, deviceleft.role.ToString()));
        }
        else if (DevicesLeft.Count > 1)
        {
            Debug.Log("Found more than one left/right hand!");
        }


        InputDevices.GetDevicesAtXRNode(controllerleft, DevicesLeft);
        if (DevicesLeft.Count == 1)
        {
            deviceright = DevicesLeft[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", deviceright.name, deviceright.role.ToString()));
        }
        else if (DevicesLeft.Count > 1)
        {
            Debug.Log("Found more than one left/right hand!");
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (deviceleft.TryGetFeatureValue(CommonUsages.secondaryButton, out triggerValue) && triggerValue)
        {
            if (!pressedleft)
            {
                placeleft();
                pressedleft = true;
            }

        }
        else
        {

            pressedleft = false;
        }


        if (deviceright.TryGetFeatureValue(CommonUsages.secondaryButton, out triggerValue) && triggerValue)
        {
            if (!pressedright)
            {
                placeright();
                pressedright = true;
            }

        }
        else
        {

            pressedright = false;
        }



    }

    void placeright()
    {

        player.transform.position = new Vector3(0.34f,0f,0.78f);

    }

    void placeleft()
    {

        player.transform.position = new Vector3(-0.32f, 0f, -0.91f);

    }
}
