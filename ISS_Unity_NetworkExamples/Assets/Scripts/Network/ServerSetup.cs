using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ServerSetup : NetworkBehaviour
{
    public Behaviour[] componentsToEnableOnServer;
    public Behaviour[] componentsToDisableOnServer;
    public GameObject[] ChildrenToActivateOnServer;
    public GameObject[] ChildrenToDeactivateOnServer;

    private bool enableFlag = false;

    private OSCManager oscManager;
    private TrackersManagerOSC trackersManager;

    void OnEnable()
    {
        enableFlag = true;
    }

    void Start()
    {
        if (isServer)
        {
            for (int i = 0; i < ChildrenToActivateOnServer.Length; i++)
            {
                ChildrenToActivateOnServer[i].SetActive(true);
            }
            for (int i = 0; i < ChildrenToDeactivateOnServer.Length; i++)
            {
                ChildrenToDeactivateOnServer[i].SetActive(false);
            }
            for (int i = 0; i < componentsToEnableOnServer.Length; i++)
            {
                componentsToEnableOnServer[i].enabled = true;
            }
            for (int i = 0; i < componentsToDisableOnServer.Length; i++)
            {
                componentsToDisableOnServer[i].enabled = false;
            }
        }
        else // server or other clients
        {
            for (int i = 0; i < ChildrenToActivateOnServer.Length; i++)
            {
                ChildrenToActivateOnServer[i].SetActive(false);
            }
            for (int i = 0; i < ChildrenToDeactivateOnServer.Length; i++)
            {
                ChildrenToDeactivateOnServer[i].SetActive(true);
            }
            for (int i = 0; i < componentsToEnableOnServer.Length; i++)
            {
                componentsToEnableOnServer[i].enabled = false;
            }
            for (int i = 0; i < componentsToDisableOnServer.Length; i++)
            {
                componentsToDisableOnServer[i].enabled = true;
            }
        }
    }


    void Update()
    {
        if (enableFlag)
        {
            // DestroyAllTangibles has to be called within Update function
            if (trackersManager)
            {
                // prevent trying to access old tangibles gameObjects not alive anymore
                trackersManager.DestroyAllTrackers();
            }

            if (isServer)
            {
                oscManager = GetComponentInChildren<OSCManager>();
                oscManager.CreateServers();

                trackersManager = GetComponentInChildren<TrackersManagerOSC>();
                trackersManager.AttachOscServer();
            }
            enableFlag = false;
        }
    }

    private void OnDisable()
    {
        enableFlag = false;
        if (oscManager != null)
        {
            oscManager.StopServers();
        }
    }
}
