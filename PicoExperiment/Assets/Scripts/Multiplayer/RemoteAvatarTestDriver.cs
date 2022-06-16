using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

public class RemoteAvatarTestDriver : MonoBehaviour
{
    class PacketLatencyPair
    {
        public byte[] PacketData;
        public float FakeLatency;
    };

    public CaptureAvatarPackages LocalAvatar;
    public RemoteAvatarDriver LoopbackAvatar;

    [System.Serializable]
    public class SimulatedLatencySettings
    {
        [Range(0.0f, 0.5f)]
        public float FakeLatencyMax = 0.25f; //250 ms max latency

        [Range(0.0f, 0.5f)]
        public float FakeLatencyMin = 0.002f; //2ms min latency

        [Range(0.0f, 1.0f)]
        public float LatencyWeight = 0.25f;  // How much the latest sample impacts the current latency

        [Range(0, 10)]
        public int MaxSamples = 4; //How many samples in our window

        internal float AverageWindow = 0f;
        internal float LatencySum = 0f;
        internal LinkedList<float> LatencyValues = new LinkedList<float>();

        public float NextValue()
        {
            AverageWindow = LatencySum / (float)LatencyValues.Count;
            float RandomLatency = UnityEngine.Random.Range(FakeLatencyMin, FakeLatencyMax);
            float FakeLatency = AverageWindow * (1f - LatencyWeight) + LatencyWeight * RandomLatency;

            if (LatencyValues.Count >= MaxSamples)
            {
                LatencySum -= LatencyValues.First.Value;
                LatencyValues.RemoveFirst();
            }

            LatencySum += FakeLatency;
            LatencyValues.AddLast(FakeLatency);

            return FakeLatency;
        }
    };

    public SimulatedLatencySettings LatencySettings = new SimulatedLatencySettings();

    private int PacketSequence = 0;

    LinkedList<PacketLatencyPair> packetQueue = new LinkedList<PacketLatencyPair>();

    // Start is called before the first frame update
    void Start()
    {
		
		LocalAvatar.RecordPackets = true;
		LocalAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;
		float FirstValue = UnityEngine.Random.Range(LatencySettings.FakeLatencyMin, LatencySettings.FakeLatencyMax);
		LatencySettings.LatencyValues.AddFirst(FirstValue);
		LatencySettings.LatencySum += FirstValue;
		
	}

    // Update is called once per frame
    void OnLocalAvatarPacketRecorded(object sender, PacketEventArgs args)
    {
        using (MemoryStream outputStream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(outputStream);

            writer.Write(PacketSequence++);
            args.Packet.Write(outputStream);
           
            SendPacketData(outputStream.ToArray());
        }
    }

    void SendPacketData(byte[] data)
    {
        PacketLatencyPair PacketPair = new PacketLatencyPair();
        PacketPair.PacketData = data;
        PacketPair.FakeLatency = LatencySettings.NextValue();

        packetQueue.AddLast(PacketPair);
    }

    void Update()
    {
        if (packetQueue.Count > 0)
        {
            List<PacketLatencyPair> deadList = new List<PacketLatencyPair>();
            foreach (var packet in packetQueue)
            {
                packet.FakeLatency -= Time.deltaTime;

                if (packet.FakeLatency < 0f)
                {
                    ReceivePacketData(packet.PacketData);
                    deadList.Add(packet);
                }
            }

            foreach (var packet in deadList)
            {
                packetQueue.Remove(packet);
            }
        }
    }


    void ReceivePacketData(byte[] data)
    {
        using (MemoryStream inputStream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int sequence = reader.ReadInt32();

            AvatarPacket avatarPacket;
   
            avatarPacket = AvatarPacket.Read(inputStream);

            LoopbackAvatar.QueuePacket(sequence, avatarPacket);
        }
    }

}
