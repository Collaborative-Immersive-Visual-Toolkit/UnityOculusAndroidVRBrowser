using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class UiHelperManager : MonoBehaviourPun
{

    private cone own;
    private cone[] remote;
    public LaserPointerModified lpm;

    public bool MaterialActive = true;

    public bool StickyCircleActive = true;

    public void toggleOwnCone()
    {

        GameObject obj = GameObject.FindGameObjectWithTag("octagon");

        if (obj != null)
        {

            cone c = obj.GetComponent<cone>();
            c.SwitchVis();

        }

    }

    public void toggleOthersCone()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("remoteavatar");

        foreach (GameObject obj in objs)
        {

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

    public void toggleMaterialsUpdate()
    {

        MaterialActive = lpm.toggleMaterialUpdateReturn();

    }

    public void toggleStickyCircle()
    {

        StickyCircleActive = lpm.toggleStickyReturn();
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
        if (obj.Code == MasterManager.GameSettings.UiHelperSwitch)
        {

            object[] data = (object[])obj.CustomData;

            if ((int)data[0] == 1)
            {
                Load1();
            }
            else if ((int)data[0] == 2)
            {
                Load2();
            }

        }

    }

    private void Load1()
    {

        if (!MaterialActive) MaterialActive = lpm.toggleMaterialUpdateReturn();
        if (!StickyCircleActive) StickyCircleActive = lpm.toggleStickyReturn();
    }

    private void Load2()
    {

        if (MaterialActive) MaterialActive = lpm.toggleMaterialUpdateReturn();
        if (StickyCircleActive) StickyCircleActive = lpm.toggleStickyReturn();

    }
}