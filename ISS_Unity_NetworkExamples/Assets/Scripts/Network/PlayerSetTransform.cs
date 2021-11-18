using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetTransform : NetworkBehaviour
{
    
    public string trackerSerialToFollow;
    public GameObject trackerToFollow;

    private void Start() 
    {
        trackerSerialToFollow = CommandLineParser.instance.GetComponent<CommandLineParser>().trackerSerial;
    }

    void Update()
    {
        if(trackerToFollow == null)
        {
            GameObject[] trackers = GameObject.FindGameObjectsWithTag("ViveTracker");
            foreach(GameObject vt in trackers) 
            {
                if(trackerSerialToFollow == vt.GetComponent<ViveTrackerSerialNetwork>().trackerSerial)
                    trackerToFollow = vt;
            }
        }
        else 
        {
            transform.position = trackerToFollow.transform.position;
            transform.rotation = trackerToFollow.transform.rotation;
        }
    }
}
