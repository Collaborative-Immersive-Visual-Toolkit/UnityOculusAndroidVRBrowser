using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stickyCircleRemote : MonoBehaviour
{

    public float alpha;
    public Vector3[] pos;
    LineRenderer lineRenderer;

    private void Awake()
    {

        lineRenderer = GetComponent<LineRenderer>();

    }

    public void updateLineRender(Vector3[] circlePos)
    {

        pos = circlePos;
        lineRenderer.positionCount = pos.Length;
        lineRenderer.SetPositions(pos);

    }

    public void UpdateAlpha(float circleAlpha)
    {

        alpha = circleAlpha;
        lineRenderer.materials[0].SetFloat("_Alpha", alpha);

    }

}
