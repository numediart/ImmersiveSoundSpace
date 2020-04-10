/* BSD Licence

 * Copyright (c) 2017-2020, UMONS - CLICK' Living Lab
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 * * Neither the name of the UMONS - nor CLICK' Living Lab - nor the
 *   names of its contributors may be used to endorse or promote products
 *   derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE REGENTS AND CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

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
            case "/vive/test":
                Debug.Log("test message received, " + int.Parse(mes.Data[0].ToString()) + " controllers detected");
                messages.Add("test message received, ");
                messages.Add(int.Parse(mes.Data[0].ToString()) + " controllers detected");
                break;
            case "/vive/tracker":
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
