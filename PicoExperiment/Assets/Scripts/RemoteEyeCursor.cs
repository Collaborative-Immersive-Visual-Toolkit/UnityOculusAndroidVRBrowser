using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteEyeCursor : MonoBehaviour
{
    public GameObject Cursor;

    public Vector3 cursorPos;

    private Vector3 cursorNormal;

    public bool display;

    public bool off;

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
        if (obj.Code == MasterManager.GameSettings.CursorUpdate)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[3] == gameObject.transform.parent.gameObject.name)
            {

                cursorPos = (Vector3)data[0];
                cursorNormal = (Vector3)data[1];
                display = (bool)data[2];
                if(!off) updateCursor();

            }

        }
    }


    private void updateCursor()
    {

        if (display)
        {
            
            Cursor.SetActive(true);
            Cursor.transform.position = cursorPos;
            Cursor.transform.rotation = Quaternion.LookRotation(cursorNormal, Vector3.up);
           
        }
        else
        {
           
            Cursor.SetActive(false);
            
        }

    }


    
}
