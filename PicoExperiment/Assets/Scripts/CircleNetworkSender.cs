using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CircleNetworkSender : MonoBehaviourPun
{
    public void RaiseCircleNewEvent(object[] data)
    {

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.EyeCircleIncome, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);

    }

    
    public void RaiseCircleDestroyEvent(object[] data)
    {

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.EyeCircleDestroy, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);

    }

}
