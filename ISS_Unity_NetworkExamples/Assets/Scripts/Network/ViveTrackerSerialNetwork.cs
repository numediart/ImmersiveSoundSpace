using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ViveTrackerSerialNetwork : NetworkBehaviour
{
    [SyncVar]
    public string trackerSerial;
}
