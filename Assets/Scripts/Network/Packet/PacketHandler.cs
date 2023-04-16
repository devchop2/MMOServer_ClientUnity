using System;
using System.Text;
using ServerCore;
using UnityEngine;

public class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_BroadcastEnterGame enter = packet as S_BroadcastEnterGame;
        PlayerManager.Instance.EnterGame(enter);
    }

    public static void S_BroadcastLeaveRoomHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_BroadcastLeaveRoom enter = packet as S_BroadcastLeaveRoom;
        PlayerManager.Instance.LeaveGame(enter);
    }

    public static void S_PlayerListHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_PlayerList enter = packet as S_PlayerList;
        PlayerManager.Instance.Add(enter);
    }

    public static void S_BroadcastMoveHandler(Session session, IPacket packet)
    {
        ServerSession s = session as ServerSession;
        S_BroadcastMove enter = packet as S_BroadcastMove;
        PlayerManager.Instance.Move(enter);
    }

}

