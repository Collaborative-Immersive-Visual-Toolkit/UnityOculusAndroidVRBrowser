using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using Unity.XR.PXR;
using System.IO;
using ExitGames.Client.Photon;

[System.Serializable]
public class ChangeVisualization : UnityEvent<visualization> { }


public class UrlManager : MonoBehaviourPun
{

    public Launcher l;

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

    public List<BoxCollider> BoxColliders;
    public List<GameObject> Underlay;
    public List<PXR_OverLay> Overlay;

    public ChangeVisualization changeVisualization;

    private bool android = false;
    public bool useGeckoView;
    public int currentVis;
    public bool currentHalf;

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

            currentVis = (int)data[0];

            if (currentVis == 1) LoadVis1();
            else if (currentVis == 2) LoadVis2();
            else if (currentVis == 3) LoadVis3();
 
        }
        else if (obj.Code == MasterManager.GameSettings.HalfSwitch) {


            object[] data = (object[])obj.CustomData;

            currentHalf = (bool)data[0];

            hideHalf(currentHalf);

        }
    }

    public void RaiseVisSwitch()
    {
        object[] data = new object[] { currentVis };
        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.VisualizationChange, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }


    public void RaiseHalfSwitch()
    {
        object[] data = new object[] { currentHalf };
        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.HalfSwitch, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);

    }

    private void Start()
    {
        LoadVis1();

#if UNITY_ANDROID && !UNITY_EDITOR
        android = true;
#endif
        if (!android || !useGeckoView) turnOffBrowsers();

    }

    private void Update()
    {


        if (Input.GetKeyDown("q"))
        {
            currentVis = 1;
            Debug.Log("1");
            LoadVis1();
            Debug.Log("2");
            RaiseVisSwitch();
            Debug.Log("3");
        }
        else if (Input.GetKeyDown("w"))
        {
            currentVis = 2;
            LoadVis2();
            RaiseVisSwitch();
        }
        else if (Input.GetKeyDown("e"))
        {
            currentVis = 3;
            LoadVis3();
            RaiseVisSwitch();
        }
        else if (Input.GetKeyDown("s"))
        {
            ToggleHalf();

        }

    }

    public void ToggleHalf() {

        currentHalf = !currentHalf;
        hideHalf(currentHalf);
        RaiseHalfSwitch();

    }

    public void hideHalf(bool whichhalf) {

        for (int i = 0; i < BoxColliders.Count; i++) {

            if (i < 4)
            {

                BoxColliders[i].gameObject.SetActive(whichhalf);

            }
            else {

                BoxColliders[i].gameObject.SetActive(!whichhalf);

            }
        }

    }



    public void LoadVis1() 
    {
        changeVisualization.Invoke(visualization.visualization1);

        LoadVis("");

        Debug.Log("1");
    }

    public void LoadVis2()
    {
        changeVisualization.Invoke(visualization.visualization2);

        LoadVis("_Gender");
        Debug.Log("2");
    }

    public void LoadVis3()
    {
        changeVisualization.Invoke(visualization.visualization3);

        LoadVis("_Third");
        Debug.Log("3");
    }

    public void LoadVis(string postfix)
    {

            if (android && useGeckoView)
            {
                if (!screenOne.enabled)
                {

                    screenOne.startUrl = mainUrl + urlOne + postfix + ".html";
                    screenTwo.startUrl = mainUrl + urlTwo + postfix + ".html";
                    screenThree.startUrl = mainUrl + urlThree + postfix + ".html";
                    screenFour.startUrl = mainUrl + urlFour + postfix + ".html";
                    screenFive.startUrl = mainUrl + urlFive + postfix + ".html";
                    screenSix.startUrl = mainUrl + urlSix + postfix + ".html";
                    screenSeven.startUrl = mainUrl + urlSeven + postfix + ".html";
                    screenEight.startUrl = mainUrl + urlEight + postfix + ".html";

                    screenOne.enabled = true;
                    screenTwo.enabled = true;
                    screenThree.enabled = true;
                    screenFour.enabled = true;
                    screenFive.enabled = true;
                    screenSix.enabled = true;
                    screenSeven.enabled = true;
                    screenEight.enabled = true;
                }
                else
                {

                    screenOne.LoadURL(mainUrl + urlOne + postfix + ".html");
                    screenTwo.LoadURL(mainUrl + urlTwo + postfix + ".html");
                    screenThree.LoadURL(mainUrl + urlThree + postfix + ".html");
                    screenFour.LoadURL(mainUrl + urlFour + postfix + ".html");
                    screenFive.LoadURL(mainUrl + urlFive + postfix + ".html");
                    screenSix.LoadURL(mainUrl + urlSix + postfix + ".html");
                    screenSeven.LoadURL(mainUrl + urlSeven + postfix + ".html");
                    screenEight.LoadURL(mainUrl + urlEight + postfix + ".html");
                }
            }
            else
            {
                changeMaterials(postfix);
                ChangeOverlays(postfix);
            }

        
    }

    private void changeMaterials(string postfix)
    {

        changeMaterial(Underlay[0], urlOne + postfix);
        changeMaterial(Underlay[1], urlTwo + postfix);
        changeMaterial(Underlay[2], urlThree + postfix);
        changeMaterial(Underlay[3], urlFour + postfix);
        changeMaterial(Underlay[4], urlFive + postfix);
        changeMaterial(Underlay[5], urlSix + postfix);
        changeMaterial(Underlay[6], urlSeven + postfix);
        changeMaterial(Underlay[7], urlEight + postfix);

    }

    private void ChangeOverlays(string postfix) {


        changeOverlay(Overlay[0],  urlOne + postfix);
        changeOverlay(Overlay[1],  urlTwo + postfix);
        changeOverlay(Overlay[2],  urlThree + postfix);
        changeOverlay(Overlay[3],  urlFour + postfix);
        changeOverlay(Overlay[4],  urlFive + postfix);
        changeOverlay(Overlay[5],  urlSix + postfix);
        changeOverlay(Overlay[6],  urlSeven + postfix);
        changeOverlay(Overlay[7],  urlEight + postfix);
    }

    private void turnOffBrowsers() {


        screenOne.enabled = false;
        screenTwo.enabled = false;
        screenThree.enabled = false;
        screenFour.enabled = false;
        screenFive.enabled = false;
        screenSix.enabled = false;
        screenSeven.enabled = false;
        screenEight.enabled = false;


    }

    private void changeMaterial(GameObject g, string s) {

        Material[] a = new Material[1];
        var texture = Resources.Load<Texture2D>("Textures/" + s);

        Debug.Log(a);
        Debug.Log(texture);

        a[0] = Resources.Load("Materials/" + s, typeof(Material)) as Material;
        MeshRenderer _Renderer = g.GetComponent<MeshRenderer>();

        _Renderer.materials = a;

        _Renderer.material.SetTexture("_MainTex", texture);
    }

    private void changeOverlay(PXR_OverLay overlay, string s) {

        //Load a Texture 
        var texture = Resources.Load<Texture2D>("Textures/" + s);
        overlay.SetTexture(texture);
        overlay.isExternalAndroidSurface = false;
    }


}

