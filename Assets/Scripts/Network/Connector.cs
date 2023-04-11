﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> handler, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                sessionFactory = handler;
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.UserToken = socket;
                args.RemoteEndPoint = endPoint;
                args.Completed += OnConnectCompleted;

                RegisterConnect(args);
            }
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if (!pending) OnConnectCompleted(null, args);

        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            Debug.Log("On Connected Complete called. " + args.SocketError);
            if (args.SocketError == SocketError.Success)
            {
                Session session = sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Debug.Log("fail." + args.SocketError.ToString());
                Console.WriteLine("OnConnect Fail " + args.SocketError.ToString());
            }
        }
    }
}
