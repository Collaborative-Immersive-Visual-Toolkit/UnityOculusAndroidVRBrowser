using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InsightRecording : MonoBehaviour
{
    InputDevice device;
    bool triggerValue;
    public GameObject sphere;
    public XRNode controller;

    bool pressed = false;

    private void Start()
    {

        var Devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(controller, Devices);
        if (Devices.Count == 1)
        {
            device = Devices[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
        }
        else if (Devices.Count > 1)
        {
            Debug.Log("Found more than one left/right hand!");
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (device.TryGetFeatureValue(CommonUsages.primaryButton, out triggerValue) && triggerValue)
        {
            if (!pressed)
            {
                toggleSpehere();
                Debug.Log("toggle");
                pressed = true;
            }

        }
        else {

            pressed = false;
        }
    }

    void toggleSpehere() {

        if (sphere.activeSelf)
        {

            sphere.SetActive(false);

        }
        else {

            sphere.SetActive(true);
        }

    }

}
