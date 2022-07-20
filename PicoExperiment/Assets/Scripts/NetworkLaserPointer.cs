using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkLaserPointer : MonoBehaviour
{
    public void RaiseLaserPointerChange(object[] data)
    {

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.LaserPointerChange, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }

}
