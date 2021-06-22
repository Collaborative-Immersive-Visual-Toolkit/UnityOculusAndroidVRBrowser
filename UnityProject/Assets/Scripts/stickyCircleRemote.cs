using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stickyCircleRemote : MonoBehaviour
{

    public float alpha;
    public Vector3[] pos;
    LineRenderer lineRenderer;

    RaycastHit hit1 = new RaycastHit();
    RaycastHit hit2 = new RaycastHit();
    public int layerMask ;

    public GameObject reorient;

    float timeToGo;

    void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            
            timeToGo = Time.fixedTime + 10.0f;

            if (reorient != null)
            {
                if (checkIfInside(GetAveragePoint()))
                {
                    reorient.SetActive(true);
                }
                else
                {
                    reorient.SetActive(false);
                }
            }
        }
    }

    private void Awake()
    {
        layerMask = LayerMask.GetMask("");
        lineRenderer = GetComponent<LineRenderer>();
        reorient = GameObject.Find("ReOrientButtonPlaceholder");
        if (reorient != null) reorient = reorient.GetComponent<ReorientManager>().g;
        timeToGo = Time.fixedTime + 20.0f;
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


    public Vector3 GetAveragePoint()
    {

        Vector3 average = Vector3.zero;

        foreach (Vector3 p in pos) average += p;

        return average / pos.Length;

    }


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
}
