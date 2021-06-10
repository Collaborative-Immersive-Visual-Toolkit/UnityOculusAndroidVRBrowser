using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteLaser : MonoBehaviourPun
{
    public enum LaserBeamBehavior
    {
        On,        // laser beam always on
        Off,        // laser beam always off
        OnWhenHitTarget,  // laser beam only activates when hit valid target
    }

    private LineRenderer lineRenderer;

    public GameObject stickyPointerPrefab;
    private GameObject stickyPointer;

    private bool insideOtherCone;


    private Color c;

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
        if (obj.Code == MasterManager.GameSettings.LaserPointerChange)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[5] == gameObject.transform.parent.gameObject.name)
            {
                insideOtherCone = (bool)data[4];
                UpdateLaserBeam((Vector3)data[0], (Vector3)data[1], (bool)data[2] , (LaserBeamBehavior)data[3]);
                UpdateMaterial();

            }


        } else if (obj.Code == MasterManager.GameSettings.StickyPointerChange) {


            object[] data = (object[])obj.CustomData;

            if ((string)data[3] == gameObject.transform.parent.gameObject.name)
            {

                UpdateStickyPointer((Vector3)data[0], (bool)data[1], (LaserBeamBehavior)data[2]);

            }
        }
    }

    private void UpdateLaserBeam(Vector3 start, Vector3 end, bool _hitTarget, LaserBeamBehavior  laserBeamBehavior)
    {
        if (laserBeamBehavior == LaserBeamBehavior.Off)
        {
            
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
            }

            return;
        }
        else if (laserBeamBehavior == LaserBeamBehavior.On)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);          
        }
        else if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            if (_hitTarget)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                    
                }

                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
            }
            else
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    private void UpdateMaterial()
    {

        if (insideOtherCone) lineRenderer.materials[0].color = Color.green;
        else lineRenderer.materials[0].color = c;
    }

    private void UpdateStickyPointer(Vector3 end, bool sticky, LaserBeamBehavior laserBeamBehavior) {

        if (stickyPointer == null)
        {
            stickyPointer = Instantiate(stickyPointerPrefab, end, Quaternion.identity);
        }

        if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            stickyPointer.transform.position = end;
        }

        stickyPointer.SetActive(sticky);
        
    }
}
