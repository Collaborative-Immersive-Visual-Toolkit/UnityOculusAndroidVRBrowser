using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class ETMovingManager : MonoBehaviour
{
    Transform _selectObj;
    private Matrix4x4 matrix;

    void Update()
    {
        if (Camera.main)
        {
            matrix = Matrix4x4.TRS(Camera.main.transform.position, Camera.main.transform.rotation, Vector3.one);
        }
        else
        {
            matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        }
        if (PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 origin) && PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 direction))
        {
            Vector3 Origin = origin;
            Vector3 Direction = direction;
            var RealOriginOffset = matrix.MultiplyPoint(Origin);
            var DirectionOffset = matrix.MultiplyVector(Direction);
            Ray ray = new Ray(RealOriginOffset, DirectionOffset);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20))
            {
                if (hit.collider.transform.name.Equals("line")) return;
                if (_selectObj != null && _selectObj != hit.transform)
                {
                    _selectObj.GetComponent<ETMEnity>().StopAnimation();
                    _selectObj = null;
                }
                else if (_selectObj == null)
                {
                    _selectObj = hit.transform;
                    _selectObj.GetComponent<ETMEnity>().PlayAnimation();
                }

            }
            else
            {
                if (_selectObj != null)
                {
                    _selectObj.GetComponent<ETMEnity>().StopAnimation();
                    _selectObj = null;
                }

            }
        }
        else
        {
            if (_selectObj)
            {
                _selectObj.GetComponent<ETMEnity>().StopAnimation();
                _selectObj = null;
            }
        }
    }
}
