using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkGoogleSpeechResult : MonoBehaviour
{
    public void RaiseSpeechResultChange(string str)
    {
        object[] data = new object[] { str, PhotonNetwork.NickName };
        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.SpeechResult, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }

}
