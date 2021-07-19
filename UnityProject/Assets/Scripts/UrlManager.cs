using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;


public class UrlManager : MonoBehaviourPun
{

    public BrowserView screenOne;
    public BrowserView screenTwo;
    public BrowserView screenThree;
    public BrowserView screenFour;
    public BrowserView screenFive;
    public BrowserView screenSix;
    public BrowserView screenSeven;
    public BrowserView screenEight;

    public string urlOne;
    public string urlTwo;
    public string urlThree;
    public string urlFour;
    public string urlFive;
    public string urlSix;
    public string urlSeven;
    public string urlEight;

    public string mainUrl;

    public void Awake()
    {
        screenOne.enabled = false;
        screenTwo.enabled = false;
        screenThree.enabled = false;
        screenFour.enabled = false;
        screenFive.enabled = false;
        screenSix.enabled = false;
        screenSeven.enabled = false;
        screenEight.enabled = false;

        screenOne.startUrl = mainUrl + urlOne + ".html";
        screenTwo.startUrl = mainUrl + urlTwo + ".html";
        screenThree.startUrl = mainUrl + urlThree + ".html";
        screenFour.startUrl = mainUrl + urlFour + ".html";
        screenFive.startUrl = mainUrl + urlFive + ".html";
        screenSix.startUrl = mainUrl + urlSix + ".html";
        screenSeven.startUrl = mainUrl + urlSeven + ".html";
        screenEight.startUrl = mainUrl + urlEight + ".html";

    }


    public void Start()
    {
       screenOne.enabled = true;
        screenTwo.enabled = true;
        screenThree.enabled = true;
        screenFour.enabled = true;
        screenFive.enabled = true;
        screenSix.enabled = true;
        screenSeven.enabled = true;
        screenEight.enabled = true;
    }

    public void Update()
    {
        /*
        if (MasterManager.GameSettings._devmode)
        {

            if (OVRInput.Get(OVRInput.Button.Three))
            {
                LoadVis1();
            }
            else if (OVRInput.Get(OVRInput.Button.Four)) 
            {
                LoadVis2();
            }
        }*/


#if UNITY_EDITOR

        if (Input.GetKeyDown("1"))
        {
            object[] data = new object[] {1};
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.VisualizationChange, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
        else if (Input.GetKeyDown("2"))
        {
            object[] data = new object[] {2};
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.VisualizationChange, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
        }

#endif

       
    }

    private void LoadVis1() 
    {
        screenOne.LoadURL(mainUrl + urlOne + ".html");
        screenTwo.LoadURL(mainUrl + urlTwo + ".html");
        screenThree.LoadURL(mainUrl + urlThree + ".html");
        screenFour.LoadURL(mainUrl + urlFour + ".html");
        screenFive.LoadURL(mainUrl + urlFive + ".html");
        screenSix.LoadURL(mainUrl + urlSix + ".html");
        screenSeven.LoadURL(mainUrl + urlSeven + ".html");
        screenEight.LoadURL(mainUrl + urlEight + ".html");
    }

    private void LoadVis2()
    {
        screenOne.LoadURL(mainUrl + urlOne + "_Gender.html");
        screenTwo.LoadURL(mainUrl + urlTwo + "_Gender.html");
        screenThree.LoadURL(mainUrl + urlThree + "_Gender.html");
        screenFour.LoadURL(mainUrl + urlFour + "_Gender.html");
        screenFive.LoadURL(mainUrl + urlFive + "_Gender.html");
        screenSix.LoadURL(mainUrl + urlSix + "_Gender.html");
        screenSeven.LoadURL(mainUrl + urlSeven + "_Gender.html");
        screenEight.LoadURL(mainUrl + urlEight + "_Gender.html");
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.VisualizationChange)
        {

            object[] data = (object[])obj.CustomData;

            if ((int)data[0] == 1)
            {
                LoadVis1();

            }
            else if((int)data[0] == 2)
            {
                LoadVis2();
            }

        }

    }

}
