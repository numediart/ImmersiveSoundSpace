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
