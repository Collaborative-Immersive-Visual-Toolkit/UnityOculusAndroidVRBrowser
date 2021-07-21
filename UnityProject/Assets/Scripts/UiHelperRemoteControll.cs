using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class UiHelperRemoteControll : MonoBehaviourPun
{



    public void Update()
    {


#if UNITY_EDITOR

        if (Input.GetKeyDown("9"))
        {
            object[] data = new object[] { 1 };
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.UiHelperSwitch, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
            MasterManager.GameSettings.Ui = "CT";
        }
        else if (Input.GetKeyDown("0"))
        {
            object[] data = new object[] { 2 };
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.UiHelperSwitch, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
            MasterManager.GameSettings.Ui = "CB";
        }

#endif


    }


}
