using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteVisualCone : MonoBehaviour
{

    public bool visible = true;
    public LineRenderer lineRenderer;
    public Color c;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        c = lineRenderer.materials[0].color;
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

            if ((string)data[2] == gameObject.transform.parent.gameObject.name)
            {

                if(visible)UpdateVisualCone(data[0],data[1]);

            }


        }
    }

    private void UpdateVisualCone(object positions,object alpha)
    {

        Vector3[] pos = (Vector3[])positions;

        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions((Vector3[])pos);

        c.a = (float)alpha;
        lineRenderer.materials[0].color = c;
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
