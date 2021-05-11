using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class HandedInputNetworkEvents : MonoBehaviourPun { 
    public void ChangeOfActiveController(string controller) {

        object[] data =  new object[] { controller, MasterManager.GameSettings.Nickname };

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.ChangeHandInputRemote, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
    
    }
}
