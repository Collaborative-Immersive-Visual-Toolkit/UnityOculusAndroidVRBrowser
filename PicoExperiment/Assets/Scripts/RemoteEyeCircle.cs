using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteEyeCircle : MonoBehaviourPun
{
    public LineRenderer circlelr;

    private Vector3[] circlePos;

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
        if (obj.Code == MasterManager.GameSettings.EyeCircleIncome)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[1] == gameObject.transform.parent.gameObject.name)
            {

                circlePos = (Vector3[])data[1];
                updateCircleLineRender();

            }

        }
        else if (obj.Code == MasterManager.GameSettings.EyeCircleDestroy) {

            object[] data = (object[])obj.CustomData;

            if ((string)data[0] == gameObject.transform.parent.gameObject.name)
            {

                clearCircleLineRender();

            }
        }

    }


    private void updateCircleLineRender()
    {

        circlelr.positionCount = circlePos.Length;
        circlelr.SetPositions(circlePos);

    }

    private void clearCircleLineRender()
    {

        circlelr.positionCount = 0;

        
    }
}
