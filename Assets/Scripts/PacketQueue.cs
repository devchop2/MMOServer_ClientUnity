using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance = new PacketQueue();
    Queue<IPacket> queue = new Queue<IPacket>();
    object lockObj = new object();

    public void Push(IPacket packet)
    {
        lock (lockObj)
        {
            queue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (lockObj)
        {
            if (queue.Count == 0) return null;
            return queue.Dequeue();
        }
    }

}
