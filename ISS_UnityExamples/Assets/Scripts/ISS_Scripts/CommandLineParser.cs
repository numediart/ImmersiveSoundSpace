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
    public GameObject mainAudioListener; // this is usually the MainCamera, must have the script MoveFromOSC attached to itself
    public int defaultListeningPort;     // OSC port to listen
    public string headphoneTrackerSerial; // The serial number of the Vive Tracker attached to the headphones (will move the mainaudioListener)

    // Use this for initialization
    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        int port = defaultListeningPort;
        string tracker = headphoneTrackerSerial;

        for (int i = 0; i < args.Length; i++)
        {
            //Debug.Log("ARG " + i + ": " + args[i]);
            //DebugLogger.Instance.Print("ARG " + i + ": " + args[i]);
            switch (args[i])
            {
                case "--tracker":
                case "-t":
                    tracker = args[i + 1];
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

        MoveFromOSC moveScript = mainAudioListener.GetComponent<MoveFromOSC>();
        if (moveScript)
            moveScript.serialNumbertoFollow = tracker;
        DebugLogger.Instance.Print("serial number of the tracker to follow is " + moveScript.serialNumbertoFollow);
    }
}
