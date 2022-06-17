using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class PhotonAvatarView : MonoBehaviour, IPunObservable
{
    public PhotonView photonView;
	public CaptureAvatarPackages LocalAvatar;
	private RemoteAvatarDriver remoteDriver;
	private List<byte[]> packetData;


    public void Start()
    {


        if (photonView == null) photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {

            if (LocalAvatar == null)
            LocalAvatar.RecordPackets = true;
            LocalAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;

            packetData = new List<byte[]>();
        }
        else
        {
            remoteDriver = GetComponent<RemoteAvatarDriver>();

        }
    }

    public void OnDisable()
    {
        if (photonView.IsMine)
        {
            LocalAvatar.RecordPackets = false;
            LocalAvatar.PacketRecorded -= OnLocalAvatarPacketRecorded;
        }
    }

    private int localSequence;

    public void OnLocalAvatarPacketRecorded(object sender, PacketEventArgs args)
    {

        if (!PhotonNetwork.InRoom || (PhotonNetwork.CurrentRoom.PlayerCount < 2))
        {
            return;
        }

        using (MemoryStream outputStream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(outputStream);

            writer.Write(localSequence++);
            args.Packet.Write(outputStream);

            packetData.Add(outputStream.ToArray());
        }
    }

    private void DeserializeAndQueuePacketData(byte[] data)
    {

        using (MemoryStream inputStream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(inputStream);
            
            int sequence = reader.ReadInt32();

            AvatarPacket avatarPacket;

            avatarPacket = AvatarPacket.Read(inputStream);

            if (remoteDriver == null) return;

            remoteDriver.QueuePacket(sequence, avatarPacket);
        }

   
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


        if (stream.IsWriting)
        {
            if (packetData.Count == 0)
            {
                return;
            }

            stream.SendNext(packetData.Count);

            foreach (byte[] b in packetData)
            {
                stream.SendNext(b);
            }

            packetData.Clear();
        }

        if (stream.IsReading)
        {

            int num = (int)stream.ReceiveNext();

            for (int counter = 0; counter < num; ++counter)
            {
                byte[] data = (byte[])stream.ReceiveNext();

                DeserializeAndQueuePacketData(data);
            }
        }
    }

}
