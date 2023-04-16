using System;
using System.Collections.Generic;

public class SessionManager
{
    static SessionManager _instance = new SessionManager();
    public static SessionManager Instance { get { return _instance; } }

    List<ServerSession> _sessions = new List<ServerSession>();
    object lockObj = new object();

    public ServerSession Generate()
    {
        lock (lockObj)
        {
            ServerSession session = new ServerSession();
            _sessions.Add(session);
            return session;
        }
    }

}


