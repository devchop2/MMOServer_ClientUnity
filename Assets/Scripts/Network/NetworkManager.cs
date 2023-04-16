using System.Collections;
using System.Collections.Generic;
using ServerCore;
using System.Net;
using System.Threading;
using UnityEngine;
using System;

public class NetworkManager : MonoBehaviour
{
    ServerSession session = new ServerSession();

    private void Start()
    {
        Connect();

    }

    void Connect()
    {
        string host = Dns.GetHostName(); // get host name;

        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        foreach (var item in ipHost.AddressList)
        {
            if (item.ToString().StartsWith("172."))
            {
                ipAddr = item;
                break;
            }
        }

        //ip address : 부하분산을 위해 여러개의 ip가 생성될 수 있음. 우선 첫번 째 꺼를 사용함. 
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 8080); // 7777: port number

        //서버에게 연결을 시도. 
        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return session; }, 1);
    }

    private void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (var item in list)
        {
            PacketManager.Instance.HandlePacket(session, item);
        }

        //SessionManager.Instance.SendForEach();
    }


    public void Send(ArraySegment<byte> arr) { session?.Send(arr); }

}
