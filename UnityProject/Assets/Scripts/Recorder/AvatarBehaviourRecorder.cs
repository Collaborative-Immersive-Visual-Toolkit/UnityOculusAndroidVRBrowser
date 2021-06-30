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

    private bool recording = false;

    string PlayerPos;
    string HeadPos;
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

    private float nextSampleTime = 0.0f;
    public float sampleFrequency = 0.04f;

    private void OnEnable()
    {

        //this was creating lag
        /*Time.captureFramerate = frameRate;*/
        //sampleFrequency = 1f / frameRate;

    }

    void Update()
    {
        if (Time.unscaledTime > nextSampleTime)
        {
            nextSampleTime += sampleFrequency;

            if (ram.inputs.Count > 1 && ram.inputs.Count <= 2 && !recording)
            {
                Record();
            }
            else if (ram.inputs.Count > 2)
            {
                Debug.Log("there are too many users logged");
            }


            if (writer == null) return;

            currentTime = Time.unscaledTime - startTime;

            line = currentTime.ToString("F3");

            foreach (inputs i in ram.inputs)
            {
                PlayerPos = i.gameObject.transform.position.ToString("F3");
                HeadPos = i.LocalHead == null ? "null,null,null" : i.LocalHead.position.ToString("F3");
                HeadEAng = i.LocalHead == null ? "null,null,null" : i.LocalHead.eulerAngles.ToString("F3");
                ControllerRPos = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.position.ToString("F3");
                ControllerREAng = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.eulerAngles.ToString("F3");
                ControllerLPos = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.position.ToString("F3");
                ControllerLEAng = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.eulerAngles.ToString("F3");
                PointerPos = i.Pointer._endPoint == Vector3.zero ? "null,null,null" : i.Pointer._endPoint.ToString("F3");
                PointerVis = i.Pointer.insideOtherCone && PointerPos  != "null,null,null" ? "1" : "0";
                StickyCircle = i.StickyCircle.GetAveragePoint() == Vector3.zero ? "null,null,null" : i.StickyCircle.center.ToString("F3");
                StickyCircleVis = i.StickyCircle.circleVisible ? "1" : "0";


                line += "," + PlayerPos.Trim(remove)+","+
                              HeadPos.Trim(remove) + "," + HeadEAng.Trim(remove) + "," +
                              ControllerRPos.Trim(remove) + "," + ControllerREAng.Trim(remove) + "," +
                              ControllerLPos.Trim(remove) + "," + ControllerLEAng.Trim(remove) + "," +
                              PointerPos.Trim(remove) + "," + PointerVis.Trim(remove) + "," +
                              StickyCircle.Trim(remove) + "," + StickyCircleVis.Trim(remove);

            }

            writer.WriteLine(line);
        }
    }

    private void Record()
    {
        
            recording = true;
            this.NewData(fileName);
        
    }

    public void NewData(string name) {

        if (writer != null) closeWriter();

        string path = Application.dataPath + "\\" + MasterManager.GameSettings.DataFolder +"\\" + name + ".csv";
        writer = new StreamWriter(path, true);

        writer.WriteLine("time, " +
            "U1PosX, U1PosY, U1PosZ, U1HeadX, U1HeadY, U1HeadZ, U1HeadEulerX, U1HeadEulerY, U1LocalHeadEulerZ, " +
            "U1ControllerRX, U1ControllerRY, U1ControllerRZ, U1ControllerEulerRX, U1ControllerEulerRY, U1ControllerEulerRZ," +
            "U1ControllerLX, U1ControllerLY, U1ControllerLZ, U1ControllerEulerLX, U1ControllerEulerLY, U1ControllerEulerLZ," +
            "U1PointerX, U1PointerY, U1PointerZ, U1PointerVis, U1StickyPointerX, U1StickyPointerY, U1StickyPointerZ, U1StickyPointerVis," +
            "U2PosX, U2PosY, U2PosZ, U2HeadX, U2HeadY, U2HeadZ, U2HeadEulerX, U2HeadEulerY, U2LocalHeadEulerZ, " +
            "U2ControllerRX, U2ControllerRY, U2ControllerRZ, U2ControllerEulerRX, U2ControllerEulerRY, U2ControllerEulerRZ," +
            "U2ControllerLX, U2ControllerLY, U2ControllerLZ, U2ControllerEulerLX, U2ControllerEulerLY, U2ControllerEulerLZ," +
            "U2PointerX, U2PointerY, U2PointerZ, U2PointerVis, U2StickyPointerX, U2StickyPointerY, U2StickyPointerZ, U2StickyPointerVis");

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

        recording = false;

        if (writer != null) writer.Close();

        writer = null;

    }

}
