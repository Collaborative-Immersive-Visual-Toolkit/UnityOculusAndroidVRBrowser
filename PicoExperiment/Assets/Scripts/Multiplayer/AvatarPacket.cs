using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class AvatarPacket
{
    // Used with SDK driven packet flow
    public IntPtr ovrNativePacket = IntPtr.Zero;

    // ===============================================================
    // All code below used for unity only pose blending option.
    // ===============================================================
    List<float> frameTimes = new List<float>();
    List<CaptureAvatarPackages.PoseFrame> frames = new List<CaptureAvatarPackages.PoseFrame>();

    public float Duration { get { return frameTimes[frameTimes.Count - 1]; } }
    public CaptureAvatarPackages.PoseFrame FinalFrame { get { return frames[frames.Count - 1]; } }

    public AvatarPacket()
    {
    }

    public AvatarPacket(CaptureAvatarPackages.PoseFrame initialPose)
    {
        frameTimes.Add(0.0f);
        frames.Add(initialPose);
    }

    AvatarPacket(List<float> frameTimes, List<CaptureAvatarPackages.PoseFrame> frames)
    {
        this.frameTimes = frameTimes;
        this.frames = frames;
    }

    public void AddFrame(CaptureAvatarPackages.PoseFrame frame, float deltaSeconds)
    {
        frameTimes.Add(Duration + deltaSeconds);
        frames.Add(frame);
    }

    public CaptureAvatarPackages.PoseFrame GetPoseFrame(float seconds)
    {
        if (frames.Count == 1)
        {
            return frames[0];
        }

        // This can be replaced with a more efficient binary search
        int tailIndex = 1;
        while (tailIndex < frameTimes.Count && frameTimes[tailIndex] < seconds)
        {
            ++tailIndex;
        }
        CaptureAvatarPackages.PoseFrame a = frames[tailIndex - 1];
        CaptureAvatarPackages.PoseFrame b = frames[tailIndex];
        float aTime = frameTimes[tailIndex - 1];
        float bTime = frameTimes[tailIndex];
        float t = (seconds - aTime) / (bTime - aTime);
        return CaptureAvatarPackages.PoseFrame.Interpolate(a, b, t);
    }

    public static AvatarPacket Read(Stream stream)
    {
        BinaryReader reader = new BinaryReader(stream);

        // Todo: bounds check frame count
        int frameCount = reader.ReadInt32();
        List<float> frameTimes = new List<float>(frameCount);
        for (int i = 0; i < frameCount; ++i)
        {
            frameTimes.Add(reader.ReadSingle());
        }
        List<CaptureAvatarPackages.PoseFrame> frames = new List<CaptureAvatarPackages.PoseFrame>(frameCount);
        for (int i = 0; i < frameCount; ++i)
        {
            frames.Add(reader.ReadPoseFrame());
        }

       

        return new AvatarPacket(frameTimes, frames);
    }

    public void Write(Stream stream)
    {
        BinaryWriter writer = new BinaryWriter(stream);

        // Write all of the frames
        int frameCount = frameTimes.Count;
        writer.Write(frameCount);
        for (int i = 0; i < frameCount; ++i)
        {
            writer.Write(frameTimes[i]);
        }
        for (int i = 0; i < frameCount; ++i)
        {
            CaptureAvatarPackages.PoseFrame frame = frames[i];
            writer.Write(frame);
        }

      
    }
}

static class BinaryWriterExtensions
{
    public static void Write(this BinaryWriter writer, CaptureAvatarPackages.PoseFrame frame)
    {
        writer.Write(frame.headPosition);
        writer.Write(frame.headRotation);

        writer.Write(frame.handLeftPosition);
        writer.Write(frame.handLeftRotation);
        writer.Write(frame.handRightPosition);
        writer.Write(frame.handRightRotation);

        writer.Write(frame.EyeLeftPosition);
        writer.Write(frame.EyeLeftRotation);
        writer.Write(frame.EyeRightPosition);
        writer.Write(frame.EyeRightRotation);

       writer.Write(frame.blendShapeWeights);


    }

    public static void Write(this BinaryWriter writer, Vector3 vec3)
    {
        writer.Write(vec3.x);
        writer.Write(vec3.y);
        writer.Write(vec3.z);
    }

    public static void Write(this BinaryWriter writer, Vector2 vec2)
    {
        writer.Write(vec2.x);
        writer.Write(vec2.y);
    }

    public static void Write(this BinaryWriter writer, Quaternion quat)
    {
        writer.Write(quat.x);
        writer.Write(quat.y);
        writer.Write(quat.z);
        writer.Write(quat.w);
    }

    public static void Write(this BinaryWriter writer, List<float> lista)
    {
        for (int i = 0; i < 15; i++)
        {
            writer.Write(lista[i]);
        }

    }

}

static class BinaryReaderExtensions
{
    public static CaptureAvatarPackages.PoseFrame ReadPoseFrame(this BinaryReader reader)
    {
        return new CaptureAvatarPackages.PoseFrame
        {
            headPosition = reader.ReadVector3(),
            headRotation = reader.ReadQuaternion(),
            
            handLeftPosition = reader.ReadVector3(),
            handLeftRotation = reader.ReadQuaternion(),
            handRightPosition = reader.ReadVector3(),
            handRightRotation = reader.ReadQuaternion(),

            EyeLeftPosition = reader.ReadVector3(),
            EyeLeftRotation = reader.ReadQuaternion(),
            EyeRightPosition = reader.ReadVector3(),
            EyeRightRotation = reader.ReadQuaternion(),

            blendShapeWeights = reader.ReadFloatList(),

        };
    }

    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle()
        };
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        return new Vector3
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle()
        };
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader)
    {
        return new Quaternion
        {
            x = reader.ReadSingle(),
            y = reader.ReadSingle(),
            z = reader.ReadSingle(),
            w = reader.ReadSingle(),
        };
    }

    public static List<float> ReadFloatList(this BinaryReader reader)
    {
        List<float> lista = new List<float>();

        for (int i = 0; i < 15; i++) {

            lista.Add(reader.ReadSingle()); 
        }

        return lista;
    }

}
