
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.XR;

public class RemoteLaser : MonoBehaviour
{
    public LineRenderer lrLeft;
    public LineRenderer lrRight;
    
    private XRNode controller;
    public Vector3 start;
    public Vector3 end;
    private bool display;
    
    private Vector3[] m_ClearArray = new[] { Vector3.zero, Vector3.zero };

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
        if (obj.Code == MasterManager.GameSettings.LaserPointerChange)
        {

            object[] data = (object[])obj.CustomData;

            //{ start, end, true, controller, PhotonNetwork.NickName };

            if ((string)data[4] == gameObject.transform.parent.gameObject.name)
            {

                start= (Vector3)data[0];
                end = (Vector3)data[1];
                display = (bool)data[2];
                controller = (XRNode)data[3];
                updateLaser();
            }

        }
    }

    private void updateLaser()
    {

        LineRenderer lr = controller == XRNode.LeftHand ? lrRight : lrLeft;

        if (display)
        {

            Vector3[] Array = new[] { start, end };
            lr.SetPositions(Array);
            lr.positionCount = 2;

        }
        else
        {
            lr.SetPositions(m_ClearArray);
            lr.positionCount = 0;

        }

    }
}
