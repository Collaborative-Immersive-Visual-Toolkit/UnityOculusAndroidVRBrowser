using System.IO;
using UnityEngine;
using System.Collections;
using System;


public class AvatarBehaviourRecorder : MonoBehaviour
{
    StreamWriter writer;

    string line;

    public int frameRate = 25;

    float startTime;
    
    float currentTime;

    Char[] remove = new Char[] { ' ', '(', ')' };

    GameObject cube;

    public string fileName;

    public RemoteAvatarsManager ram;

    string HeadPos ;
    string HeadEAng;
    string ControllerRPos;
    string ControllerREAng;
    string ControllerLPos;
    string ControllerLEAng;
    string PointerPos;
    string PointerVis;
    string StickyCircle;
    string StickyCircleVis;
    string Relocate;

    private void OnEnable()
    {

        //this was creating lag
        /*Time.captureFramerate = frameRate;*/

    }

    void Update()
    {

        if (ram.inputs.Count > 1 && ram.inputs.Count <= 2)
        {
            Record();
        }
        else if (ram.inputs.Count > 2) 
        {
            Debug.Log("there are too many users logged");
        }


        if (writer == null ) return;

        currentTime = Time.unscaledTime - startTime;

        line = currentTime.ToString("F3");

        foreach (inputs i in ram.inputs) {

            HeadPos = i.LocalHead == null ? "null,null,null" : i.LocalHead.position.ToString("F3");
            HeadEAng = i.LocalHead == null ? "null,null,null" : i.LocalHead.eulerAngles.ToString("F3");
            ControllerRPos = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.position.ToString("F3");
            ControllerREAng = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.eulerAngles.ToString("F3");
            ControllerLPos = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.position.ToString("F3");
            ControllerLEAng = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.eulerAngles.ToString("F3");
            PointerPos = i.Pointer._endPoint == Vector3.zero ? "null,null,null" : i.Pointer._endPoint.ToString("F3");
            PointerVis = i.Pointer.insideOtherCone  ? "1" : "0";
            StickyCircle = i.StickyCircle.GetAveragePoint() == Vector3.zero ? "null,null,null" : i.StickyCircle.center.ToString("F3");
            StickyCircleVis = i.StickyCircle.alpha < 1f ? "1" : "0";
            //speaking

            line += "," + HeadPos.Trim(remove) + "," + HeadEAng.Trim(remove) + "," +
                    ControllerRPos.Trim(remove) + "," + ControllerREAng.Trim(remove) + "," +
                    ControllerLPos.Trim(remove) + "," + ControllerLEAng.Trim(remove) + "," +
                    PointerPos.Trim(remove) + "," + StickyCircle.Trim(remove) + "," + StickyCircleVis.Trim(remove);
        }
        
        writer.WriteLine(line);

    }

    private void Record()
    {
        this.NewData(fileName);
    }

    public void NewData(string name) {

        closeWriter();

        string path = Application.dataPath + "\\" + MasterManager.GameSettings.DataFolder +"\\" + name + ".csv";
        writer = new StreamWriter(path, true);

        /*writer.WriteLine("time in s, HeadX, HeadY, HeadZ, HeadEulerX, HeadEulerY, LocalHeadEulerZ, ControllerX, ControllerY, ControllerZ,ControllerEulerX, ControllerEulerY, ControllerEulerZ,PointerHead1X,PointerHead1Y,PointerHead1Z,PointerHand1X,PointerHand1Y,PointerHand1Z," +
           "RemoteHeadX, RemoteHeadY, RemoteHeadZ,RemoteHeadEulerX, RemoteHeadEulerY, RemoteHeadEulerZ, RemoteControllerX, RemoteontrollerY, RemoteControllerZ,ControllerEulerX, RemoteControllerEulerY, RemoteControllerEulerZ,PointerHead2X,PointerHead2Y,PointerHead2Z,PointerHand2X,PointerHand2Y,PointerHand2Z");
        */
        startTime = Time.unscaledTime;

    }

    void OnDisable()
    {
        closeWriter();
    }

    void OnApplicationQuit()
    {
        closeWriter(); 
    }

    public void closeWriter() {

        if (writer != null) writer.Close();

        writer = null;

    }

}
