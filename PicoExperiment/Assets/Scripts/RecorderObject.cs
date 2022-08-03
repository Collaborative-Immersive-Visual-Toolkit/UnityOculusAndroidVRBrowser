using System;
using System.IO;
using UnityEngine;


public class RecorderObject:MonoBehaviour
{
    FileStream stream;

    public bool recording = false;

    AndroidJavaObject intent;
    AndroidJavaObject context;



    private void Start()
    {
       //StartRecording();
        //recordScreen();
    }
    public void StartRecording()
    {

        Debug.Log("[voice recording object] Open File Stream");

        recording = true;

        //Create and open file for the stream in RemoteVoiceAdded handler.

        string fileName = DateTime.Now.DayOfYear + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;

        string path = Application.persistentDataPath + "/" + fileName + ".wav";

        stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

    }



    public void SaveAndCloseFile()
    {
        recording = false;

        //Save and close the file in RemoteVoiceRemoved handler.
        if (stream != null) stream.Close();

        Debug.Log("[voice recording object] Closing File Stream");
    }

    public void WriteFrameAudioData(float[] obj)
    {
        if (stream != null)  stream.AppendWaveData(obj);
    }

    public void OnApplicationPause()
    {
        SaveAndCloseFile();
        //intent.Call<AndroidJavaObject>("putExtra", "action_type", 1);
        //context.Call<AndroidJavaObject>("stopService", intent);

    }

    public void OnAudioFilterReadProxy(float[] data, int channels)
    {
        Debug.Log(data);
        if (recording) WriteFrameAudioData(data);
    }

    void recordScreen() {


        context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        intent = new AndroidJavaObject("android.content.Intent");

        intent.Call<AndroidJavaObject>("setAction", "pvr.intent.action.RECORD");

        intent.Call<AndroidJavaObject>("setPackage", "com.pvr.shortcut");

        intent.Call<AndroidJavaObject>("putExtra", "action_type", 0);

        //0: starts recording screen; 1: stops recording screen

        context.Call<AndroidJavaObject>("startService", intent);
    }
    
}
