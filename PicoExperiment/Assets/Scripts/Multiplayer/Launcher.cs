using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Audio;
using ExitGames.Client.Photon;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GoogleCloudStreamingSpeechToText;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks, IConnectionCallbacks, IMatchmakingCallbacks//, IOnEventCallback
{
    private GameObject localAvatar;

    private GameObject localObserver;

    public GameObject localAvatarsMenu;

    public GameObject cone;

    public bool voiceDebug = true;

    PhotonView photonView;

    public string RoomName;

    private void Awake()
    {
        Resources.LoadAll("ScriptableObjects");

        if (MasterManager.GameSettings.Observer)
        {
            Connect();
        }

    }

    public void Connect()
    {
        
        Debug.Log("[PUN] connecting to server");

        PhotonNetwork.AuthValues = new AuthenticationValues();

        if (MasterManager.GameSettings.Observer) {
            PhotonNetwork.NickName ="Observer";
            PhotonNetwork.AuthValues.UserId = "1";
        }
        else
        {
            PhotonNetwork.NickName = MasterManager.GameSettings.Nickname;                
        }
     
        PhotonNetwork.GameVersion = MasterManager.GameSettings.Gameversion;
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster() {

        Debug.Log("[PUN] connected to server");

        Debug.Log("[PUN] connected with Nickname: " + PhotonNetwork.LocalPlayer.NickName + "\n UserID: " + PhotonNetwork.LocalPlayer.UserId);

        Debug.Log("[PUN] joining room " + MasterManager.GameSettings.RoomName);
        //Debug.Log("[PUN] joining room " + RoomName);

        RoomOptions options = new RoomOptions();
        options.PublishUserId = true;
        //PhotonNetwork.JoinOrCreateRoom(MasterManager.GameSettings.RoomName, options, TypedLobby.Default);
        PhotonNetwork.JoinOrCreateRoom(MasterManager.GameSettings.RoomName, options, TypedLobby.Default);
    }

    void IMatchmakingCallbacks.OnJoinedRoom()
    {

        Debug.Log("[PUN] joined room " + PhotonNetwork.CurrentRoom);

        //if (MasterManager.GameSettings.Observer) ObserverInstantiation();
        //else InstantiateLocalAvatar();

    }

    //AVATAR
    //local avatar
    void InstantiateLocalAvatar()
    {

        //Debug.Log("[PUN] instantiate LocalAvatar");

        //GameObject player = GameObject.Find("PlayerController");

        ////check if an avatar attached to the OVR player controller already exist
        //Transform attachedLocalAvatar = player.transform.FindDeepChild("LocalAvatar");

        //photonView = player.AddComponent<PhotonView>();//Add a photonview to the OVR player controller 
        //PhotonTransformView photonTransformView = player.AddComponent<PhotonTransformView>();//Add a photonTransformView to the OVR player controller 
        //photonTransformView.m_SynchronizeRotation = false;
        //photonView.ObservedComponents = new List<Component>();
        //photonView.ObservedComponents.Add(photonTransformView);
        //photonView.Synchronization = ViewSynchronization.UnreliableOnChange; // set observe option to unreliable onchange

        ////instantiate the local avatr
        //GameObject TrackingSpace = GameObject.Find("TrackingSpace");
        //localAvatar = Instantiate(Resources.Load("LocalAvatar"), TrackingSpace.transform.position, TrackingSpace.transform.rotation, TrackingSpace.transform) as GameObject;
        //PhotonAvatarView photonAvatrView = localAvatar.GetComponent<PhotonAvatarView>();
        //photonAvatrView.photonView = photonView;
        //photonAvatrView.ovrAvatar = localAvatar.GetComponent<OvrAvatar>();
        //photonView.ObservedComponents.Add(photonAvatrView);


        //if (PhotonNetwork.AllocateViewID(photonView))
        //{

        //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        //    {
        //        CachingOption = EventCaching.AddToRoomCache,
        //        Receivers = ReceiverGroup.Others
        //    };

        //    // request to all other clients in the network to create a remote 
        //    PhotonNetwork.RaiseEvent(MasterManager.GameSettings.InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);

        //    //OvrAvatar ovrAvatar = localAvatar.GetComponent<OvrAvatar>();
        //    //ovrAvatar.oculusUserID = MasterManager.GameSettings.UserID;

        //    Debug.Log("[PUN] LocalAvatar instantiatiation triggered now waiting for OVRAvatar to initialize");

        //    OvrAvatar.LocalAvatarInstantiated += LocalAvatarInstantiated;
        //}
        //else
        //{
        //    Debug.LogError("[PUN] Failed instantiate LocalAvatar, Failed to allocate a ViewId.");

        //    Destroy(localAvatar);
        //}
    }

}
