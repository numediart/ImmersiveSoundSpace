using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[DefaultExecutionOrder(100)]
public class DisableOnClients : NetworkBehaviour
{

    private bool enableFlag = false;

    void OnEnable()
    {
        enableFlag = true;
    }

    void Update()
    {
        if (enableFlag)
        {
            if (isClientOnly)
            {
                gameObject.SetActive(false);
                enableFlag = false;
            }
            else
            {
                gameObject.SetActive(true);
                enableFlag = false;
            }
        }
    }


    private void OnDisable()
    {
        enableFlag = true;
    }
}
