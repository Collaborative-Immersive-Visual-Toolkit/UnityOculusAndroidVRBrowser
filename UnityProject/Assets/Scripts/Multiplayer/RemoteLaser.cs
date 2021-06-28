using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteLaser : MonoBehaviourPun
{
    public GameObject Pointer;
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

    public bool insideOtherCone;

    private Vector3 _startPoint;
    public Vector3 _endPoint;
    private bool _hitTarget;
    private bool sticky;
    private bool reorient;

    private Color c;

    private Vector3[] circlepos;
    private float circlealpha;

    public Material[] Visible;
    public Material[] NonVisible;
    public stickyCircleRemote circle;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        //c = lineRenderer.materials[0].color;
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

            if ((string)data[8] == gameObject.transform.parent.gameObject.name)
            {

                _startPoint = (Vector3)data[0];
                _endPoint = (Vector3)data[1];
                _hitTarget = (bool)data[2]; 
                sticky = (bool)data[3];
                laserBeamBehavior = (LaserBeamBehavior)data[4];
                insideOtherCone = (bool)data[5];
                circlepos = (Vector3[])data[6];
                circlealpha = (float)data[7];

                UpdatePointer();
                UpdateLaserBeam();
                //UpdateStickyPointer();
                UpdateMaterial();
                UpdateStickyCircle();



            }

        }
 
    }

    private void UpdatePointer() {

        Pointer.transform.position = _endPoint;
    }
    
    private void UpdateLaserBeam()
    {
        if (laserBeamBehavior == LaserBeamBehavior.Off)
        {
            
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                Pointer.SetActive(false);
                _endPoint = Vector3.zero;
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
                    Pointer.SetActive(true);
                }

                lineRenderer.SetPosition(0, _startPoint);
                lineRenderer.SetPosition(1, _endPoint);
            }
            else
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                    Pointer.SetActive(false);
                    _endPoint = Vector3.zero;
                }
            }
        }
    }

    private void UpdateMaterial()
    {

            if (insideOtherCone) lineRenderer.materials= Visible;
            else lineRenderer.materials = NonVisible;
        
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

    private void UpdateStickyCircle()
    {

        if(circlepos != circle.pos) circle.updateLineRender(circlepos);
        if (circlealpha != circle.alpha)
        {
            circle.UpdateMaterial(circlealpha);
            circle.UpdateAlpha(circlealpha);
        }

    }


}
