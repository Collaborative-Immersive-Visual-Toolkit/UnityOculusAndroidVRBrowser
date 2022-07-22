using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEngine.XR;
using Unity.XR.PXR;
[System.Serializable]
public class ChangeVisualization : UnityEvent<visualization> { }


public class UrlManager : MonoBehaviourPun
{


    public Launcher l;

    private BrowserView screenOne;
    private BrowserView screenTwo;
    private BrowserView screenThree;
    private BrowserView screenFour;
    private BrowserView screenFive;
    private BrowserView screenSix;
    private BrowserView screenSeven;
    private BrowserView screenEight;

    public GameObject screenOnePrefab;
    public GameObject screenTwoPrefab;
    public GameObject screenThreePrefab;
    public GameObject screenFourPrefab;
    public GameObject screenFivePrefab;
    public GameObject screenSixPrefab;
    public GameObject screenSevenPrefab;
    public GameObject screenEightPrefab;

    private Dictionary<int, BrowserView> browsers;
    private Dictionary<int, GameObject> browsersPrefabs;

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

    public ChangeVisualization changeVisualization;

    public SearchKeyWordsScreen skws;

    bool clicked = false;

    private void SetDictionaryToBrowser(int index)
    {
        if (index == 0) screenOne = browsers[index];
        if (index == 1) screenTwo = browsers[index];
        if (index == 2) screenThree = browsers[index];
        if (index == 3) screenFour = browsers[index];
        if (index == 4) screenFive = browsers[index];
        if (index == 5) screenSix = browsers[index];
        if (index == 6) screenSeven = browsers[index];
        if (index == 7) screenEight = browsers[index];
    }

    private void SetBoxCollider(int index)
    {
        BoxColliders[index] = browsers[index].transform.parent.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        browsers = new Dictionary<int, BrowserView>() 
        {
            { 0,  screenOne},
            { 1,  screenTwo},
            { 2,  screenThree},
            { 3,  screenFour},
            { 4,  screenFive},
            { 5,  screenSix},
            { 6,  screenSeven},
            { 7,  screenEight}
        };

        browsersPrefabs = new Dictionary<int, GameObject>()
        {
            { 0,  screenOnePrefab},
            { 1,  screenTwoPrefab},
            { 2,  screenThreePrefab},
            { 3,  screenFourPrefab},
            { 4,  screenFivePrefab},
            { 5,  screenSixPrefab},
            { 6,  screenSevenPrefab},
            { 7,  screenEightPrefab}
        };

        
        BoxCollider[] Ar = new BoxCollider[8];
        List<BoxCollider> BoxColliders = new List<BoxCollider>(Ar);

        for (int i=0; i< browsers.Count; i++)
        {
            Debug.Log("Creating Panel " + i.ToString());
            CreateBrowserPanel(i);
        }

        LoadVis1();

    }

    void CreateBrowserPanel(int index)
    {
        GameObject go = GameObject.Instantiate(browsersPrefabs[index], gameObject.transform) as GameObject;
        browsers[index] = go.transform.Find("Browser").gameObject.GetComponent<BrowserView>();
        Debug.Log("Browser View " + browsers[index].ToString());
        SetDictionaryToBrowser(index);
        SetBoxCollider(index);

    }

    private void Update()
    {
        //if (Input.GetKeyDown("1"))
        //{
        //    LoadVis1();

        //}
        //else if (Input.GetKeyDown("2"))
        //{
        //    LoadVis2();

        //}
        //else if (Input.GetKeyDown("3"))
        //{
        //    LoadVis3();

        // }

        if (Input.GetKeyDown("1"))
        {
            RefreshPanel(1);
        }


        bool isDoneRight = false;
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out isDoneRight);
        if (isDoneRight == false) clicked = false;

        if (isDoneRight && clicked == false)
        {
            
            clicked = true;
            Debug.Log("------------------- Grab Clicked");
            int index = GazeLocationToPanelIndex();
            if (index != -1) RefreshPanel(index);
               
        }

    }

    private int GazeLocationToPanelIndex()
    {
        Debug.Log("--------------- Screens " + skws.currentmainScreen.ToString());
        return skws.currentmainScreen;
    }

    private void RefreshPanel(int index)
    {
        /*GameObject go = browsers[index].transform.parent.gameObject;
        CreateBrowserPanel(index);
        Destroy(go);
        Debug.Log("--------------- Refreshed" + index.ToString());*/
        browsers[index].InvokeRefresh();
        browsers[index].LoadURL(browsers[index].startUrl);
    }

    public void LoadVis1() 
    {
        changeVisualization.Invoke(visualization.visualization1);

        if (MasterManager.GameSettings.Observer)
        {

            changeMaterial(screenOne.gameObject, "Materials/" + urlOne );
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo );
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree );
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour );
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive );
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix );
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven );
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight );
        }
        else
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!screenOne.enabled)
            {
                screenOne.startUrl = mainUrl + urlOne + ".html";
                screenTwo.startUrl = mainUrl + urlTwo + ".html";
                screenThree.startUrl = mainUrl + urlThree + ".html";
                screenFour.startUrl = mainUrl + urlFour + ".html";
                screenFive.startUrl = mainUrl + urlFive + ".html";
                screenSix.startUrl = mainUrl + urlSix + ".html";
                screenSeven.startUrl = mainUrl + urlSeven + ".html";
                screenEight.startUrl = mainUrl + urlEight + ".html";

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
                Debug.Log("--------- S1" + screenOne.ToString());
                screenOne.LoadURL(mainUrl + urlOne + ".html");
                Debug.Log("--------- S2" + screenTwo.ToString());
                screenTwo.LoadURL(mainUrl + urlTwo + ".html");
                Debug.Log("--------- S3" + screenThree.ToString());
                screenThree.LoadURL(mainUrl + urlThree + ".html");
                Debug.Log("--------- S4" + screenFour.ToString());
                screenFour.LoadURL(mainUrl + urlFour + ".html");
                Debug.Log("--------- S5" + screenFive.ToString());
                screenFive.LoadURL(mainUrl + urlFive + ".html");
                Debug.Log("--------- S6" + screenSix.ToString());
                screenSix.LoadURL(mainUrl + urlSix + ".html");
                Debug.Log("--------- S7" + screenSeven.ToString());
                screenSeven.LoadURL(mainUrl + urlSeven + ".html");
                Debug.Log("--------- S8" + screenEight.ToString());
                screenEight.LoadURL(mainUrl + urlEight + ".html");
            }
#elif UNITY_EDITOR

            changeMaterial(screenOne.gameObject, "Materials/" + urlOne );
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo );
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree );
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour );
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive );
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix );
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven );
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight );
#endif

        }


    }

    public void LoadVis2()
    {
        changeVisualization.Invoke(visualization.visualization2);

        if (MasterManager.GameSettings.Observer)
        {


            changeMaterial(screenOne.gameObject, "Materials/" + urlOne + "_Gender");
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo + "_Gender");
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree + "_Gender");
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour + "_Gender");
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive + "_Gender");
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix + "_Gender");
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven + "_Gender");
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight + "_Gender");


        }
        else
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!screenOne.enabled)
            {

                screenOne.startUrl = mainUrl + urlOne + "_Gender.html";
                screenTwo.startUrl = mainUrl + urlTwo + "_Gender.html";
                screenThree.startUrl = mainUrl + urlThree + "_Gender.html";
                screenFour.startUrl = mainUrl + urlFour + "_Gender.html";
                screenFive.startUrl = mainUrl + urlFive + "_Gender.html";
                screenSix.startUrl = mainUrl + urlSix + "_Gender.html";
                screenSeven.startUrl = mainUrl + urlSeven + "_Gender.html";
                screenEight.startUrl = mainUrl + urlEight + "_Gender.html";

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

                screenOne.LoadURL(mainUrl + urlOne + "_Gender.html");
                screenTwo.LoadURL(mainUrl + urlTwo + "_Gender.html");
                screenThree.LoadURL(mainUrl + urlThree + "_Gender.html");
                screenFour.LoadURL(mainUrl + urlFour + "_Gender.html");
                screenFive.LoadURL(mainUrl + urlFive + "_Gender.html");
                screenSix.LoadURL(mainUrl + urlSix + "_Gender.html");
                screenSeven.LoadURL(mainUrl + urlSeven + "_Gender.html");
                screenEight.LoadURL(mainUrl + urlEight + "_Gender.html");
            }
#elif UNITY_EDITOR


            changeMaterial(screenOne.gameObject, "Materials/" + urlOne + "_Gender");
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo + "_Gender");
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree + "_Gender");
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour + "_Gender");
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive + "_Gender");
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix + "_Gender");
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven + "_Gender");
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight + "_Gender");
#endif
        }
    }

    public void LoadVis3()
    {
        changeVisualization.Invoke(visualization.visualization3);

        if (MasterManager.GameSettings.Observer)
        {


            changeMaterial(screenOne.gameObject, "Materials/" + urlOne + "_Third");
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo + "_Third");
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree + "_Third");
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour + "_Third");
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive + "_Third");
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix + "_Third");
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven + "_Third");
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight + "_Third");


        }
        else
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!screenOne.enabled)
            {

                screenOne.startUrl = mainUrl + urlOne + "_Third.html";
                screenTwo.startUrl = mainUrl + urlTwo + "_Third.html";
                screenThree.startUrl = mainUrl + urlThree + "_Third.html";
                screenFour.startUrl = mainUrl + urlFour + "_Third.html";
                screenFive.startUrl = mainUrl + urlFive + "_Third.html";
                screenSix.startUrl = mainUrl + urlSix + "_Third.html";
                screenSeven.startUrl = mainUrl + urlSeven + "_Third.html";
                screenEight.startUrl = mainUrl + urlEight + "_Third.html";

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

                screenOne.LoadURL(mainUrl + urlOne + "_Third.html");
                screenTwo.LoadURL(mainUrl + urlTwo + "_Third.html");
                screenThree.LoadURL(mainUrl + urlThree + "_Third.html");
                screenFour.LoadURL(mainUrl + urlFour + "_Third.html");
                screenFive.LoadURL(mainUrl + urlFive + "_Third.html");
                screenSix.LoadURL(mainUrl + urlSix + "_Third.html");
                screenSeven.LoadURL(mainUrl + urlSeven + "_Third.html");
                screenEight.LoadURL(mainUrl + urlEight + "_Third.html");
            }
#elif UNITY_EDITOR

            changeMaterial(screenOne.gameObject, "Materials/" + urlOne + "_Third");
            changeMaterial(screenTwo.gameObject, "Materials/" + urlTwo + "_Third");
            changeMaterial(screenThree.gameObject, "Materials/" + urlThree + "_Third");
            changeMaterial(screenFour.gameObject, "Materials/" + urlFour + "_Third");
            changeMaterial(screenFive.gameObject, "Materials/" + urlFive + "_Third");
            changeMaterial(screenSix.gameObject, "Materials/" + urlSix + "_Third");
            changeMaterial(screenSeven.gameObject, "Materials/" + urlSeven + "_Third");
            changeMaterial(screenEight.gameObject, "Materials/" + urlEight + "_Third");
#endif

        }
    }

    public void changeMaterial(GameObject g, string s) {

        Material[] a = new Material[1];
        a[0] = Resources.Load(s, typeof(Material)) as Material;
        g.GetComponent<MeshRenderer>().materials = a;
    }
}

