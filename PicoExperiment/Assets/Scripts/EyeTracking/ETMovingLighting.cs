using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.XR.PXR;
public class ETMovingLighting : MonoBehaviour
{
    private Matrix4x4 matrix;

    // Use this for initialization
    void Start()
    {
        //set the delaytime to getEyeTracking;
        StartCoroutine(Lighting(0.05f));
    }

    IEnumerator Lighting(float steptime)
    {
        yield return new WaitForSeconds(0.5f);
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
                GetComponent<Light>().enabled = true;
                Ray ray = new Ray(RealOriginOffset, DirectionOffset);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 20))
                {
                    transform.DOMove(hit.point, steptime).SetEase(Ease.Linear);
                }
                else GetComponent<Light>().enabled = false;
            }
            else
            {
                GetComponent<Light>().enabled = false;
            }
            yield return new WaitForSeconds(steptime);

        }
    }
}
