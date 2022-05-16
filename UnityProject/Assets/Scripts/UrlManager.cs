using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.IO;

public class UrlManager : MonoBehaviourPun
{

    public Launcher l;

    public const string keywordsCoordinatesFileName = "keywords_coordinates.json";

    private keywordsCoordinates keyCo;

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

    private void Start()
    {


#if UNITY_EDITOR
        string keywordsCoordinatesPath = Path.Combine(Application.streamingAssetsPath, keywordsCoordinatesFileName);

#elif UNITY_ANDROID

        string keywordsCoordinatesPath =  Path.Combine(Application.persistentDataPath, keywordsCoordinatesFileName);

        //soluation is to manually copy the file to -> adb push <local file> <remote location>

#endif

        if (!File.Exists(keywordsCoordinatesPath))
        {
            Debug.LogError("Could not find " + keywordsCoordinatesPath);
        }
        else {

            string jsonString = File.ReadAllText(keywordsCoordinatesPath);
            keyCo = keywordsCoordinates.CreateFromJSON(jsonString);
        }


    }

    private void Update()
    {

            if (Input.GetKeyDown("1"))
            {
                LoadVis1();
            }
            else if (Input.GetKeyDown("2"))
            {
                LoadVis2();
            }
            else if (Input.GetKeyDown("3"))
            {
                LoadVis3();
            }

    }

    public void LoadVis1() 
    {
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

                screenOne.LoadURL(mainUrl + urlOne + ".html");
                screenTwo.LoadURL(mainUrl + urlTwo + ".html");
                screenThree.LoadURL(mainUrl + urlThree + ".html");
                screenFour.LoadURL(mainUrl + urlFour + ".html");
                screenFive.LoadURL(mainUrl + urlFive + ".html");
                screenSix.LoadURL(mainUrl + urlSix + ".html");
                screenSeven.LoadURL(mainUrl + urlSeven + ".html");
                screenEight.LoadURL(mainUrl + urlEight + ".html");
            }
        }

    }

    public void LoadVis2()
    {


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
        }
    }

    public void LoadVis3()
    {


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
        }
    }

    public void changeMaterial(GameObject g, string s) {

        Material[] a = new Material[1];
        a[0] = Resources.Load(s, typeof(Material)) as Material;
        g.GetComponent<MeshRenderer>().materials = a;
    }
}


[Serializable]
public class keywordsCoordinates
{

    public Page BoxAndWhiskers;
    public Page BoxAndWhiskers_Gender;
    public Page BoxAndWhiskers_Third;

    public Page BoxAndWhiskers2;
    public Page BoxAndWhiskers2_Gender;
    public Page BoxAndWhiskers2_Third;

    public Page Histograms;
    public Page Histograms_Gender;
    public Page Histograms_Third;

    public Page Oscar;
    public Page Oscar_Gender;
    public Page Oscar_Third;

    public Page Scatterplot1;
    public Page Scatterplot1_Gender;
    public Page Scatterplot1_Third;

    public Page Scatterplot2;
    public Page Scatterplot2_Gender;
    public Page Scatterplot2_Third;

    public Page StackBarChart;
    public Page StackBarChart_Gender;
    public Page StackBarChart_Third;

    public static keywordsCoordinates CreateFromJSON(string jsonString)
    {   
        return JsonUtility.FromJson<keywordsCoordinates>(jsonString);

    }

}

[Serializable]
public class Page
{
   public PageElement[] graphs;
   public string[] keys;
   public RectCorners[] rect;
   public PageElement[] scatter;
   public PageElement[] xaxeslabels;
   public PageElement[] xticks;
   public PageElement[] yaxeslabels;
   public PageElement[] yticks;
   public mapofIndexes[] mapofIndexes;
      
}

[Serializable]
public class mapofIndexes
{
    public float[] indexes;
    public string[] keywords;
  
}


[Serializable]
public class PageElement
{
    public float bottom;
    public float left;
    public string name;
    public float right;
    public float top;
}

[Serializable]
public class RectCorners
{
    public float bottom;
    public float left;
    public float right;
    public float top;
}

