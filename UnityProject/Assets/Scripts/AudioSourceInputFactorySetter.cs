using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;
using ILogger = Photon.Voice.ILogger;

public class AudioSourceInputFactorySetter : VoiceComponent {
    private AudioSourceInputFactory audioSourcePusher;

    [SerializeField]
    public Recorder recorder;

    [SerializeField]
    public AudioSource audioSource;

    protected override void Awake() {
        this.audioSourcePusher = new AudioSourceInputFactory(this.audioSource, this.Logger);
        this.recorder.SourceType = Recorder.InputSourceType.Factory;
        this.recorder.InputFactory = this.InputFactory;
        if (this.recorder.RequiresRestart) {
            this.recorder.RestartRecording();
        } else if (this.recorder.IsInitialized) {
            this.recorder.StartRecording();
        }
        this.recorder.AutoStart = true;
    }

    private IAudioDesc InputFactory() {
        return this.audioSourcePusher;
    }
}

public class AudioSourceInputFactory : IAudioPusher<float> {

    private AudioSource audioSource;
    private ILogger logger;

    private AudioOutCapture audioOutCapture;

    private int sampleRate;
    private int channels;

    public int SamplingRate { get { return this.Error == null ? this.sampleRate : 0; } }
    public int Channels { get { return this.Error == null ? this.channels : 0; } }
    public string Error { get; private set; }

    public AudioSourceInputFactory(AudioSource aS, ILogger lg) {
        try {
            this.logger = lg;
            this.audioSource = aS;
            this.sampleRate = AudioSettings.outputSampleRate;
            switch (AudioSettings.speakerMode) {
                case AudioSpeakerMode.Mono: this.channels = 1; break;
                case AudioSpeakerMode.Stereo: this.channels = 2; break;
                default:
                    this.Error = string.Concat("Only Mono and Stereo project speaker mode supported. Current mode is ", AudioSettings.speakerMode);
                    this.logger.LogError("AudioSourceInputFactory: {0}", this.Error);
                    return;
            }
            if (!this.audioSource.enabled) {
                this.logger.LogWarning("AudioSourceInputFactory: AudioSource component disabled, enabling it.");
                this.audioSource.enabled = true;
            }
            if (!this.audioSource.gameObject.activeSelf) {
                this.logger.LogWarning("AudioSourceInputFactory: AudioSource GameObject inactive, activating it.");
                this.audioSource.gameObject.SetActive(true);
            }
            if (!this.audioSource.gameObject.activeInHierarchy) {
                this.Error = "AudioSource GameObject is not active in hierarchy, audio input can't work.";
                this.logger.LogError("AudioSourceInputFactory: {0}", this.Error);
                return;
            }
            this.audioOutCapture = this.audioSource.gameObject.GetComponent<AudioOutCapture>();
            if (ReferenceEquals(null, this.audioOutCapture) || !this.audioOutCapture) {
                this.audioOutCapture = this.audioSource.gameObject.AddComponent<AudioOutCapture>();
            }
            if (!this.audioOutCapture.enabled) {
                this.logger.LogWarning("AudioSourceInputFactory: AudioOutCapture component disabled, enabling it.");
                this.audioOutCapture.enabled = true;
            }
        } catch (Exception e) {
            this.Error = e.ToString();
            if (this.Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
            {
                this.Error = "Exception in MicWrapperPusher constructor";
            }
            this.logger.LogError("AudioSourceInputFactory: {0}", this.Error);
        }
    }

    private float[] frame2 = Array.Empty<float>();

    private void AudioOutCaptureOnOnAudioFrame(float[] frame, int channelsNumber) {
        if (channelsNumber != this.Channels) {
            this.logger.LogWarning("AudioSourceInputFactory: channels number mismatch; expected:{0} got:{1}.", this.Channels, channelsNumber);
        }
        if (this.frame2.Length != frame.Length) {
            this.frame2 = new float[frame.Length];
        }
        Array.Copy(frame, this.frame2, frame.Length);
        this.pushCallback(frame);
        Array.Clear(frame, 0, frame.Length);
    }

    private Action<float[]> pushCallback;

    public void SetCallback(Action<float[]> callback, ObjectFactory<float[], int> bufferFactory) {
        this.pushCallback = callback;
        this.audioOutCapture.OnAudioFrame += this.AudioOutCaptureOnOnAudioFrame;
    }

    public void Dispose() {
        if (this.pushCallback != null && this.audioOutCapture != null) {
            this.audioOutCapture.OnAudioFrame -= this.AudioOutCaptureOnOnAudioFrame;
        }
    }
}
