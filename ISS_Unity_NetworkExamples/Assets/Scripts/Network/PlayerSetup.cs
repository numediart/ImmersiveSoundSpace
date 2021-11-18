using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    public GameObject[] ChildrenToActivateOnClient;
    public GameObject[] ChildrenToDeactivateOnClient;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            for (int i = 0; i < ChildrenToActivateOnClient.Length; i++)
            {
                ChildrenToActivateOnClient[i].SetActive(true);
            }
            for (int i = 0; i < ChildrenToDeactivateOnClient.Length; i++)
            {
                ChildrenToDeactivateOnClient[i].SetActive(false);
            }
        }
        else // server or other clients
        {
            for (int i = 0; i < ChildrenToActivateOnClient.Length; i++)
            {
                ChildrenToActivateOnClient[i].SetActive(false);
            }
            for (int i = 0; i < ChildrenToDeactivateOnClient.Length; i++)
            {
                ChildrenToDeactivateOnClient[i].SetActive(true);
            }
        }
    }

}
