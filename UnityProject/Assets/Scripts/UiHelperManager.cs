using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
public class UiHelperManager : MonoBehaviour
{

    private cone own;
    private cone[] remote;
    public LaserPointerModified lpm;

    public bool MaterialActive = true;

    public bool StickyCircleActive = true;

    public void toggleOwnCone() {

        GameObject obj = GameObject.FindGameObjectWithTag("octagon");

        if (obj != null) {

            cone c = obj.GetComponent<cone>();
            c.SwitchVis();

        }

    }

    public void toggleOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("remoteavatar");

        foreach (GameObject obj in objs) {

            RemoteVisualCone rvc = obj.GetComponentInChildren<RemoteVisualCone>();
            if (rvc != null)
            {
                rvc.SwitchVis();
                return;
            }
            
            cone c = obj.GetComponentInChildren<cone>();
            if (c != null) c.SwitchVis();
        }
    }

    public void toggleMaterialsUpdate() {

        MaterialActive = lpm.toggleMaterialUpdateReturn();

    }

    public void toggleStickyCircle() {

        StickyCircleActive = lpm.toggleStickyReturn();
    }



}
