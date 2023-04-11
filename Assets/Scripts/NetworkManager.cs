using System.Collections;
using System.Collections.Generic;
using ServerCore;
using System.Net;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession session = new ServerSession();

    private void Start()
    {
        Connect();
        StartCoroutine(ISend());
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
        IPacket packet = PacketQueue.Instance.Pop();
        if (packet != null) PacketManager.Instance.HandlePacket(session, packet);
        //SessionManager.Instance.SendForEach();
    }

    IEnumerator ISend()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            C_Chat chat = new C_Chat();
            chat.chat = "i'm Unity";

            var segment = chat.Serialize();
            Debug.Log("send!");
            session.Send(segment);
        }

    }
}
