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
using UnityEngine.UI;

public class CommandLineParser : MonoBehaviour
{
    public GameObject playerOSC;

    // Use this for initialization
    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        int port = 5005;
        for (int i = 0; i < args.Length; i++)
        {
            //Debug.Log("ARG " + i + ": " + args[i]);
            //DebugLogger.Instance.Print("ARG " + i + ": " + args[i]);
            switch (args[i])
            {
                case "--tracker":
                case "-t":
                    string device = args[i + 1];
                    MoveFromOSC moveScript = playerOSC.GetComponent<MoveFromOSC>();
                    if (moveScript)
                        moveScript.serialNumbertoFollow = device;
                    DebugLogger.Instance.Print("serial number of the tracker to follow is " + device);
                    break;
                case "--port":
                case "-p":
                        port = int.Parse(args[i + 1]);
                    break;
                default:
                    break;
            }
        }

        OSCHandler.Instance.CreateServer("MoveObject", port);
        OscLogger.Instance.Print("Server MoveObject created on port " + port);
    }
}
