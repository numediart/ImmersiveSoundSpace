// © - 2020 – UMONS - CLICK' Living Lab
// 
// Immersive Sound Space of University of MONS – CLICK' Living Lab (Fabien Grisard) is free software: 
// you can redistribute it and/or modify it under the terms of the 3-Clause BSD licence. 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the 3-Clause BSD licence License for more details.
// 
// ​​​​​You should have received a copy of the 3-Clause BSD licence along with this program.  
// 
// Each use of this software must be attributed to University of MONS – CLICK' Living Lab  (Fabien Grisard).


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class MoveFromOSC : MonoBehaviour {

    public string serialNumbertoFollow;
    public Vector3 posInit;
    public Vector3 posScaling = new Vector3(1f, 1f, 1f);

    private Quaternion quat = new Quaternion();
    private Vector3 pos = new Vector3();
    private List<string> messages = new List<string>();

    private OSCServer moveObjectServer;

    // Use this for initialization
    void Start()
    {
        foreach (KeyValuePair<string, ServerLog> pair in OSCHandler.Instance.Servers)
        {
            if (pair.Key == "MoveObject")
            {
                moveObjectServer = pair.Value.server;
                pair.Value.server.PacketReceivedEvent += OnPacketReceived;
                Debug.Log("Server found : " + moveObjectServer.ToString());
            }
        }
    }

    // Update is called once per frame
    void Update () {
        OSCHandler.Instance.UpdateLogs();
        //HandlePacket(moveObjectServer.LastReceivedPacket);

        Vector3 correctedPosition = pos;
        correctedPosition.x *= posScaling.x;
        correctedPosition.y *= posScaling.y;
        correctedPosition.z *= posScaling.z;
        correctedPosition += posInit;
        transform.position = correctedPosition;
        transform.rotation = quat;

        if (messages.Count > 0)
        {
            foreach (string s in messages)
            {
                OscLogger.Instance.Print(s);
            }
            messages.Clear();
        }
    }

    void OnPacketReceived(OSCServer server, OSCPacket packet)
    {
        HandlePacket(packet);
    }

    void HandlePacket(OSCPacket packet)
    {
        if (packet.IsBundle())
        {
            foreach (OSCMessage mes in packet.Data)
            {
                HandleMessage(mes);
            }
        }
        else
        {
            HandleMessage(packet);
        }
    }

    void HandleMessage(OSCPacket mes)
    {
        switch (mes.Address.ToString().ToLower())
        {
            case "/iss/test":
                Debug.Log("test message received, " + int.Parse(mes.Data[0].ToString()) + " controllers detected");
                messages.Add("test message received, ");
                messages.Add(int.Parse(mes.Data[0].ToString()) + " controllers detected");
                break;
            case "/iss/tracker":
                string serial = mes.Data[0].ToString();
                if (mes.Data.Count >= 8 && serialNumbertoFollow.Contains(serial))
                {
                    messages.Add("/vive/tracker message received, serial matches");
                    pos.x = float.Parse(mes.Data[1].ToString());
                    pos.y = float.Parse(mes.Data[2].ToString());
                    pos.z = float.Parse(mes.Data[3].ToString());

                    quat.x = float.Parse(mes.Data[4].ToString());
                    quat.y = float.Parse(mes.Data[5].ToString());
                    quat.z = float.Parse(mes.Data[6].ToString());
                    quat.w = float.Parse(mes.Data[7].ToString());
                }
                break;
            default:
                break;
        }
    }
}
