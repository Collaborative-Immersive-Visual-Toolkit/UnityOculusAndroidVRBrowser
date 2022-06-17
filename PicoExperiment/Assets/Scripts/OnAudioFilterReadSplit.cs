using UnityEngine;
using GoogleCloudStreamingSpeechToText;
using Photon.Voice.Unity;

public class OnAudioFilterReadSplit : MonoBehaviour
{


    public CustomStreamingRecognizer csr;

    public OVRLipSyncContext lsc;

    public Speaker spk;

    void OnAudioFilterRead(float[] data, int channels)
    {

        if (csr != null) csr.OnAudioFilterReadProxy(data, channels);
        if (lsc != null) lsc.OnAudioFilterReadProxy(data, channels);
#if USE_ONAUDIOFILTERREAD
        if (spk != null) spk.OnAudioFilterReadProxy(data, channels);
#endif
    }

}
