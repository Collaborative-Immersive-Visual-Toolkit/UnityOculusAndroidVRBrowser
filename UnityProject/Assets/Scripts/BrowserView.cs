﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class BrowserView : MonoBehaviour 
{
    public event  Action<int> OnProgressUpdate = delegate{  };
    public event Action<string> OnPageLoad = delegate {  }; 

    public Button BackButton;
    public Button ForwardButton;
    public TMP_InputField UrlInputField;
    public TMP_Text ProgressText;

    public RawImage RawImage;
    public Image Image;
    private UserAgent _currentUserAgent = UserAgent.mobile;
    public OVROverlay _overlay;
    public string startUrl;
    private EventTrigger eventTrigger = null;

    /// <summary>
    /// The currently visited url.
    /// </summary>
    public string CurrentUrl = "";

    private int _surfaceWidth = Screen.width;
    private int _surfaceHeight = 1440; 
    private AndroidJavaObject _ajc;
    private Texture2D _imageTexture2D;
    private RectTransform _rawImageRect;
    private static string classString = "com.eyeflite.ian.geckoviewplugin.GeckoViewPLugin";
    private BrowserHistoryType _currentBrowserHistoryType;

    private bool Dragging = false;
    private Vector2 dragStartPosition = new Vector2();
    public enum BrowserHistoryType
    {
        Browser,
        Youtube,
    }


    /// <summary>
    /// There's no normal browser substring because it is anything but the youtube.com/tv one.
    /// </summary>
    private const string YoutubeSubstring = "";

    public static readonly Dictionary<BrowserHistoryType, string> DefaultUrls =
        new Dictionary<BrowserHistoryType, string>()
        {
            {BrowserHistoryType.Browser, YoutubeSubstring},
            {BrowserHistoryType.Youtube,  YoutubeSubstring},
        };
    


    // CALL THIS TO ADD KEYS TO BROWSER    
    public void AppendText(string appendText)
    {
        CallAjc("AddKeys", new object[] {appendText});
    }

    // CHANGE PER YOUR INPUT MODULE SPECIFICS
    public void OnClick(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;

        
        if (!Dragging)
        {
           
            AddTap(eventData);
           
        }

    }

    public void StartDrag(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;

        AddTap(eventData);

        //Debug.Log("StartDrag");
        Dragging = true;
        dragStartPosition = eventData.position;
        //Debug.Log(data);
    }

    public void EndDrag(BaseEventData data)
    {
        //Debug.Log("EndDrag");
        Dragging = false;
        //Debug.Log(data);
    }

    public void Drag(BaseEventData data)
    {
        PointerEventData eventData = (PointerEventData)data;

        Vector2 currentPosition = PointerPositionInRect(eventData.position);

        float verticalDistance = dragStartPosition.y - currentPosition.y;

        if (verticalDistance > 50 || verticalDistance < -50 )
        {
            dragStartPosition = currentPosition;
            InvokeScrollY((int)verticalDistance);
        }


    }

    public Vector2 PointerPositionInRect(Vector3 screenPoint)
    {

        Camera thisCamera = Camera.main;
        Debug.Assert(thisCamera.name == "CenterEyeAnchor");
        Vector2 positionInRect = new Vector2();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rawImageRect,
            screenPoint, thisCamera, out positionInRect);
        // Debug.Log("main camera is: " + Camera.main.name);

        // Take care of the pivots and their effect on position
        positionInRect.x += _rawImageRect.pivot.x * _rawImageRect.rect.width;
        positionInRect.y += (_rawImageRect.pivot.y * _rawImageRect.rect.height);

        Debug.Assert(Math.Abs(_rawImageRect.pivot.y) > 0);
        // Change coordinate system 
        positionInRect.y += -_rawImageRect.rect.height;
        positionInRect.y = Math.Abs(positionInRect.y);
        // Debug.Log(positionInRect);

        // get the screen dimensions and divide them by the rectangle's screen dimensions for scaling
        float screenWidth = _surfaceWidth; //rect.width;
        float screenHeight = _surfaceHeight; //rect.height;

        float xScale = screenWidth / _rawImageRect.rect.width; // rectWidthInScreen;
        float yScale = screenHeight / _rawImageRect.rect.height; // rectHeightInScreen;

        Vector2 positionInWebView = new Vector2(positionInRect.x * xScale, positionInRect.y * yScale);

        return positionInWebView;
    }


    //TODO: show your keyboard here
    public void ChangeKeyboardVisiblity(bool show)
    {
        
    }

    /// <summary>
    /// Calls the plugin to get png bytes from the surface.
    /// </summary>
    /// <returns>PNG byte array</returns>
    public byte[] TakePngScreenShot(int width=0, int height=0, int quality=100)
    {
        if (width == 0 || width> _surfaceWidth) width = _surfaceWidth;
        if (height == 0 || height>_surfaceHeight) height = _surfaceHeight;
            
        AndroidJavaObject jo = _ajc.Call<AndroidJavaObject>("GetSurfaceBytesBuffer",
            new object[]{width, height, quality});
        AndroidJavaObject bufferObject = jo.Get<AndroidJavaObject>("Buffer");
        byte[] bytes = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(bufferObject.GetRawObject());
        return bytes;
    }

    #region Button Interactions
      

    // PLUGIN METHODS:

    public void InvokeStopLoading()
    {
        CallAjc("StopWebview", new object[] { });
    }
    
    public void LoadURLIfNew(string url)
    {
        if (url!=CurrentUrl)
            LoadURL(url);
    }
   
    public void LoadURL(string url)
    {
        var rand = new System.Random();

        //add a query to force cache refreshing
        url = url +"?"+ rand.Next(1000);
        SetInputFieldUrl(url);
        CallAjc("LoadURL", new object[] {url});

    }



    public void InvokeGoBack()
    {
        CallAjc("GoBack", new object[] { });
    }

    public void InvokeGoForward()
    {
        CallAjc("GoForward", new object[] { });
    }


    // SCROLLING:
    
    private int _scrollBy = 50;
    public void InvokeScrollUp(float multiplier = 1f)
    {
        int scrollBy = (int) (_scrollBy * multiplier);
        CallAjc("ScrollByXorY", new object[] { 0, -scrollBy });
    //            int javaCode = 19;
    }

    public void InvokeScrollDown(float multiplier = 1f)
    {
        int scrollBy = (int) (_scrollBy * multiplier);
        CallAjc("ScrollByXorY", new object[] { 0, scrollBy });
    //            int javaCode = 20;
    }

    public void InvokeScrollLeft()
    {
        CallAjc("ScrollByXorY", new object[] { -50, 0 });
    }

    public void InvokeScrollRight()
    {
        CallAjc("ScrollByXorY", new object[] { 50, 0 });
    }

    public void InvokeScrollPageUp()
    {
        int javaCode = 92;
        CallAjc("AddKeyCode", new object[] { javaCode });
    }

    public void InvokeScrollPageDown()
    {
        int javaCode = 93;
        CallAjc("AddKeyCode", new object[] { javaCode });
    }

    public void InvokeScrollY(int value)
    {
        //Debug.Log(value);
        //CallAjc("ScrollByXorY", new object[] { 0, value });

        if (value > 0) InvokeScrollPageUp();
        else InvokeScrollPageDown();
    }

    public void InvokeScrollX(int value)
    {

        CallAjc("ScrollByXorY", new object[] { value, 0 });
    }

    public void InvokeScrollAllTheWay(bool down)
    {
        int javaCode = 122; // home key
        
        if (down)
            javaCode =123; // end key
        
        CallAjc("AddKeyCode", new object[] { javaCode });

    }

    public enum UserAgent
    {
        mobile,
        desktop,
        vr
    }

    private void SetYoutubeUserAgentOverride()
    {
        string userAgent = "Mozilla/5.0 (CrKey armv7l 1.5.16041)";
        CallAjc("OverrideSessionUserAgent", new object[] { BrowserHistoryType.Youtube.ToString().ToLower(), userAgent});
    }

    private void SetBrowserUserAgent(UserAgent type, bool reload)
    {
        CallAjc("SetBrowserUserAgent", new object[] { type.ToString("G"), reload});
    }


    /// <summary>
    /// Current hack to zoom out in desktop view.
    /// reloads in desktop view and then adds a double tap at 100% progress
    /// </summary>
    public void InvokeToggleUserAgentThenZoomOut()
    {
        InvokeToggleUserAgent();
        if (_currentUserAgent == UserAgent.desktop)
            OnProgressUpdate += DoubleTapAt100Progress;
    }

            
    private void DoubleTapAt100Progress(int progress)
    {
        if (progress< 100)
            return; 
        
        //   Debug.Log("adding double tap");
        OnProgressUpdate -= DoubleTapAt100Progress;
        Vector2 position = new Vector2(0,0);
        // TODO:
    //            AddTapDirectly(position);
    //            AddTapDirectly(position);
    }


    private void InvokeToggleUserAgent()
    {
        if (_currentUserAgent== UserAgent.mobile)
            _currentUserAgent = UserAgent.desktop;

        else if (_currentUserAgent == UserAgent.desktop)
            _currentUserAgent = UserAgent.mobile;

        SetBrowserUserAgent(_currentUserAgent, true);
    }

   
    public void InvokeRefresh()
    {
        CallAjc("Refresh", new object[] { });
    }

    
    // KEYBOARD INPUT:
    
    public void Backspace()
    {
        int javaCode = 67;
        CallAjc("AddKeyCode", new object[] { javaCode });
    }

    public void MoveCaretLeft()
    {
        int javaCode = 21;
        CallAjc("AddKeyCode", new object[] { javaCode });
    }

    public void MoveCaretRight()
    {
        int javaCode = 22;
        CallAjc("AddKeyCode", new object[] { javaCode });

    }

    public void MoveCaretUp()
    {
        int javaCode = 19;
        CallAjc("AddKeyCode", new object[] { javaCode });
    }

    public void MoveCaretDown()
    {
        int javaCode = 20;
        CallAjc("AddKeyCode", new object[] { javaCode });
    }


    public void Escape()
    {
        // youtube notes: b_key= 30, backspace = 67, media close = 128 ,menu=82, escape=111, back = 4, tv_contents_menu = 256
        int code = 111; 
        CallAjc("AddKeyCode", new object[] {code});
    }

    public void Enter()
    {
        // j,k,l all seem like they can be used to pause/play a video
        // youtube notes: numpad_enter=160, enter =66, space = 62
        int code = 66;
        CallAjc("AddKeyCode", new object[] {code});
    }

    public void AddKeyCodeWithModifier(int keyCode, int modifier)
    {
        CallAjc("AddKeyCode", new object[] {keyCode, modifier});
    }


    // SESSIONS:
    
    public void DeactivateGeckoSession()
    {
        CallAjc("ActivateSession", new object[] {false});
    }

    public void ActivateGeckoSession()
    {
        CallAjc("ActivateSession", new object[] {true});
    }
    
    public void SwitchSessionTo( BrowserHistoryType browserHistoryType)
    {
        Dictionary<BrowserHistoryType, string> DefaultUrls =
        new Dictionary<BrowserHistoryType, string>()
        {
                {BrowserHistoryType.Browser, startUrl},
                {BrowserHistoryType.Youtube,  startUrl},
        };

        CallAjc("ChangeSessionAndLoadUrlIfUnopened", new object[]{browserHistoryType.ToString().ToLower(), DefaultUrls[browserHistoryType]});
        _currentBrowserHistoryType = browserHistoryType;

    }

    
    
#endregion

    #region Initialization
    private void Awake()
    {

#if UNITY_EDITOR

            _overlay.enabled = false;


#elif UNITY_ANDROID
        UnityThread.initUnityThread();
        //RawImage.GetComponent<Button>().onClick.AddListener(OnClick);
        if(RawImage!=null) _rawImageRect = RawImage.GetComponent<RectTransform>();
        else if(Image != null) _rawImageRect = Image.GetComponent<RectTransform>();

        InitializeAndroidPlugin();

#endif
    }

    private void Update()
    {
       // if (Dragging) Drag();
    }


    private void OnGeckoViewReady()
    {
       


    // call override to allow navigating to youtube tv
    // SetYoutubeUserAgentOverride();
        SwitchSessionTo(BrowserHistoryType.Browser);
        ActivateGeckoSession();
        //LoadURL(startUrl);
    }

  
    /// <summary>
        /// Sets the surface for our GeckoView plugin when the OVROverlay is ready and the plugin is initialized
        /// Also we sign up for the sign in event so we can load the first webpage/initialize history
        /// when the user signs ins
        /// </summary>
        /// <returns>The start function</returns>
        private IEnumerator Start()
        {

          

#if UNITY_EDITOR
            yield break;
#endif
            // Wait for surface to be available.
            while (_overlay.externalSurfaceObject == IntPtr.Zero || _ajc == null)
            {
                yield return null;
            }
            Debug.Log("Browser Start!");
            var pluginClass = _ajc.GetRawClass();
            var pluginObject = _ajc.GetRawObject();
            var surfaceMethodId = AndroidJNI.GetMethodID(pluginClass, "PassSurface", "(Landroid/view/Surface;)V");

            AndroidJNI.CallVoidMethod(pluginObject, surfaceMethodId,
                new jvalue[] { new jvalue { l = _overlay.externalSurfaceObject } });

            //try
            //{
            //    AndroidJNI.DeleteLocalRef(pluginClass);
            //    AndroidJNI.DeleteLocalRef(pluginObject);
            //}
            //catch 
            //{
            //}

        OnGeckoViewReady();
        }

       
        /// <summary>
        /// Initializes the android plugin.by creating our GeckoView instance
        /// and passing it the proper width/height and user agent
        /// also passes a reference to the callback object
        /// </summary>
        public void InitializeAndroidPlugin()
        {
#if UNITY_EDITOR
            return;
#endif
            // testing new values 
            _surfaceWidth = _overlay.externalSurfaceWidth; //(int) _rawImage.rectTransform.rect.width;
            _surfaceHeight = _overlay.externalSurfaceHeight; //(int) (_rawImage.rectTransform.rect.height);

            Debug.Log("_surfaceWidth "+ _surfaceWidth);
            Debug.Log("_surfaceHeight " + _surfaceHeight);

            var tempAjc = new AndroidJavaClass(classString);
            _ajc = tempAjc.CallStatic<AndroidJavaObject>("CreateInstance",
                new object[] {_overlay.externalSurfaceWidth, _overlay.externalSurfaceHeight, UserAgent.mobile.ToString("G") });
            UnityInterface androidPluginCallback = new UnityInterface {BrowserView = this};
            _ajc.Call("SetUnityBitmapCallback", androidPluginCallback);
        }
    
    
    
    
//    // Browser view must have pivot point at (0.5,0.5)



#endregion
     
#region AndroidInterface
    
    // method to to tap in the right coords despite difference in scaling
    private void AddTap(PointerEventData eventData)
    {

        Vector2 positionInWebView = PointerPositionInRect(eventData.position);

        object[] data = new object[] { positionInWebView.x, positionInWebView.y  };

        // if we're within the bounds of the rectangle
        if (_ajc!= null)
        {
            CallAjc("AddLongTap", data);
        }

        object[] data2 = new object[] { positionInWebView.x, positionInWebView.y, gameObject.transform.parent.gameObject.name};

        // we also send a message to the rest of the components in the gameobject 
        gameObject.SendMessage("RaiseAddLongTapNetworkEvent", data2);

    }


    public void ReceivedAddLongTapNetworkEvent( object[] data )
    {

        Debug.Log("ReceivedAddLongTapNetworkEvent");

        object[] data2 = new object[] { (float)data[0], (float)data[1] };

        // if we're within the bounds of the rectangle
        if (_ajc != null)
        {
            CallAjc("AddLongTap", data2);
        }
        

    }


    // before calling anything to theplugin, make sure it has drawing enabled
    private void CallAjc(string methodName, object[] paramies)
    {
        if (_ajc != null)
        {
            //_ajc.Call("SetShouldDraw", true);
            _ajc.Call(methodName,paramies);
            Debug.Log("CallAjc");
            Debug.Log(paramies);
        }
    }


    public void SetInputFieldUrl(string url)
    {

        // don't set the url to the same thing multiple times, it may overwrite what the user has typed
        if (url == CurrentUrl) 
            return;
        
        UrlInputField.text = url;
        CurrentUrl = url;
    }

    public void UpdateProgress(int progress)
    {
        OnProgressUpdate?.Invoke(progress);
        ProgressText.text = progress.ToString();
  
    }

    public void CanGoBack(bool canGoBack)
    {
        // only for browser
        if (_currentBrowserHistoryType != BrowserHistoryType.Browser)
            return;
            
        BackButton.enabled = canGoBack;
    }

    public void CanGoForward(bool canGoForward)
    {
        // only for browser
        if (_currentBrowserHistoryType != BrowserHistoryType.Browser)
            return;
            
        ForwardButton.enabled = canGoForward;
    }

    private string _filePathToReadWhenComplete = "";
    public void PrepareReadFile(string path, string directory, string fileName, string url)
    {
        Debug.Log("download file called in plugin");
        
        path = Path.Combine(path, directory);
        Debug.Log("abs path is: " + path);
        _filePathToReadWhenComplete = Path.Combine(path, fileName);
    }

    public void ReadFile()
    {
        Debug.Log("abs path with filename is: " + _filePathToReadWhenComplete);
        string fileContents = System.IO.File.ReadAllText(_filePathToReadWhenComplete);
        Debug.Log("file contents are: " + fileContents);

    }
     
    public void OnPageVisited(string url, string lastUrl)
    {
        Debug.Log("on page visited: " + url );
        Debug.Log("last page visited: " + lastUrl );
        // NOTE: seems like this method is used too often for random sites of no importance
        OnPageLoad?.Invoke(url);
    }
    
    // used for autofill text, if the input field target changes
    public void RestartInput()
    {
        
    }

    // reload the last url
    public void OnSessionCrash()
    {
        Debug.Log("Attempting browser restart" );
        //CallAjc("RestartBrowser", new object[]{("trash", true)});   
    }

#endregion
 
    private bool ValidHttpURL(string s, out Uri resultURI)
    {        
        bool returnVal = false;
        
        if (!Regex.IsMatch(s, @"^https?:\/\/", RegexOptions.IgnoreCase))
            s = "http://" + s;
        
        if (Uri.TryCreate(s, UriKind.Absolute, out resultURI))
            returnVal = (resultURI.Scheme == Uri.UriSchemeHttp || 
                         resultURI.Scheme == Uri.UriSchemeHttps);
        
        if (!s.Contains(".") || s.Contains(" "))
        {
            returnVal = false;
        }
        
        if (!Uri.IsWellFormedUriString(s, UriKind.Absolute)) 
            returnVal = false;   
        


        return returnVal;
    }


}

  class UnityInterface : AndroidJavaProxy
    {
        public UnityInterface() : base("com.eyeflite.ian.geckoviewplugin.UnityInterface")
        {
        }

        public BrowserView BrowserView;


        public void updateProgress(int progress)
        {
            //Debug.Log("update progress called ");
            UnityThread.executeInUpdate(() => BrowserView.UpdateProgress(progress));

        }

        // TODO: implement different sessions for youtube vs. browser
        public void CanGoBack(bool able)
        {
//            Debug.Log("Gecko Says we can go back");
            UnityThread.executeInUpdate(() => BrowserView.CanGoBack(able));
        }
        
        // TODO: implement different sessions for youtube vs. browser
        public void CanGoForward(bool able)
        {
//            Debug.Log("Gecko Says we can go forward");
            UnityThread.executeInUpdate(() => BrowserView.CanGoForward(able));
        }

        public void updateURL(string url)
        {

//        Debug.Log("update url called! " + url);
            UnityThread.executeInUpdate(() => BrowserView.SetInputFieldUrl(url));
        }

        public void OnPageVisited(string url, string lastUrl)
        {
            UnityThread.executeInUpdate(() => BrowserView.OnPageVisited(url, lastUrl));

        }


        public void ChangeKeyboardVisiblity(bool show)
        {
            UnityThread.executeInUpdate(() => BrowserView.ChangeKeyboardVisiblity(show));
        }

        public void RestartInput()
        {
            UnityThread.executeInUpdate(BrowserView.RestartInput);
        }

        public void OnSessionCrash()
        {
            UnityThread.executeInUpdate(BrowserView.OnSessionCrash);
        }



    }
