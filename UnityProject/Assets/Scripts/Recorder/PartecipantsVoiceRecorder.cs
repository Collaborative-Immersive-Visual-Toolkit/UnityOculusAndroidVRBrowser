﻿using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PartecipantsVoiceRecorder : MonoBehaviour
{
    BinaryWriter writer;

    List<RecorderObject> list = new List<RecorderObject>();

    bool recording = false;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        PhotonVoiceNetwork.Instance.RemoteVoiceAdded += HandleRemoteVoicAdded;
#endif       
    }

    private void HandleRemoteVoicAdded(RemoteVoiceLink obj)
    {

        Debug.Log("[voice recording] Create a new voice recorded object linked to the remote voice link");

        RecorderObject ro = new RecorderObject();

        ro.obj = obj;

        list.Add(ro);

    }

    public void StartRecording() {

        if (recording) return;

        foreach (RecorderObject r in list) 
            r.StartRecording();

        
        recording = true;


    }

    public void OnApplicationQuit()
    {
        foreach (RecorderObject r in list)
            r.SaveAndCloseFile();
    }

}

public class RecorderObject 
{
    FileStream stream;

    public RemoteVoiceLink obj;

    public void StartRecording()
    {

        Debug.Log("[voice recording object] Open File Stream");


        //Create and open file for the stream in RemoteVoiceAdded handler.

        string fileName = obj.PlayerId.ToString();

        string path = Application.dataPath + "\\" + MasterManager.GameSettings.DataFolder + "\\" + fileName + ".wav";

        stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        //register opening and closing events

        //obj.FloatFrameDecoded += WriteFrameAudioData;


        obj.RemoteVoiceRemoved += SaveAndCloseFile;

    }

    public void SaveAndCloseFile()
    {
        //Save and close the file in RemoteVoiceRemoved handler.
        stream.Close();

        Debug.Log("[voice recording object] Closing File Stream");
    }

    private void WriteFrameAudioData(float[] obj)
    {
        if ( stream != null) stream.AppendWaveData(obj);

    }



}

public static class BinaryWriterToWavExtensions
{

    //https://stackoverflow.com/questions/50302315/save-audio-stream-float-frames-as-wav-with-c-sharp


    private const int HeaderSize = 44;

    private const int Hz = 12000; //frequency or sampling rate

    private const float RescaleFactor = 32767; //to convert float to Int16

    public static void AppendWaveData<T>(this T stream, float[] buffer)
       where T : Stream
    {
        if (stream.Length > HeaderSize)
        {
            stream.Seek(0, SeekOrigin.End);
        }
        else
        {
            stream.SetLength(HeaderSize);
            stream.Position = HeaderSize;
        }

        // rescale
        var floats = Array.ConvertAll(buffer, x => (short)(x * RescaleFactor));

        // Copy to bytes
        var result = new byte[floats.Length * sizeof(short)];
        Buffer.BlockCopy(floats, 0, result, 0, result.Length);

        // write to stream
        stream.Write(result, 0, result.Length);

        // Update Header
        UpdateHeader(stream);
    }

    public static void UpdateHeader(Stream stream)
    {
        var writer = new BinaryWriter(stream);

        writer.Seek(0, SeekOrigin.Begin);

        writer.Write(Encoding.ASCII.GetBytes("RIFF")); //RIFF marker. Marks the file as a riff file. Characters are each 1 byte long. 
        writer.Write((int)(writer.BaseStream.Length - 8)); //file-size (equals file-size - 8). Size of the overall file - 8 bytes, in bytes (32-bit integer). Typically, you'd fill this in after creation.
        writer.Write(Encoding.ASCII.GetBytes("WAVE")); //File Type Header. For our purposes, it always equals "WAVE".
        writer.Write(Encoding.ASCII.GetBytes("fmt ")); //Mark the format section. Format chunk marker. Includes trailing null. 
        writer.Write(16); //Length of format data.  Always 16. 
        writer.Write((short)1); //Type of format (1 is PCM, other number means compression) . 2 byte integer. Wave type PCM
        writer.Write((short)2); //Number of Channels - 2 byte integer
        writer.Write(Hz); //Sample Rate - 32 byte integer. Sample Rate = Number of Samples per second, or Hertz.
        writer.Write(Hz * 2 * 1); // sampleRate * bytesPerSample * number of channels, here 16000*2*1.
        writer.Write((short)(1 * 2)); //channels * bytesPerSample, here 1 * 2  // Bytes Per Sample: 1=8 bit Mono,  2 = 8 bit Stereo or 16 bit Mono, 4 = 16 bit Stereo
        writer.Write((short)16); //Bits per sample (BitsPerSample * Channels) ?? should be 8???
        writer.Write(Encoding.ASCII.GetBytes("data")); //"data" chunk header. Marks the beginning of the data section.    
        writer.Write((int)(writer.BaseStream.Length - HeaderSize)); //Size of the data section. data-size (equals file-size - 44). or NumSamples * NumChannels * bytesPerSample ??        
    }
} //end of class