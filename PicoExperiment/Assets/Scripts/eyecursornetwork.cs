using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class eyecursornetwork : MonoBehaviour
{
    public void RaiseCursorUpdateEvent(object[] data)
    {

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.CursorUpdate, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }


}
