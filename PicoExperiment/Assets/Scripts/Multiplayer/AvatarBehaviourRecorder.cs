using System.IO;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor;
#endif


public class AvatarBehaviourRecorder : MonoBehaviour
{
    string path;

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
    string HeadForward;
    string HeadUp;
    string HeadCone;
    string ControllerRPos;
    string ControllerREAng;
    string ControllerLPos;
    string ControllerLEAng;
    string PointerPos;
    string LeftEye;
    string RightEye;
    string Gaze;
    string Speech;

    static string null3 = "null,null,null";

    private float nextSampleTime = 0.0f;
    public float sampleFrequency = 0.04f;


#if UNITY_EDITOR

    private RecorderWindow GetRecorderWindow()
    {
        return (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
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
                HeadForward = i.LocalHead == null ? "null,null,null" : i.LocalHead.forward.ToString("F3");
                HeadUp = i.LocalHead == null ? "null,null,null" : i.LocalHead.up.ToString("F3");
                
                HeadCone = i.Cone == null ? "null" : Vector2ArrayToString(i.Cone.uvpos.ToArray());

                LeftEye = i.LeftEye == null ? "null,null,null" : i.LeftEye.forward.ToString("F3");
                RightEye = i.RightEye == null ? "null,null,null" : i.RightEye.forward.ToString("F3");
                Gaze = i.Gaze.cursorPos == null ? "null,null,null" : i.Gaze.cursorPos.ToString("F3");

                ControllerRPos = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.position.ToString("F3");
                ControllerREAng = i.ControllerRight == null ? "null,null,null" : i.ControllerRight.eulerAngles.ToString("F3");
                ControllerLPos = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.position.ToString("F3");
                ControllerLEAng = i.ControllerLeft == null ? "null,null,null" : i.ControllerLeft.eulerAngles.ToString("F3");
                
                PointerPos = i.Pointer.end == Vector3.zero ? "null,null,null" : i.Pointer.end.ToString("F3");

                Speech = i.Speech.getResult();

                line += "," + PlayerPos.Trim(remove) + "," +
                              HeadPos.Trim(remove) + "," + HeadForward.Trim(remove) + "," + HeadUp.Trim(remove) + "," + HeadCone.Trim(remove) + "," +
                              LeftEye.Trim(remove) + "," + RightEye.Trim(remove) + "," + Gaze.Trim(remove) + "," +
                              ControllerRPos.Trim(remove) + "," + ControllerREAng.Trim(remove) + "," +
                              ControllerLPos.Trim(remove) + "," + ControllerLEAng.Trim(remove) + "," +
                              PointerPos.Trim(remove) + "," + Speech; 
            }

            writer.WriteLine(line);
        }

    }

    private static string Vector2ArrayToString(Vector2[] array) {

        string sarray = "";

        foreach(Vector2 a in array) sarray += a.ToString("F3");

        return sarray.Replace(',', ';').Replace(')',':').Replace("(","");
    }
   
    private void Record()
    {
        
            recording = true;
            this.NewData(fileName);
            StartUnityRecorder();
        
    }

    public void NewData(string name) {

        if (writer != null) closeWriter();

        path = MasterManager.GameSettings.DataFolder +"\\" + name + ".csv";
        writer = new StreamWriter(path, true);

        string header = "timestamp," +
        "U1PosX, U1PosY, U1PosZ," +
        "U1HeadPosX, U1HeadPosY, U1HeadPosZ," +
        "U1HeadForwardX, U1HeadForwardY, U1HeadForwardZ," +
        "U1HeadUpX, U1HeadUpY, U1HeadUpZ," +
        "U1HeadCone," +
        "U1LeftEyeX,U1LeftEyeY,U1LeftEyeZ," +
        "U1RightEyeX,U1RightEyeY,U1RightEyeZ," +
        "U1GazeX,U1GazeY,U1GazeZ," +
        "U1ControllerRPosX, U1ControllerRPosY, U1ControllerRPosZ," +
        "U1ControllerREAngX, U1ControllerREAngY,  U1ControllerREAngZ," +
        "U1ControllerLPosX, U1ControllerLPosY, U1ControllerLPosZ," +
        "U1ControllerLEAngX, U1ControllerLEAngY,  U1ControllerLEAngZ," +
        "U1PointerX, U1PointerY,  U1PointerZ," +
        "U2Speech," +
        "U2PosX, U1PosY, U1PosZ," +
        "U2HeadPosX, U2HeadPosY, U2HeadPosZ," +
        "U2HeadForwardX, U2HeadForwardY, U2HeadForwardZ," +
        "U2HeadUpX, U2HeadUpY, U2HeadUpZ," +
        "U2HeadCone," +
        "U2LeftEyeX,U2LeftEyeY,U2LeftEyeZ," +
        "U2RightEyeX,U2RightEyeY,U2RightEyeZ," +
        "U2GazeX,U2GazeY,U2GazeZ," +
        "U2ControllerRPosX, U2ControllerRPosY, U2ControllerRPosZ," +
        "U2ControllerREAngX, U2ControllerREAngY,  U2ControllerREAngZ," +
        "U2ControllerLPosX, U2ControllerLPosY, U2ControllerLPosZ," +
        "U2ControllerLEAngX, U2ControllerLEAngY,  U2ControllerLEAngZ," +
        "U2PointerX, U2PointerY,  U2PointerZ," +
        "U2Speech";

        writer.WriteLine(header);

        startTime = Time.unscaledTime;

    }

    void OnDisable()
    {
        closeWriter();
        StopUnityRecorder();
    }

    void OnApplicationQuit()
    {
        closeWriter();
        StopUnityRecorder();
    }

    public void closeWriter() {

        recording = false;

        if (writer != null) writer.Close();

        writer = null;

    }

    public void StartUnityRecorder() {

        RecorderWindow recorderWindow = GetRecorderWindow();

        if (!recorderWindow.IsRecording())
        {
            recorderWindow.StartRecording();
        }


    }

    public void StopUnityRecorder() {

        RecorderWindow recorderWindow = GetRecorderWindow();
        if (recorderWindow.IsRecording())
            recorderWindow.StopRecording();



    }

#endif
}
