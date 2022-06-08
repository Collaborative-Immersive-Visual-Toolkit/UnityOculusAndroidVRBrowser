using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.XR.PXR;
using UnityEngine.XR;

public class EyeTrackingButton : MonoBehaviour {

	private Vector3 normalPosition;
	public float targetPositionZ = -50f;
	private float time = 0.15f;
	private Tween hoverTween;
    bool isSelected;
    private Matrix4x4 matrix;

    // Use this for initialization
    void Start () {
        StartCoroutine(SetNormalPos());
	}

    IEnumerator SetNormalPos() {
        yield return new WaitForEndOfFrame();
        normalPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        bool result = PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 origin) && PXR_EyeTracking.GetFoveatedGazeDirection(out Vector3 direction);
        if (result)
        {
            if (Camera.main)
            {
                matrix = Matrix4x4.TRS(Camera.main.transform.position, Camera.main.transform.rotation, Vector3.one);
            }
            else
            {
                matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            }
            PXR_EyeTracking.GetCombineEyeGazePoint(out origin);
            PXR_EyeTracking.GetFoveatedGazeDirection(out direction);
            Vector3 Origin = origin;
            Vector3 Direction = direction;
            var RealOriginOffset = matrix.MultiplyPoint(Origin);
            var DirectionOffset = matrix.MultiplyVector(Direction);
            Ray ray = new Ray(RealOriginOffset, DirectionOffset);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                if (hit.transform.name.Equals(transform.name)) OnHover(true);
                else OnHover(false);
            }
            else OnHover(false);

            bool triggerIsDone;
            if (isSelected && (Input.GetKeyDown(KeyCode.JoystickButton0)||InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton,out triggerIsDone) && triggerIsDone)) {
                OnClick();
            }
        }
    }

    private void OnHover(bool isHover)
	{
		if (!this.enabled)
			return;
		if (isHover) {
            if (transform.localPosition.z != targetPositionZ) {
                isSelected = true;
                hoverTween.Kill();
                hoverTween = this.transform.DOLocalMoveZ(targetPositionZ, time);
            }
            
		} else {
            if (transform.localPosition.z != normalPosition.z)
            {
                isSelected = false;
                hoverTween.Kill();
                hoverTween = this.transform.DOLocalMoveZ(normalPosition.z, time);
            }
		}
	}

	private void OnClick()
	{
		if (!this.enabled)
			return;
		hoverTween.Kill ();
		this.transform.localPosition = normalPosition;
        switch (gameObject.name)
        {
            case "calibration_bt":
                Main.Instance.GoCalibration();
                break;
            case "coordinates_bt":
                Main.Instance.GoCoordinates();
                break;
            case "ET_moving_bt":
                Main.Instance.GoET_moving();
                break;
            case "ET_lighting_bt":
                Main.Instance.GoET_lighting();
                break;
            case "ET_shooting_bt":
                Main.Instance.GoET_shooting();
                break;
            case "gamedemo_bt":
                Main.Instance.GoET_moving();
                break;
            case "mirror_bt":
                Main.Instance.GoMirror();
                break;
            default:
                break;
        }
    }

	void OnDestroy()
	{

	}

}
