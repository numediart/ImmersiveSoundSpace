using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using Mirror;
using System.Net;


[Serializable]
public class ServerToCreate{
    public int rcvPort;
    public string strId;
}

public class OSCManager : MonoBehaviour
{
    public List<ServerToCreate> serversToCreate;

    private bool serversCreated = false;

    public void CreateServers()
    {
        foreach (ServerToCreate s in serversToCreate)
        {
            OSCServer server = OSCHandler.Instance.CreateServer(s.strId, s.rcvPort);
            server.SleepMilliseconds = 1;
            Debug.Log("OSC Server '" + s.strId + "' created on port " + s.rcvPort);
        }
        serversCreated = true;
        //OSCHandler.Instance.CreateClient("HMD", IPAddress.Parse(sendToIp), sendPort);
    }


    public void StopServers()
    {
        Debug.Log("Stop OSC Servers");
        if(serversCreated)
            OSCHandler.Instance.Halt();
        serversCreated = false;
    }

    void Update()
    {
        if(serversCreated)
            OSCHandler.Instance.UpdateLogs();
    }
}
