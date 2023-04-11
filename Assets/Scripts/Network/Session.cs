﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace ServerCore
{
    //send, recv를 담당하는 세션
    public abstract class Session
    {
        Socket _socket;

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        object pendingLock = new object();
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        #region Handlers

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        #endregion
        public void Start(Socket socket)
        {
            _socket = socket;

            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }

        public void Send(List<ArraySegment<byte>> sendBuffs)
        {
            if (sendBuffs == null || sendBuffs.Count == 0) return;
            lock (pendingLock)
            {
                foreach (var item in sendBuffs)
                {
                    _sendQueue.Enqueue(item);
                }
                if (pendingList.Count == 0) RegisterSend();
            }
        }
        public void Send(ArraySegment<byte> sendBuff)
        {
            if (sendBuff == null) return;
            //한번에 하나만 호출되도록 보장
            lock (pendingLock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (pendingList.Count == 0) RegisterSend();
                //아직 전송작업이 진행되고있지 않으면 바로 전송을 Regist > pendingList = 0 이라는것은 현재 작업중인 리스트가 없다는것.

            }
        }


        void RegisterSend()
        {
            if (_disconnected == 1) return;
            //queue 안에 있는 모든 byte[] 를 넣음. 
            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();
                pendingList.Add(buff);
            }

            try
            {
                sendArgs.BufferList = pendingList;
                Debug.Log("socket:" + _socket);
                bool pending = _socket.SendAsync(sendArgs);
                if (!pending) OnSendCompleted(null, sendArgs);
            }
            catch (Exception e)
            {
                Debug.Log("register send exception occurred." + e.Message);
            }

        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {

            lock (pendingLock)
            {

                int sendBytes = args.BytesTransferred;
                if (sendBytes > 0 && args.SocketError == SocketError.Success)
                {
                    sendArgs.BufferList = null;
                    pendingList.Clear();


                    OnSend(sendBytes);
                    if (_sendQueue.Count > 0) RegisterSend();
                }
                else
                {
                    Debug.Log("OnRecvComplete Fail." + args.SocketError.ToString());
                    Disconnect();
                }
            }

        }

        #region Network 
        void RegisterRecv()
        {
            if (_disconnected == 1) return;


            //현재 유효한 범위를 지정해줌.
            _recvBuffer.Clean();  //혹시 index가 범위를 넘어가지 않도록 clean
            ArraySegment<byte> writeSegment = _recvBuffer.WriteSegment; //쓰기 가능한 공간 추출
            recvArgs.SetBuffer(writeSegment.Array, writeSegment.Offset, writeSegment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(recvArgs);
                if (!pending) OnRecvCompleted(null, recvArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Register Recv exception. " + e.Message);
            }

        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            int recvBytes = args.BytesTransferred;
            if (recvBytes > 0 && args.SocketError == SocketError.Success)
            {
                try
                {

                    bool writeSuccess = _recvBuffer.OnWrite(recvBytes);
                    if (!writeSuccess)
                    {
                        Disconnect();
                        return;
                    }

                    //데이터가 조립되지않아서  처리하지않았을경우 0 반환. 아닐경우 처리한 데이터 반환.
                    int ProcessLen = OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, recvBytes));
                    if (ProcessLen < 0 || _recvBuffer.DataSize < ProcessLen) //예외.
                    {
                        Disconnect();
                        return;
                    }

                    bool readSuccess = _recvBuffer.OnRead(ProcessLen);
                    if (!readSuccess)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine("OnRecvComplete Fail." + e.Message);
                    Disconnect();
                }
            }
            else
            {
                //disconnect
            }
        }

        public void Disconnect()
        {
            //여러 군데에서 discconect를 연달아 호출하면 문제가 생김. disconnected 변수를 두어 한번만 호출될 수 있도록 함.
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) return;

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }

        #endregion

        void Clear()
        {
            lock (pendingLock)
            {
                _sendQueue.Clear();
                pendingList.Clear();
            }

        }

    }
}
