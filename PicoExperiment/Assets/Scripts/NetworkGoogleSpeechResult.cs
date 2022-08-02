using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkGoogleSpeechResult : MonoBehaviour
{
    public string result;
    private string resultinter;

    public bool start = false;
    private bool returned = false;
    public void RaiseSpeechResultChange(string str)
    {
        result = str;
        start = false;
        returned = false;
        object[] data = new object[] { str, PhotonNetwork.NickName };
        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.SpeechResult, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }

    public void startspeechInterim(string str)
    {
        start = true;
    }

    public bool GetSpeechInterim()
    {
        if (!start) return false;
        else
        {
            start = false;

            if (returned) return false;
            else
            {
                returned = true;
                return true;
            }
        }
    }

    public string GetSpeechResult() {
        resultinter = result;
        result = "";
        return resultinter;
    }

    
    

}
