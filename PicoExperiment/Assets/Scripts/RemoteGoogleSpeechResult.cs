
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;

public class RemoteGoogleSpeechResult : MonoBehaviour
{
    public string _result;
    private string _transitionresult;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
    }

    public string getResult() {


        _transitionresult = _result;

        _result = "";

        return _transitionresult;
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.SpeechResult)
        {

            object[] data = (object[])obj.CustomData;

            if ((string)data[1] == gameObject.transform.parent.gameObject.name)
            {

                _result = (string)data[0];

            }

        }
    }


}
