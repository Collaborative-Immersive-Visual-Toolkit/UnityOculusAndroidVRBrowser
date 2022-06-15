using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnAudioFilterReadProxy : UnityEvent<float[], int> { }

public class OnAudioFilterReadSplit : MonoBehaviour
{
    public OnAudioFilterReadProxy onEvent;

    void OnAudioFilterRead(float[] data, int channels)
    {
        onEvent.Invoke(data,channels);
    }

}
