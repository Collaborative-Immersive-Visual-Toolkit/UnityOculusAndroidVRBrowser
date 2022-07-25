using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class switchCondition : MonoBehaviour
{
    public int condition;
    private void Update()
    {

        if (Input.GetKeyDown("1"))
        {
            ConditionOne();
            RaiseUiSwitch();
        }
        else if (Input.GetKeyDown("2"))
        {
            ConditionTwo();
            RaiseUiSwitch();
        }
        else if (Input.GetKeyDown("3"))
        {
            ConditionThree();
            RaiseUiSwitch();
        }

    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.UiHelperSwitch)
        {

            Debug.Log("0");

            object[] data = (object[])obj.CustomData;

           

            condition = (int)data[0];

            if (condition == 1) ConditionOne();
            else if (condition == 2) ConditionTwo();
            else if (condition == 3) ConditionThree();

            Debug.Log("00");
        }
    }

    public void RaiseUiSwitch()
    {
        object[] data = new object[] { condition };
        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.UiHelperSwitch, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }

    public void ConditionOne() {

        Debug.Log("1");
        condition = 1;
        Debug.Log("2");
        SwitchOffVoiceCone();
        Debug.Log("3");
        SwitchOffEyeCursor();
        Debug.Log("4");
        SwitchOnCone();
        Debug.Log("5");
        
  
    }

    public void ConditionTwo()
    {
        condition = 2;

        SwitchOffCone();
        SwitchOffEyeCursor();

        SwitchOnVoiceCone();

    
    }

    public void ConditionThree() 
    {

        condition = 3;

        SwitchOffCone();
        SwitchOffVoiceCone();

        SwitchOnEyeCursor();

    }


    // cone 

    public void SwitchOnCone() {
        SwitchOnOwnCone();
        SwitchOnOthersCone();
    }

    public void SwitchOffCone()
    {
        SwitchOffOwnCone();
        SwitchOffOthersCone();
    }

    public void SwitchOffOwnCone()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            if (c.visible) c.SwitchVis();

        }

    }

    public void SwitchOnOwnCone()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            if (!c.visible) c.SwitchVis();

        }

    }

    public void SwitchOnOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("RemoteAvatar");

        foreach (GameObject obj in objs)
        {

            RemoteVisualCone rvc = obj.GetComponentInChildren<RemoteVisualCone>();
            if (rvc != null)
            {
                if (!rvc.visible) rvc.SwitchVis();

            }

        }


    }

    public void SwitchOffOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("RemoteAvatar");

        foreach (GameObject obj in objs)
        {

            RemoteVisualCone rvc = obj.GetComponentInChildren<RemoteVisualCone>();
            if (rvc != null)
            {
                if (rvc.visible) rvc.SwitchVis();

            }

        }


    }


    // voice 

    public void SwitchOnVoiceCone()
    {
        SwitchOnCone();
        SwitchOnConeVoice();
    }

    public void SwitchOffVoiceCone()
    {
        SwitchOffCone();
        SwitchOffConeVoice();
    }
    
    public void SwitchOffConeVoice() {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            c.interpolateElipse = false;
            c.reverse = false;

        }
    }

    public void SwitchOnConeVoice()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("owncone");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            c.interpolateElipse = true;
            c.reverse = true;

        }
    }



    // eye cursor 

    public void SwitchOnEyeCursor() {

        SwitchOnOwnEyeCursor();
        SwitchOnOtherEyeCursor();
    }

    public void SwitchOffEyeCursor()
    {
        SwitchOnOwnEyeCursor();
        SwitchOnOtherEyeCursor();
    }

    public void SwitchOffOwnEyeCursor() {


        GameObject obj = GameObject.FindGameObjectWithTag("EyeCursor");

        if (obj != null)
        {

            eyecursor c = obj.GetComponent<eyecursor>();
            c.visible = false;

        }
    }

    public void SwitchOnOwnEyeCursor()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("EyeCursor");

        if (obj != null)
        {

            eyecursor c = obj.GetComponent<eyecursor>();
            c.visible = true;

        }

    }

    public void SwitchOffOtherEyeCursor()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("RemoteAvatar");

        foreach (GameObject obj in objs)
        {

            RemoteEyeCursor rec = obj.GetComponentInChildren<RemoteEyeCursor>();
            if (rec != null)
            {
                if (rec.display) rec.display=false;

            }

        }


    }

    public void SwitchOnOtherEyeCursor()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("RemoteAvatar");

        foreach (GameObject obj in objs)
        {

            RemoteEyeCursor rec = obj.GetComponentInChildren<RemoteEyeCursor>();
            if (rec != null)
            {
                if (rec.display) rec.display = true;

            }

        }

    }
}
