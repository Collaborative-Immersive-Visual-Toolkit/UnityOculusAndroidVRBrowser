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
    private LaserBeamBehavior laserBeamBehavior;
    private LineRenderer lineRenderer;

    public GameObject stickyPointerPrefab;
    private GameObject stickyPointer;

    private bool insideOtherCone;

    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private bool _hitTarget;
    private bool sticky;

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

            if ((string)data[6] == gameObject.transform.parent.gameObject.name)
            {

                _startPoint = (Vector3)data[0];
                _endPoint = (Vector3)data[1];
                _hitTarget = (bool)data[2]; 
                sticky = (bool)data[3];
                laserBeamBehavior = (LaserBeamBehavior)data[4];
                insideOtherCone = (bool)data[5];

                UpdateLaserBeam();
                UpdateStickyPointer();
                UpdateMaterial();


            }

        }
 
    }

    private void UpdateLaserBeam()
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
            lineRenderer.SetPosition(0, _startPoint);
            lineRenderer.SetPosition(1, _endPoint);          
        }
        else if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            if (_hitTarget)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                    
                }

                lineRenderer.SetPosition(0, _startPoint);
                lineRenderer.SetPosition(1, _endPoint);
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

    private void UpdateStickyPointer() {

        if (stickyPointer == null)
        {
            stickyPointer = Instantiate(stickyPointerPrefab, _endPoint, Quaternion.identity);
        }

        if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            stickyPointer.transform.position = _endPoint;
        }

        stickyPointer.SetActive(sticky);
        
    }
}
