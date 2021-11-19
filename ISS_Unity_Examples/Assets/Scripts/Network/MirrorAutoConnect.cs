using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MirrorAutoConnect : MonoBehaviour
{
    public bool forceServerInEditor;
    private NetworkManager netMan;

    private bool forceServer = false;
    private float lastConnectionAttemptTime;
    public float timeBetweenConnectionAttempts;
    public Text message;


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

        if (Application.isEditor && forceServerInEditor)
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

        if(message)
        {
            message.text = "Connect to server @ " + netMan.networkAddress;
        }
    }

    void Update()
    {
        if (!forceServer && !netMan.isNetworkActive) {
            if (Time.time - lastConnectionAttemptTime > timeBetweenConnectionAttempts)
            {
                Debug.LogWarning("No server found @ " + netMan.networkAddress + ", check you are connected to the same network and the server is running");
                Debug.Log("Trying again to connect to server @ " + netMan.networkAddress);
                if (message)
                {
                    message.text = "No server found @ " + netMan.networkAddress + ",\n" + 
                        "check you are connected to the same network and the server is running.\n" +
                        "Trying again to connect to server @ " + netMan.networkAddress;
                }

                netMan.StartClient();
                lastConnectionAttemptTime = Time.time;
            }
        }
    }
}
