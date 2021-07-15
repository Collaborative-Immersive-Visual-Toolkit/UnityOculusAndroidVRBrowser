using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlManager : MonoBehaviour
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

        screenOne.startUrl = mainUrl + urlOne;
        screenTwo.startUrl = mainUrl + urlTwo;
        screenThree.startUrl = mainUrl + urlThree;
        screenFour.startUrl = mainUrl + urlFour;
        screenFive.startUrl = mainUrl + urlFive;
        screenSix.startUrl = mainUrl + urlSix;
        screenSeven.startUrl = mainUrl + urlSeven;
        screenEight.startUrl = mainUrl + urlEight;

        /*screenOne.LoadURL(mainUrl+urlOne);
        screenTwo.LoadURL(mainUrl + urlTwo);
        screenThree.LoadURL(mainUrl + urlThree);
        screenFour.LoadURL(mainUrl + urlFour);
        screenFive.LoadURL(mainUrl + urlFive);
        screenSix.LoadURL(mainUrl + urlSix);
        screenSeven.LoadURL(mainUrl + urlSeven);
        screenEight.LoadURL(mainUrl + urlEight);*/
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

}
