using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class MirrorAutoConnect : MonoBehaviour
{
    private NetworkManager netMan;

    private bool forceServer = false;
    private float lastConnectionAttemptTime;
    public float timeBetweenConnectionAttempts;


    void Awake()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--server":
                case "-s":
                    forceServer = true;
                    break;
                default:
                    break;
            }
        }

        if (Application.isEditor)
        {
            forceServer = true;
        }
    }

    void Start()
    {
        netMan = NetworkManager.singleton;

        if (!forceServer)
        {
            Debug.Log("Connect to server @ " + netMan.networkAddress);
            netMan.StartClient();
            lastConnectionAttemptTime = Time.time;
        }
        else
        {
            netMan.StartServer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!forceServer && !netMan.isNetworkActive) {
            if (Time.time - lastConnectionAttemptTime > timeBetweenConnectionAttempts)
            {
                Debug.LogWarning("No server found @ " + netMan.networkAddress + ", check you are connected to the same network and the server is running");
                Debug.Log("Trying again to connect to server @ " + netMan.networkAddress);
                
                netMan.StartClient();
                lastConnectionAttemptTime = Time.time;
            }
        }
    }
}
