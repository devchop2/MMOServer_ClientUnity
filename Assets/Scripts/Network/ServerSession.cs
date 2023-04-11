using ServerCore;
using System;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log("On Connected :" + endPoint);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log("OnDisconnected :" + endPoint);
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer, (session, packet) =>
        {
            PacketQueue.Instance.Push(packet);
        });
    }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine("Send Complete. total Bytes:" + numOfBytes);
    }
}

