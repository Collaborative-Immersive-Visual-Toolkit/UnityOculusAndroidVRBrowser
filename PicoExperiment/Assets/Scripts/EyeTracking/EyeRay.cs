using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.XR.PXR;

public class EyeRay : MonoBehaviour
{
    public Transform greenpoint;

    private Matrix4x4 matrix;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(EyeRaycast(0.04f));
    }
    IEnumerator EyeRaycast(float steptime)
    {
        while (true)
        {
            if (Camera.main)
            {
                matrix = Matrix4x4.TRS(Camera.main.transform.position, Camera.main.transform.rotation, Vector3.one);
            }
            else
            {
                matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            }
            bool result = (PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 Origin) && PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 Direction));
            PXR_EyeTracking.GetCombineEyeGazePoint(out Origin);
            PXR_EyeTracking.GetCombineEyeGazeVector(out Direction);
            var RealOriginOffset = matrix.MultiplyPoint(Origin);
            var DirectionOffset = matrix.MultiplyVector(Direction);
            if (result)
            {
                Ray ray = new Ray(RealOriginOffset, DirectionOffset);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200))
                {
                    if (hit.collider.transform.name.Equals("zbx"))
                    {
                        Debug.Log("zbx---" + hit.point);
                        greenpoint.gameObject.SetActive(true);
                        greenpoint.DOMove(hit.point, steptime).SetEase(Ease.Linear);
                    }
                }
                else
                {
                    greenpoint.gameObject.SetActive(false);
                }
            }
            yield return new WaitForSeconds(steptime);
        }
    }
}
