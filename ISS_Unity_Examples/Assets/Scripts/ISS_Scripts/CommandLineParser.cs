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
using System;

public class CommandLineParser : MonoBehaviour
{
    public static string trackerSerial; // The serial number of the Vive Tracker attached to the headphones (will move the mainaudioListener)
    public static int oscPort;

    public static GameObject instance;

    // Use this for initialization
    void Awake()
    {
        if(instance != null) {
            Destroy(this.gameObject);
            return;
        }

        instance = this.gameObject;
        DontDestroyOnLoad(this);

        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            //Debug.Log("ARG " + i + ": " + args[i]);
            //DebugLogger.Instance.Print("ARG " + i + ": " + args[i]);
            switch (args[i])
            {
                case "--tracker":
                case "-t":
                    trackerSerial = args[i + 1];
                    Debug.Log("Got Tracker Serial from command line : " + trackerSerial);
                    break;
                case "--port":
                case "-p":
                    oscPort = Int32.Parse(args[i + 1]);
                    Debug.Log("Got OSC port to listen from command line : " + oscPort);
                    break;
                default:
                    break;
            }
        }
    }
}
