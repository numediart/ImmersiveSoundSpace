using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    public GameObject[] ChildrenToActivateOnLocalPlayer;
    public GameObject[] ChildrenToDeactivateOnLocalPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            for (int i = 0; i < ChildrenToActivateOnLocalPlayer.Length; i++)
            {
                ChildrenToActivateOnLocalPlayer[i].SetActive(true);
            }
            for (int i = 0; i < ChildrenToDeactivateOnLocalPlayer.Length; i++)
            {
                ChildrenToDeactivateOnLocalPlayer[i].SetActive(false);
            }
        }
        else // server or other clients
        {
            for (int i = 0; i < ChildrenToActivateOnLocalPlayer.Length; i++)
            {
                ChildrenToActivateOnLocalPlayer[i].SetActive(false);
            }
            for (int i = 0; i < ChildrenToDeactivateOnLocalPlayer.Length; i++)
            {
                ChildrenToDeactivateOnLocalPlayer[i].SetActive(true);
            }
        }
    }

}
