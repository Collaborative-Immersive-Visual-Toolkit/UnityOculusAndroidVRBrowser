

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class LaserPointerModified : OVRCursor
{
    public enum LaserBeamBehavior
    {
        On,        // laser beam always on
        Off,        // laser beam always off
        OnWhenHitTarget,  // laser beam only activates when hit valid target
    }

    public GameObject cursorVisual;
    public float maxLength = 10.0f;

    public LaserBeamBehavior _laserBeamBehavior;
    bool m_restoreOnInputAcquired = false;

    public GameObject stickyPointerPrefab;
    private GameObject stickyPointer;
    private bool sticky;
    private bool stickyinsideOtherCone;
    public LaserBeamBehavior laserBeamBehavior
    {
        set
        {
            _laserBeamBehavior = value;
            if (laserBeamBehavior == LaserBeamBehavior.Off || laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                lineRenderer.enabled = true;
            }
        }
        get
        {
            return _laserBeamBehavior;
        }
    }
    private Vector3 _startPoint;
    private Vector3 _forward;
    private Vector3 _endPoint;
    private bool _hitTarget;
    private LineRenderer lineRenderer;
    private bool insideOtherCone;
    public int layerMask = 1 << 10;

    RaycastHit hit1 = new RaycastHit();

    RaycastHit hit2 = new RaycastHit();

    private Color c;
    bool checkIfInside(Vector3 point)
    {

        hit1 = new RaycastHit();
        hit2 = new RaycastHit();

        Vector3 direction = new Vector3(0, 1, 0);

        if (Physics.Raycast(point, direction, out hit1, 6f, layerMask) &&
            Physics.Raycast(point, -direction, out hit2, 6f, layerMask))
        {
            if (hit1.transform.name == hit2.transform.name) return true;
        }

        return false;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        c = lineRenderer.materials[0].color;
    }

    private void Start()
    {
        if (cursorVisual) cursorVisual.SetActive(false);
        OVRManager.InputFocusAcquired += OnInputFocusAcquired;
        OVRManager.InputFocusLost += OnInputFocusLost;
    }

    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
        _startPoint = start;
        _endPoint = dest;
        _hitTarget = true;
    }

    public override void SetCursorRay(Transform t)
    {
        _startPoint = t.position;
        _forward = t.forward;
        _hitTarget = false;
    }

    private void LateUpdate()
    {

        //turn on laser if trigger is pressed
        SwitchBetweenTypesOfLaserBehaviour();

        //check if the pointer is within the cone of vision of the others
        insideOtherCone = checkIfInside(_endPoint);

        //attach a sticky pointer if the 
        stickyPointerManager(_endPoint);

        // do the standard laser pointer stuff from the ovr
        lineRenderer.SetPosition(0, _startPoint);
        if (_hitTarget)
        {
            lineRenderer.SetPosition(1, _endPoint);
            UpdateLaserBeam(_startPoint, _endPoint);
            if (cursorVisual)
            {
                cursorVisual.transform.position = _endPoint;
                cursorVisual.SetActive(true);
            }
        }
        else
        {
            UpdateLaserBeam(_startPoint, _startPoint + maxLength * _forward);
            lineRenderer.SetPosition(1, _startPoint + maxLength * _forward);
            if (cursorVisual) cursorVisual.SetActive(false);
        }
    }

    private void stickyPointerManager(Vector3 _endPoint) {

        if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            if (stickyPointer == null)
            {
                stickyPointer = Instantiate(stickyPointerPrefab, _endPoint, Quaternion.identity);
            }

            if (!insideOtherCone & !sticky)
            {
                sticky = true;
                stickyPointer.SetActive(true);
                stickyPointer.transform.position = _endPoint;

            }
            else if (!insideOtherCone & sticky)
            {
                stickyPointer.transform.position = _endPoint;

            }
            else if (insideOtherCone & sticky)
            {
                sticky = false;
                stickyPointer.SetActive(false);

            }

           
        }
        else if (laserBeamBehavior == LaserBeamBehavior.Off & sticky) {

            stickyinsideOtherCone = checkIfInside(stickyPointer.transform.position);

            if (stickyinsideOtherCone) {

                sticky = false;
                stickyPointer.SetActive(false);

            }

        }

        object[] data = new object[] { _endPoint, sticky, laserBeamBehavior, PhotonNetwork.NickName };
        gameObject.SendMessage("RaiseStickyPointerChangeEvent", data, SendMessageOptions.DontRequireReceiver);


    }

    private void SwitchBetweenTypesOfLaserBehaviour() {

        //if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) >= 0.5f || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= 0.5f)
       if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= 0.5f )
        {
            laserBeamBehavior = LaserBeamBehavior.OnWhenHitTarget;

        }
        else
        {

            laserBeamBehavior = LaserBeamBehavior.Off;
        }
    }

    // make laser beam a behavior with a prop that enables or disables
    private void UpdateLaserBeam(Vector3 start, Vector3 end)
    {
        object[] data = new object[] { start, end, _hitTarget, laserBeamBehavior, insideOtherCone, PhotonNetwork.NickName };

        gameObject.SendMessage("RaiseLaserChangeEvent", data, SendMessageOptions.DontRequireReceiver);

        if (laserBeamBehavior == LaserBeamBehavior.Off)
        {
            return;
        }
        else if (laserBeamBehavior == LaserBeamBehavior.On)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            UpdateMaterial();
        }
        else if (laserBeamBehavior == LaserBeamBehavior.OnWhenHitTarget)
        {
            if (_hitTarget)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, start);
                    lineRenderer.SetPosition(1, end);
                    UpdateMaterial();
                }
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

    private void UpdateMaterial() {

        if(insideOtherCone) lineRenderer.materials[0].color = Color.green;
        else lineRenderer.materials[0].color = c;
    }

    void OnDisable()
    {
        if (cursorVisual) cursorVisual.SetActive(false);
    }
    public void OnInputFocusLost()
    {
        if (gameObject && gameObject.activeInHierarchy)
        {
            m_restoreOnInputAcquired = true;
            gameObject.SetActive(false);
        }
    }

    public void OnInputFocusAcquired()
    {
        if (m_restoreOnInputAcquired && gameObject)
        {
            m_restoreOnInputAcquired = false;
            gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
        OVRManager.InputFocusLost -= OnInputFocusLost;
    }
}
