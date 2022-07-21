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

public class Launcher : MonoBehaviourPunCallbacks, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback
{
    private Transform localAvatar;

    private GameObject localObserver;

    public GameObject localAvatarsMenu;

    public GameObject cone;

    public bool voiceDebug = true;

    PhotonView photonView;

    public string RoomName;

    private void Awake()
    {
        Resources.LoadAll("ScriptableObjects");

        Connect();

        //if (MasterManager.GameSettings.Observer)
        //{
        //    Connect();
        //}

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

        RoomOptions options = new RoomOptions();
        options.PublishUserId = true;

        PhotonNetwork.JoinOrCreateRoom(MasterManager.GameSettings.RoomName, options, TypedLobby.Default);
    }

    void IMatchmakingCallbacks.OnJoinedRoom()
    {

        Debug.Log("[PUN] joined room " + PhotonNetwork.CurrentRoom);

        if (MasterManager.GameSettings.Observer) ObserverInstantiation();
        else InstantiateLocalAvatar();

    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == MasterManager.GameSettings.InstantiateVrAvatarEventCode)
        {
            InstantiateRemoteAvatar(photonEvent);
        }
        else if (photonEvent.Code == MasterManager.GameSettings.InstantiateObserverEventCode)
        {

            InstantiateRemoteObserver(photonEvent);
        }
    }


    //AVATAR
    //local avatar
    void InstantiateLocalAvatar()
    {

        Debug.Log("[PUN] instantiate LocalAvatar");

        GameObject player = GameObject.Find("PlayerController");

        //check if an avatar attached to the OVR player controller already exist
        localAvatar = player.transform.FindDeepChild("LocalAvatar");        
        
        //Add a photonview and photonTransformView to the player controller 
        photonView = player.AddComponent<PhotonView>();
        PhotonTransformView photonTransformView = player.AddComponent<PhotonTransformView>();
        photonTransformView.m_SynchronizeRotation = false;
        photonView.ObservedComponents = new List<Component>();
        photonView.ObservedComponents.Add(photonTransformView);
        photonView.Synchronization = ViewSynchronization.UnreliableOnChange; 

        // photonAvatrView
        PhotonAvatarView photonAvatrView = localAvatar.gameObject.AddComponent<PhotonAvatarView>();
        photonAvatrView.LocalAvatar = player.GetComponent<CaptureAvatarPackages>();
        photonAvatrView.photonView = photonView;
        photonView.ObservedComponents.Add(photonAvatrView);

        if (PhotonNetwork.AllocateViewID(photonView))
        {

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            // request to all other clients in the network to create a remote 
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log("[PUN] LocalAvatar instantiatiation triggered now waiting for OVRAvatar to initialize");
        }
        else
        {
            Debug.LogError("[PUN] Failed instantiate LocalAvatar, Failed to allocate a ViewId.");
        }

        //PhotonVoiceInstantiationForLocalAvatar();
        //StartCoroutine(PhotonVoiceInstantiationForLocalAvatar());

        ///try the google speech
        GameObject InputType_Microphone =  GameObject.Find("InputType_Microphone");
        CustomStreamingRecognizer customsr = InputType_Microphone.GetComponent<CustomStreamingRecognizer>();
        SearchKeyWordsScreen Search = GameObject.Find("Ellipses").GetComponent<SearchKeyWordsScreen>();
        customsr.onInterimResult.AddListener(Search.getSpeechToText);
        customsr.onFinalResult.AddListener(Search.getSpeechToText);       

    }


    private IEnumerator PhotonVoiceInstantiationForLocalAvatar()
    {

        Debug.Log("[PUN] OVRAvatar completed instantiation of LocalAvatar now we setup voice by adding Speaker,Recorder,VoiceView ");

        //get audiosource from the localavatar       
        AudioSource audioSource = localAvatar.GetComponentInChildren<AudioSource>();

        //////get the ovr 
        //OVRLipSyncContext LipSyncContext = audioSource.gameObject.GetComponent<OVRLipSyncContext>();
        //LipSyncContext.audioSource = audioSource;
        //if (voiceDebug) LipSyncContext.audioLoopback = true; // Don't use mic.
        //else LipSyncContext.audioLoopback = false;
        //LipSyncContext.skipAudioSource = false;

        ////add speaker to the element which holds the audio source 
        Speaker speaker = audioSource.gameObject.AddComponent<Speaker>();

        ////add recorder to the element that has the photonView
        Recorder recorder = photonView.gameObject.AddComponent<Recorder>();
        recorder.DebugEchoMode = true;
        recorder.UseOnAudioFilterRead = false;
        recorder.FrameDuration = Photon.Voice.OpusCodec.FrameDuration.Frame20ms;
        recorder.SamplingRate = POpusCodec.Enums.SamplingRate.Sampling48000;
        recorder.MicrophoneType = Recorder.MicType.Photon;
        recorder.SetAndroidNativeMicrophoneSettings(true, true, true);

        ////add Photonvoice view to the local avatar
        PhotonVoiceView voiceView = photonView.gameObject.AddComponent<PhotonVoiceView>();
        voiceView.RecorderInUse = recorder;
        voiceView.SpeakerInUse = speaker;
        voiceView.SetupDebugSpeaker = true;

        ////start transmission 
        yield return voiceView.RecorderInUse.TransmitEnabled = true;
        voiceView.RecorderInUse.StartRecording();
        voiceView.RecorderInUse.RestartRecording(true);


        ///try the google speech
        //CustomStreamingRecognizer customsr = localAvatar.GetComponentInChildren<CustomStreamingRecognizer>();
        //customsr.enableDebugLogging = true;
        //customsr.Initialize();

    }


    //remote Avatar
    private void InstantiateRemoteAvatar(EventData photonEvent)
    {
        //sender 
        Player player = PhotonNetwork.CurrentRoom.Players[photonEvent.Sender];

        //check if a remote avatar still exist and clean up 
        GameObject oldRemoteAvatar = GameObject.Find(player.NickName);

        //if an avatar is still there reassociate
        if (oldRemoteAvatar != null)
        {
            PhotonView olphotonView = oldRemoteAvatar.GetComponent<PhotonView>();
            olphotonView.ViewID = (int)photonEvent.CustomData;
            return;
        }

        Debug.Log("[PUN] Instantiatate an avatar for user " + player.NickName + "\n with user ID " + player.UserId);

        GameObject remoteAvatar = Instantiate(Resources.Load("Avatars/RemoteAvatar")) as GameObject;

        remoteAvatar.name = player.NickName;

        PhotonView photonView = remoteAvatar.GetComponent<PhotonView>();
        photonView.ViewID = 0; // NEW
        photonView.ViewID = (int)photonEvent.CustomData;

        Debug.Log("[PUN] RemoteAvatar instantiated");

    }


    //OBSERVER
    //local Observer
    void ObserverInstantiation()
    {
        Debug.Log("[PUN] instantiate Local Observer");

        //instantiate the local avatar      
        localObserver = Instantiate(Resources.Load("Avatars/ObserverCamera")) as GameObject;
        photonView = localObserver.GetComponent<PhotonView>();

        //enable observer camera
        localObserver.tag = "MainCamera";


        if (PhotonNetwork.AllocateViewID(photonView))
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.InstantiateObserverEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log("[PUN] Local Observer instantiated");

            //enablePhotonVoice()
            //StartCoroutine(PhotonVoiceInstantiationForLocalObserver());

        }
        else
        {
            Debug.LogError("[PUN] Failed instantiate Local Observer, Failed to allocate a ViewId.");

            Destroy(localObserver);
        }

        //destroy the player controller 
        Destroy(GameObject.Find("PlayerController"));

        ////destroy the player cone
        //GameObject octagon = GameObject.Find("cone");
        //if (octagon != null)
        //{
        //    cone c = octagon.GetComponent<cone>();
        //    c.disabled = true;
        //}

        ////destroy UiHelpers
        DestroyImmediate(GameObject.Find("UIHelpersModified"));

        ////enable observer recorder
        //avatarRecorder.enabled = true;

        //create a folder for saving the data
        //DataFolderCreation();
    }

    private IEnumerator PhotonVoiceInstantiationForLocalObserver()
    {

        Debug.Log("[PUN] setup voice for observer by adding Speaker,Recorder,VoiceView ");

        ////add audio source
        AudioSource audioSource = localObserver.GetComponent<AudioSource>();


        ////add speaker to the element which holds the audio source 
        Speaker speaker = audioSource.gameObject.AddComponent<Speaker>();

        ////add recorder to the element that has the photonView
        Recorder recorder = localObserver.gameObject.AddComponent<Recorder>();
        recorder.DebugEchoMode = false;
        recorder.UseOnAudioFilterRead = true;
        recorder.FrameDuration = Photon.Voice.OpusCodec.FrameDuration.Frame20ms;
        recorder.SamplingRate = POpusCodec.Enums.SamplingRate.Sampling48000;
        recorder.MicrophoneType = Recorder.MicType.Photon;
        recorder.SetAndroidNativeMicrophoneSettings(true, true, true);
        recorder.RestartRecording(true);

        ////add Photonvoice view to the local avatar
        PhotonVoiceView voiceView = localObserver.gameObject.AddComponent<PhotonVoiceView>();
        voiceView.RecorderInUse = recorder;
        voiceView.SpeakerInUse = speaker;
        voiceView.SetupDebugSpeaker = true;

        ////start transmission 
        yield return voiceView.RecorderInUse.TransmitEnabled = true;
        voiceView.RecorderInUse.StartRecording();

        /////try the google speech
        //GameObject empty = new GameObject();
        //CustomStreamingRecognizer customsr = empty.AddComponent<CustomStreamingRecognizer>();
        //customsr.enableDebugLogging = true;
        //customsr.Initialize();

    }

    //Remote observer 
    private void InstantiateRemoteObserver(EventData photonEvent)
    {

        //sender 
        Player player = PhotonNetwork.CurrentRoom.Players[photonEvent.Sender];

        Debug.Log("[PUN] Instantiatate Observer for user " + player.NickName + "\n with user ID " + player.UserId);

        GameObject remoteObserver = Instantiate(Resources.Load("Avatars/RemoteObserver")) as GameObject;

        PhotonView photonView = remoteObserver.GetComponent<PhotonView>();
        photonView.ViewID = (int)photonEvent.CustomData;

        Debug.Log("[PUN] Remote Observer Instantiated");

    }

    private void DataFolderCreation()
    {

        string path = MasterManager.GameSettings.DataFolder + "\\";

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

    }

}
