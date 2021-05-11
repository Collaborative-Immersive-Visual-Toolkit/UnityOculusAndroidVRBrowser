using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class RemoteHandInputSelector : MonoBehaviourPun
{
    public GameObject remoteAvatar {

        set {

            leftHandAnchor = GetChildWithName(value, "hand_left").transform;
            rightHandAnchor = GetChildWithName(value, "hand_right").transform;
            m_InputModule =  (OVRInputModule)value.GetComponentInChildren(typeof(OVRInputModule), true);  
        }
    }

    private Transform leftHandAnchor;
    private Transform rightHandAnchor;
    private OVRInputModule m_InputModule;

    //private void Start()
    //{
    //    if (remoteAvatar != null) { 
        

    //    }
    //}

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
        if (obj.Code == MasterManager.GameSettings.ChangeHandInputRemote)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[1] == gameObject.transform.parent.gameObject.name)
            {

                SetActiveController((string)data[0]);

            }


        }
    }

    void SetActiveController(string data)
    {

        Transform t;

        if (data == "Left")
        {
            t = leftHandAnchor;
        }
        else
        {
            t = rightHandAnchor;
        }
        
        m_InputModule.rayTransform = t;

    }

    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }
}
