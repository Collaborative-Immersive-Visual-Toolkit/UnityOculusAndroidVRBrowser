using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class remote_reorientation_manager : MonoBehaviourPun
{
    private Transform t;

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
        if (obj.Code == MasterManager.GameSettings.Reorient)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[1] == gameObject.transform.parent.gameObject.name)
            {

                t = (Transform)data[0];

                this.transform.position = t.position;
                this.transform.rotation = t.rotation;

            }

        }

    }


    
}
