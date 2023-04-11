using System;
using System.Text;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_ChatHandler(Session sesion, IPacket packet)
    {

        S_Chat chatting = packet as S_Chat;
        Debug.Log($"[{chatting.playerId}] {chatting.chat}\n");

        if (chatting.playerId == 1)
        {
            var obj = GameObject.Find("Player");
            if (obj == null) Debug.Log("obj is null");
            else Debug.Log("obj;" + obj.name);
        }

    }
}

