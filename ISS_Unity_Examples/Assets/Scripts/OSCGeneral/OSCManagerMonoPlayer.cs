using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class OSCManagerMonoPlayer : MonoBehaviour
{
    public string serverStrId;
    public int rcvPort;

    void Awake()
    {
        if (CommandLineParser.oscPort > 1023 && CommandLineParser.oscPort < 65536) 
            rcvPort = CommandLineParser.oscPort;
        OSCServer server = OSCHandler.Instance.CreateServer(serverStrId, rcvPort);
        server.SleepMilliseconds = 1;
        Debug.Log("OSC Server '" + serverStrId + "' created on port " + rcvPort);
    }


    void Update()
    {
        OSCHandler.Instance.UpdateLogs();
    }
}
