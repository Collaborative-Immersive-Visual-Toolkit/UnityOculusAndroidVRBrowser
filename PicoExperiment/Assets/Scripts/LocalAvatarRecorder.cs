using System.IO;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using GoogleCloudStreamingSpeechToText;

public class LocalAvatarRecorder : MonoBehaviour
{
    StreamWriter writer;

    private bool recording = false;

    public string fileName;

    string path;

    string line;

    float startTime;

    float currentTime;

    private float nextSampleTime = 0.0f;
    public float sampleFrequency = 0.04f;

    public int frameRate = 25;

    Char[] remove = new Char[] { ' ', '(', ')' };

    string PlayerPos;
    string HeadPos;
    string HeadForward;
    string HeadUp;
    string HeadCone;
    string ControllerRPos;
    string ControllerREAng;
    string ControllerLPos;
    string ControllerLEAng;
    string PointerPosLeft;
    string PointerPosRight;
    string PointerPosLeftUV;
    string PointerPosRightUV;
    string LeftEyeDirection;
    string RightEyeDirection;
    string LeftEyePos;
    string RightEyePos;
    string Gaze;
    string GazeUV;
    string isSpeaking;
    string TranscriptStartTime;
    string Transcript;
    string Condition;
    string Visualization;
    string insightRecording;
    string foundKey;

    public Transform LocalHead;
    public Transform PlayerPosition;
    public cone Cone;
    public Transform LeftEyeTrasnform;
    public Transform RightEyeTransform;
    public eyecursor GazeCursor;
    public Transform ControllerRight;
    public Transform ControllerLeft;
    public RayCasterCustom PointerLeft;
    public RayCasterCustom PointerRight;
    public NetworkGoogleSpeechResult SpeechResult;
    public speaking speech;
    public switchCondition condition;
    public UrlManager urlmanager;
    public InsightRecording insight;
    public SearchKeyWordsScreen search;

    void Update()
    {

        if (Time.unscaledTime > nextSampleTime)
        {
            nextSampleTime += sampleFrequency;

            if (!recording) Record();

            if (writer == null) return;

            currentTime = Time.unscaledTime - startTime;

            line = currentTime.ToString("F3");

            PlayerPos = PlayerPosition.position.ToString("F3");

            HeadPos = LocalHead == null ? "null,null,null" : LocalHead.position.ToString("F3");
            HeadForward = LocalHead == null ? "null,null,null" : LocalHead.forward.ToString("F3");
            HeadUp = LocalHead == null ? "null,null,null" : LocalHead.up.ToString("F3");

            HeadCone = Cone == null ? "null" : Vector2ArrayToString(Cone.returnUVpositions());

            LeftEyePos = LeftEyeTrasnform == null ? "null,null,null" : LeftEyeTrasnform.position.ToString("F3");
            RightEyePos = RightEyeTransform == null ? "null,null,null" : RightEyeTransform.position.ToString("F3");
            LeftEyeDirection = LeftEyeTrasnform == null ? "null,null,null" : LeftEyeTrasnform.forward.ToString("F3");
            RightEyeDirection = RightEyeTransform == null ? "null,null,null" : RightEyeTransform.forward.ToString("F3");
            
            Gaze = GazeCursor.currentPoint == Vector3.zero ? "null,null,null" : GazeCursor.currentPoint.ToString("F3");
            GazeUV = GazeCursor.currentPointUV == Vector2.zero ? "null,null" : GazeCursor.currentPointUV.ToString("F3");

            ControllerRPos = ControllerRight == null ? "null,null,null" : ControllerRight.position.ToString("F3");
            ControllerREAng = ControllerRight == null ? "null,null,null" : ControllerRight.eulerAngles.ToString("F3");
            ControllerLPos = ControllerLeft == null ? "null,null,null" : ControllerLeft.position.ToString("F3");
            ControllerLEAng = ControllerLeft == null ? "null,null,null" : ControllerLeft.eulerAngles.ToString("F3");

            PointerPosLeft = PointerLeft.point == Vector3.zero ? "null,null,null" : PointerLeft.point.ToString("F3");
            PointerPosRight = PointerRight.point == Vector3.zero ? "null,null,null" : PointerRight.point.ToString("F3");
            PointerPosLeftUV = PointerLeft.point == Vector3.zero ? "null,null" : PointerLeft.pointUV.ToString("F3");
            PointerPosRightUV = PointerRight.point == Vector3.zero ? "null,null" : PointerRight.pointUV.ToString("F3");

            TranscriptStartTime = SpeechResult.GetSpeechInterim() == true ? currentTime.ToString("F3") : "";
            Transcript = SpeechResult.GetSpeechResult();
            isSpeaking = speech.isSpeaking.ToString();
            Condition = condition.condition.ToString();
            Visualization = urlmanager.currentVis.ToString();
            insightRecording = insight.sphere.activeSelf == true ? "1" : "";
            foundKey = search.getfoundkeywords();

            line += "," + PlayerPos.Trim(remove) + "," +
                        HeadPos.Trim(remove) + "," + HeadForward.Trim(remove) + "," + HeadUp.Trim(remove) + "," + HeadCone.Trim(remove) + "," +
                        LeftEyePos.Trim(remove) + "," + RightEyePos.Trim(remove) + "," +
                        LeftEyeDirection.Trim(remove) + "," + RightEyeDirection.Trim(remove) + "," +
                        Gaze.Trim(remove) + "," + GazeUV.Trim(remove) + "," +
                        ControllerRPos.Trim(remove) + "," + ControllerREAng.Trim(remove) + "," +
                        ControllerLPos.Trim(remove) + "," + ControllerLEAng.Trim(remove) + "," +
                        PointerPosLeft.Trim(remove) + "," + PointerPosRight.Trim(remove) + "," +
                        PointerPosLeftUV.Trim(remove) + "," + PointerPosRightUV.Trim(remove) + "," +
                        TranscriptStartTime.Trim(remove) + "," + Transcript + "," + 
                        isSpeaking + "," + Condition + "," + Visualization + "," + insightRecording + "," + foundKey;
            
            writer.WriteLine(line);
        }

    }

    public void RemoteAvatarEnter() {

        nextSampleTime += sampleFrequency;

        if (writer == null) return;

        currentTime = Time.unscaledTime - startTime;

        line = currentTime.ToString("F3");

        line += "two users now";

        writer.WriteLine(line);
    }

    private void Record()
    {

        recording = true;
        this.NewData(fileName + DateTime.Now.DayOfYear + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second);

    }

    public void NewData(string name)
    {

        if (writer != null) closeWriter();

        path = Application.persistentDataPath + "/" + name + ".csv";
        writer = new StreamWriter(path, true);

        Debug.Log("[LocalAvatarRecorder] file saved in " + path);

        string header = "timestamp," + 
        "U1PosX, U1PosY, U1PosZ," + 
        "U1HeadPosX, U1HeadPosY, U1HeadPosZ," + 
        "U1HeadForwardX, U1HeadForwardY, U1HeadForwardZ," + 
        "U1HeadUpX, U1HeadUpY, U1HeadUpZ," + 
        "U1HeadCone," +
        "U1LeftEyePosX,U1LeftEyePosY,U1LeftEyePosZ," +
        "U1RightEyePosX,U1RightEyePosY,U1RightEyePosZ," +
        "U1LeftEyeVecX,U1LeftEyeVecY,U1LeftEyeVecZ," + 
        "U1RightEyeVecX,U1RightEyeVecY,U1RightEyeVecZ," +
        "U1GazeX,U1GazeY,U1GazeZ,U1GazeU,U1GazeV," + 
        "U1ControllerRPosX, U1ControllerRPosY, U1ControllerRPosZ," + 
        "U1ControllerREAngX, U1ControllerREAngY,  U1ControllerREAngZ," + 
        "U1ControllerLPosX, U1ControllerLPosY, U1ControllerLPosZ," + 
        "U1ControllerLEAngX, U1ControllerLEAngY,  U1ControllerLEAngZ," + 
        "U1PointerLeftX, U1PointerLeftY,  U1PointerLeftZ," + 
        "U1PointerRightX, U1PointerRightY,  U1PointerRightZ," +
        "U1PointerLeftU, U1PointerLeftV," +
        "U1PointerRightU, U1PointerRightV," +
        "TranscriptStartTime, Transcript, IsSpeaking, Condition, Visualization, InsightRecording, FoundKey";

        writer.WriteLine(header);

        startTime = Time.unscaledTime;

    }

    public void closeWriter()
    {

        recording = false;

        if (writer != null) writer.Close();

        writer = null;

    }

    private static string Vector2ArrayToString(Vector2[] array)
    {

        string sarray = "";

        foreach (Vector2 a in array) sarray += a.ToString("F3");

        return sarray.Replace(',', ';').Replace(')', ':').Replace("(", "");
    }

}
