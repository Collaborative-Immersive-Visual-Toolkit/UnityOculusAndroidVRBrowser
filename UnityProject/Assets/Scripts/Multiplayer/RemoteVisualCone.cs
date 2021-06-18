using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteVisualCone : MonoBehaviour
{

    public bool visible = true;
    public LineRenderer lineRenderer;

    //gradient 
    private float from = 0.001f;
    private float to = 0.999f;
    private float howfar = 0f;
    private bool direction = true;
    private float alpha = 1f;


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

            if ((string)data[3] == gameObject.transform.parent.gameObject.name)
            {

                if(visible)UpdateVisualCone(data[0],data[1], data[2]);

            }


        }
    }

    private void UpdateVisualCone(object positions,object alpha, object middle)
    {

        Vector3[] pos = (Vector3[])positions;

        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions((Vector3[])pos);


        lineRenderer.materials[0].SetFloat("_Middle", (float)middle);

        lineRenderer.materials[0].SetFloat("_Alpha", (float)alpha);
    }

    public void SwitchVis()
    {

        visible = !visible;

        if (!visible) { clearLineRender(lineRenderer); } 


    }

    public void clearLineRender(LineRenderer lr)
    {
        lineRenderer.positionCount = 0;

    }
}
