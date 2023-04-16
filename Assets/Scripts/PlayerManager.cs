using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class PlayerManager
{
    MyPlayer myPlayer;
    Dictionary<int, Player> players = new Dictionary<int, Player>();

    public static PlayerManager Instance = new PlayerManager();

    public void Add(S_PlayerList list)
    {
        Object obj = Resources.Load("Player");
        foreach (var item in list.players)
        {
            GameObject go = Object.Instantiate(obj) as GameObject;
            Vector3 pos = new Vector3(item.posX, item.posY, item.posZ);
            if (item.isSelf)
            {
                myPlayer = go.AddComponent<MyPlayer>();
                myPlayer.playerId = item.playerId;
                myPlayer.SetPos(pos);
            }
            else
            {
                var player = go.AddComponent<Player>();
                player.SetPos(pos);
                player.playerId = item.playerId;
                players.Add(player.playerId, player);

            }
        }
    }

    public void EnterGame(S_BroadcastEnterGame enter)
    {
        if (myPlayer.playerId == enter.playerId) return;

        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        var player = go.AddComponent<Player>();
        player.SetPos(new Vector3(enter.posX, enter.posY, enter.posZ));
        player.playerId = enter.playerId;
        players.Add(player.playerId, player);

    }

    public void LeaveGame(S_BroadcastLeaveRoom leave)
    {
        int id = leave.playerId;
        if (id == myPlayer.playerId)
        {
            GameObject.Destroy(myPlayer.gameObject);
            myPlayer = null;
            return;
        }

        if (!players.ContainsKey(id)) return;
        var player = players[id];
        GameObject.Destroy(player.gameObject);
        players.Remove(id);
    }

    public void Move(S_BroadcastMove move)
    {
        int id = move.playerId;
        var pos = new Vector3(move.posX, move.posY, move.posZ);
        if (id == myPlayer.playerId)
        {
            myPlayer.SetPos(pos);
        }
        else
        {
            players.TryGetValue(id, out var player);
            if (player != null) player.SetPos(pos);
        }
    }

}
