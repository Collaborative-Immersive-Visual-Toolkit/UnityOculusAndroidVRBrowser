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



    ReorientManager rom;

    float timeToGo;

    // The target marker.
    public Transform target;
    private Transform Head;

    // Angular speed in radians per sec.
    public float speed = 1.0f;

    private GameObject player;

    void FixedUpdate()
    {
        if (Time.fixedTime >= timeToGo)
        {
            
            timeToGo = Time.fixedTime + .2f;

            if (reorient != null)
            {
                if (!checkIfInside(GetAveragePoint()) && alpha>0.1)
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
        if (reorient != null)
        {

            rom = reorient.GetComponent<ReorientManager>();
            reorient = rom.g;
            rom.scr = this;
        }
        timeToGo = Time.fixedTime + .2f;
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

    public void ReorientAvatar() {

        if(player==null) player = GameObject.Find("OVRPlayerController");

        if (Head == null && player != null) Head = DeepChildSearch(player, "head_JNT");

        if (Head != null && player != null)
        {

            Vector3 localTarget = Head.InverseTransformPoint(GetAveragePoint());
            float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            
            player.transform.RotateAround(Head.position, Vector3.up, angle);
        }




    }




    public Transform DeepChildSearch(GameObject g, string childName)
    {

        Transform child = null;

        for (int i = 0; i < g.transform.childCount; i++)
        {

            Transform currentchild = g.transform.GetChild(i);

            if (currentchild.gameObject.name == childName)
            {

                return currentchild;
            }
            else
            {

                child = DeepChildSearch(currentchild.gameObject, childName);

                if (child != null) return child;
            }

        }

        return null;
    }



}
