using System;
using System.Collections.Generic;
using UnityEngine;


public class RemoteAvatarDriver : MonoBehaviour
{
    [Header("Head")] [SerializeField] private Transform Head;

    [Header("Controller")] [SerializeField] private Transform LeftController;
    [SerializeField] private Transform RightController;

    [Header("Eyes")] [SerializeField] private Transform LeftEye;
    [SerializeField] private Transform RightEye;

    [Header("Face")] [SerializeField] private SkinnedMeshRenderer Face;


    Queue<AvatarPacket> packetQueue = new Queue<AvatarPacket>();

    IntPtr CurrentSDKPacket = IntPtr.Zero;

    public CaptureAvatarPackages.PoseFrame CurrentPose { get; private set; }

    float CurrentPacketTime = 0f;

    const int MinPacketQueue = 1;
    const int MaxPacketQueue = 4;

    int CurrentSequence = -1;

    // Used for legacy Unity only packet blending
    bool isStreaming = false;
    AvatarPacket currentPacket = null;


    private void Update()
    {

        UpdateFromUnityPacket();

    }

    public void QueuePacket(int sequence, AvatarPacket packet)
    {

        if (sequence > CurrentSequence)
        {
            CurrentSequence = sequence;
            packetQueue.Enqueue(packet);
        }
    }

  
    private void UpdateFromUnityPacket()
    {
        // If we're not currently streaming, check to see if we've buffered enough
        if (!isStreaming && packetQueue.Count > MinPacketQueue)
        {
            currentPacket = packetQueue.Dequeue();
            isStreaming = true;
        }

        // If we are streaming, update our pose
        if (isStreaming)
        {
            CurrentPacketTime += Time.deltaTime;

            // If we've elapsed past our current packet, advance
            while (CurrentPacketTime > currentPacket.Duration)
            {

                // If we're out of packets, stop streaming and
                // lock to the final frame
                if (packetQueue.Count == 0)
                {
                    CurrentPose = currentPacket.FinalFrame;
                    CurrentPacketTime = 0.0f;
                    currentPacket = null;
                    isStreaming = false;
                    return;
                }

                while (packetQueue.Count > MaxPacketQueue)
                {
                    packetQueue.Dequeue();
                }

                // Otherwise, dequeue the next packet
                CurrentPacketTime -= currentPacket.Duration;
                currentPacket = packetQueue.Dequeue();
            }

            // Compute the pose based on our current time offset in the packet
            CurrentPose = currentPacket.GetPoseFrame(CurrentPacketTime);

            UpdateTransformsFromPose();
        }
    }

    public void UpdateTransformsFromPose()
    {
        Head.localPosition = CurrentPose.headPosition;
        Head.localRotation = CurrentPose.headRotation;

        LeftController.localPosition = CurrentPose.handLeftPosition;
        LeftController.localRotation = CurrentPose.handLeftRotation;
        RightController.localPosition = CurrentPose.handRightPosition;
        RightController.localRotation = CurrentPose.handRightRotation;

        LeftEye.localPosition = CurrentPose.EyeLeftPosition;
        LeftEye.localRotation = CurrentPose.EyeLeftRotation;
        RightEye.localPosition = CurrentPose.EyeRightPosition;
        RightEye.localRotation = CurrentPose.EyeRightRotation;

        // Mirror the face blend shapes.
        for (int i = 0; i < 15; i++)
        {

            Face.SetBlendShapeWeight(i+1, CurrentPose.blendShapeWeights[i]);
        }


    }

}

