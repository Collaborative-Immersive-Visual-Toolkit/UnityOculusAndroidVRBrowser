using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Realtime;

public class PartecipantsVoiceRecorder : MonoBehaviourPun
{
    BinaryWriter writer;

    List<RecorderObject> list = new List<RecorderObject>();

    bool recording = false;

    public Text label;
        
    private void OnEnable()
    {
#if UNITY_EDITOR
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClientEventReceived;
        PhotonVoiceNetwork.Instance.RemoteVoiceAdded += HandleRemoteVoicAdded;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClientEventReceived;
        PhotonVoiceNetwork.Instance.RemoteVoiceAdded -= HandleRemoteVoicAdded;
#endif
    }

    private void NetworkingClientEventReceived(EventData obj)
    {
        if (obj.Code == MasterManager.GameSettings.StartRecordAudio)
        {

            object[] data = (object[])obj.CustomData;

            StartRecordingByPlayerId(data[0]);

        }
        else if (obj.Code == MasterManager.GameSettings.StopRecordAudio) 
        {

            object[] data = (object[])obj.CustomData;

            StopRecordingByPlayerId(data[0]);

        }
    }

    private void HandleRemoteVoicAdded(RemoteVoiceLink obj)
    {

        Debug.Log("[voice recording] Create a new voice recorded object linked to the remote voice link");

        RecorderObject ro = new RecorderObject();

        ro.obj = obj;



        list.Add(ro);

    }

    public void ToggleRecording() {

        object[] data = new object[] { PhotonNetwork.LocalPlayer.UserId };


        if (recording) 
        {
            recording = false;
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.StopRecordAudio, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
            label.text = "Insight Record";
        }
        else 
        {
            recording = true;
            PhotonNetwork.RaiseEvent(MasterManager.GameSettings.StartRecordAudio, data, Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
            label.text = "Stop Record Insight";
        }

    }

    public void StartRecordingByPlayerId(object playerId)
    {

        if (recording) return;

     
        Player player =null;
        
        GameObject go;

        foreach (Player p in PhotonNetwork.PlayerList) {


            if (p.UserId == (string)playerId) {

                player = p;

                Debug.Log(p.ActorNumber);
        

            }

        }

        if (player == null) return;


        foreach (RecorderObject r in list)
        {
            if (r.obj.PlayerId == player.ActorNumber)
            {
                r.g = GameObject.Find(player.NickName);
                r.StartRecording();
            } 
                
        }
            
        recording = true;

    }

    public void StopRecordingByPlayerId(object playerId)
    {

        if (!recording) return;

        Player player = null;

        foreach (Player p in PhotonNetwork.PlayerList)
        {

            if (p.UserId == (string)playerId)
            {

                player = p;
            }

        }

        if (player == null) return;

        foreach (RecorderObject r in list)
        {
            if (r.obj.PlayerId == player.ActorNumber)
            {
               
                r.SaveAndCloseFile();
            }

        }

        recording = false;

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

    int count = 0;

    public bool recording = false;

    public GameObject g;

    private speaking s;

    public void StartRecording()
    {

        Debug.Log("[voice recording object] Open File Stream");

        recording = true;

        //Create and open file for the stream in RemoteVoiceAdded handler.

        string fileName = generateFilename();

        string path = Application.dataPath + "\\" + MasterManager.GameSettings.DataFolder + "\\" + fileName + ".wav";

        stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        s = g.GetComponentInChildren<speaking>();

        s.r = this;

        obj.RemoteVoiceRemoved += SaveAndCloseFile;


    }

    public string generateFilename() {

        count += 1;

        string fileName = "insight_" +count.ToString()+"_"+ obj.PlayerId.ToString();

        return fileName;
    }

    public void SaveAndCloseFile()
    {
        recording = false;

        if (s != null)  s.r = null;

        //Save and close the file in RemoteVoiceRemoved handler.
        if (stream != null)  stream.Close();

        Debug.Log("[voice recording object] Closing File Stream");
    }

    public void WriteFrameAudioData(float[] obj)
    {
        if ( stream != null) 
            stream.AppendWaveData(obj);

    }



}

public static class BinaryWriterToWavExtensions
{

    //https://stackoverflow.com/questions/50302315/save-audio-stream-float-frames-as-wav-with-c-sharp


    private const int HeaderSize = 44;

    private const int Hz =48000; //frequency or sampling rate

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