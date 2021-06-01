using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteVisualCone : MonoBehaviour
{
   
    public LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
        if (obj.Code == MasterManager.GameSettings.VisualConeChange)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[1] == gameObject.transform.parent.gameObject.name)
            {

                UpdateVisualCone(data[0]);

            }


        }
    }

    private void UpdateVisualCone(object positions)
    {

        Vector3[] pos = (Vector3[])positions;
        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions((Vector3[])pos);
    }
}
