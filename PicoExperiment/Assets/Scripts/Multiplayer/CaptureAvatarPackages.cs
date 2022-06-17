using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class PacketRecordSettings
{
    internal bool RecordingFrames = false;
    public float UpdateRate = 1f / 30f; // 30 hz update of packets
    internal float AccumulatedTime;
};

// Avatar packets
public class PacketEventArgs : EventArgs
{
    public readonly AvatarPacket Packet;
    public PacketEventArgs(AvatarPacket packet)
    {
        Packet = packet;
    }
}

public class CaptureAvatarPackages : MonoBehaviour
{
    [Header("Head")] [SerializeField] private Transform Head;

    [Header("Controller")] [SerializeField] private Transform LeftController;
    [SerializeField] private Transform RightController;

    [Header("Eyes")] [SerializeField] private Transform LeftEye;
    [SerializeField] private Transform RightEye;

    [Header("Face")] [SerializeField] private SkinnedMeshRenderer Face;


    protected PoseFrame CurrentPose;
    public PoseFrame GetCurrentPose() { return CurrentPose; }

    private AvatarPacket CurrentUnityPacket;

    public EventHandler<PacketEventArgs> PacketRecorded;

    public PacketRecordSettings PacketSettings = new PacketRecordSettings();

    public bool RecordPackets = true;


    void Update()
    {
      
        if (RecordPackets)
        {
           CalculateCurrentPose();
           RecordUnityFrame();
        }

    }

    private void RecordUnityFrame()
    {
        var deltaSeconds = Time.deltaTime;
        var frame = GetCurrentPose();
        // If this is our first packet, store the pose as the initial frame
        if (CurrentUnityPacket == null)
        {
            CurrentUnityPacket = new AvatarPacket(frame);
            deltaSeconds = 0;
        }

        float recordedSeconds = 0;
        while (recordedSeconds < deltaSeconds)
        {
            float remainingSeconds = deltaSeconds - recordedSeconds;
            float remainingPacketSeconds = PacketSettings.UpdateRate - CurrentUnityPacket.Duration;

            // If we're not going to fill the packet, just add the frame
            if (remainingSeconds < remainingPacketSeconds)
            {
                CurrentUnityPacket.AddFrame(frame, remainingSeconds);
                recordedSeconds += remainingSeconds;
            }

            // If we're going to fill the packet, interpolate the pose, send the packet,
            // and open a new one
            else
            {
                // Interpolate between the packet's last frame and our target pose
                // to compute a pose at the end of the packet time.
                PoseFrame a = CurrentUnityPacket.FinalFrame;
                PoseFrame b = frame;
                float t = remainingPacketSeconds / remainingSeconds;
                PoseFrame intermediatePose = PoseFrame.Interpolate(a, b, t);
                CurrentUnityPacket.AddFrame(intermediatePose, remainingPacketSeconds);
                recordedSeconds += remainingPacketSeconds;

                // Broadcast the recorded packet
                if (PacketRecorded != null)
                {
                    PacketRecorded(this, new PacketEventArgs(CurrentUnityPacket));
                }

                // Open a new packet
                CurrentUnityPacket = new AvatarPacket(intermediatePose);
            }
        }
    }

    private void CalculateCurrentPose()
    {
        CurrentPose = new PoseFrame
        {

            headPosition = Head.localPosition,
            headRotation = Head.localRotation,

            handLeftPosition = LeftController.localPosition,
            handLeftRotation = LeftController.localRotation,
            handRightPosition = RightController.localPosition,
            handRightRotation = RightController.localRotation,

            EyeLeftPosition = LeftEye.localPosition,
            EyeLeftRotation = LeftEye.localRotation,
            EyeRightPosition = RightEye.localPosition,
            EyeRightRotation = RightEye.localRotation,

        };

        CurrentPose.ExtractSkinnedMeshWeights(Face);
    }


    public struct PoseFrame
    {
        public Vector3 headPosition;
        public Quaternion headRotation;

        public Vector3 handLeftPosition;
        public Quaternion handLeftRotation;
        public Vector3 handRightPosition;
        public Quaternion handRightRotation;

        public Vector3 EyeLeftPosition;
        public Quaternion EyeLeftRotation;
        public Vector3 EyeRightPosition;
        public Quaternion EyeRightRotation;


        public static List<int> _blendShapeIndexes;
        public List<float> blendShapeWeights;

        public void ExtractSkinnedMeshWeights(SkinnedMeshRenderer originalFace) {

            if (_blendShapeIndexes == null)
            {

                int blendShapeCount = originalFace.sharedMesh.blendShapeCount;

                _blendShapeIndexes = Enumerable.Range(0, blendShapeCount).ToList();

            }

            blendShapeWeights = new List<float>();

            // Mirror the face blend shapes.
            for (int i = 0; i < _blendShapeIndexes.Count; i++)
            {
                int blendShapeIndex = _blendShapeIndexes[i];
                float blendshapeWeight = originalFace.GetBlendShapeWeight(blendShapeIndex);
                blendShapeWeights.Add(blendshapeWeight);
            }
        }

        public static PoseFrame Interpolate(PoseFrame a, PoseFrame b, float t)
        {
            List<float> blendShapeWeightsNew = new List<float>();

            for (int i = 0; i < _blendShapeIndexes.Count; i++) {

                blendShapeWeightsNew.Add(Mathf.Lerp(a.blendShapeWeights[i], b.blendShapeWeights[i], t));
            }

            return new PoseFrame
            {
                headPosition = Vector3.Lerp(a.headPosition, b.headPosition, t),
                headRotation = Quaternion.Slerp(a.headRotation, b.headRotation, t),
                
                handLeftPosition = Vector3.Lerp(a.handLeftPosition, b.handLeftPosition, t),
                handLeftRotation = Quaternion.Slerp(a.handLeftRotation, b.handLeftRotation, t),
                handRightPosition = Vector3.Lerp(a.handRightPosition, b.handRightPosition, t),
                handRightRotation = Quaternion.Slerp(a.handRightRotation, b.handRightRotation, t),

                EyeLeftPosition = Vector3.Lerp(a.EyeLeftPosition, b.EyeLeftPosition, t),
                EyeLeftRotation = Quaternion.Slerp(a.EyeLeftRotation, b.EyeLeftRotation, t),
                EyeRightPosition = Vector3.Lerp(a.EyeRightPosition, b.EyeRightPosition, t),
                EyeRightRotation = Quaternion.Slerp(a.EyeRightRotation, b.EyeRightRotation, t),

                blendShapeWeights = blendShapeWeightsNew,
            };
        }
    };
    
}


