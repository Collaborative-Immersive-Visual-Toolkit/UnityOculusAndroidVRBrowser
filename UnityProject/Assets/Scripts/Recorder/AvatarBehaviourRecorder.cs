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
    string ControllerPos;
    string ControllerEAng;
    string PointerPos;

    private void OnEnable()
    {

        //this was creating lag
        /*Time.captureFramerate = frameRate;*/

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        { 
            Record();
        }
        else if(Input.GetKeyDown(KeyCode.S)) 
        {
            closeWriter();
            return;
        }

        if (writer == null ) return;

        currentTime = Time.unscaledTime - startTime;

        line = currentTime.ToString("F3");

        foreach (inputs i in ram.inputs) {

            HeadPos = i.LocalHead == null ? "null,null,null" : i.LocalHead.position.ToString("F3");
            HeadEAng = i.LocalHead == null ? "null,null,null" : i.LocalHead.eulerAngles.ToString("F3");
            ControllerPos = i.Controller == null ? "null,null,null" : i.Controller.position.ToString("F3");
            ControllerEAng = i.Controller == null ? "null,null,null" : i.Controller.eulerAngles.ToString("F3");
            PointerPos = i.Pointer.gameObject.activeSelf == false ? "null,null,null" : i.Pointer.position.ToString("F3");

            line += "," + HeadPos.Trim(remove) + "," + HeadEAng.Trim(remove) + "," +
                    ControllerPos.Trim(remove) + "," + ControllerEAng.Trim(remove) + "," +
                    PointerPos.Trim(remove);
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
