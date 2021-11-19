using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityOSC;

// Tracker Serial must be the same as the one received in OSC messages

[Serializable]
public class TrackerPrefabAndSerial
{
    public string serial;        // serial received from OSC message (e.g. Vive Tracker or ALT id)
    public GameObject prefab; 
}

[Serializable]
public class TrackerObject
{
    public static readonly int filterSize = 50;
    public GameObject gameObject;
    public Vector3 updatedPos;
    public Quaternion updatedQuat;

    public Queue<Vector3> filterPos;
    public Queue<Quaternion> filterQuat;

    public int trackingStability;
    public System.Diagnostics.Stopwatch sw;

    public TrackerObject()
    {
        sw = new System.Diagnostics.Stopwatch();
        filterPos = new Queue<Vector3>();
        filterQuat = new Queue<Quaternion>();
    }


    public void update(Vector3 p, Quaternion q, int stab)
    {
        updatedPos = p;
        filterPos.Enqueue(p);
        if(filterPos.Count >= filterSize)
        {
            filterPos.Dequeue();
        }
        updatedQuat = q;
        filterQuat.Enqueue(q);
        if(filterQuat.Count >= filterSize)
        {
            filterQuat.Dequeue();
        }
        trackingStability = stab;

        sw.Restart();
    }

    public Vector3 FilteredPos()
    {
        Vector3 averagePos = new Vector3();
        foreach(Vector3 p in filterPos)
        {
            averagePos += p;
        }
        return averagePos / filterPos.Count;
    }

    public Quaternion FilteredQuat()
    {
        Quaternion averageQuat = Quaternion.identity;
        int i = 0;
        foreach (Quaternion q in filterQuat)
        {
            averageQuat = Quaternion.Slerp(averageQuat, q, 1.0f / (i + 1));
            i++;
        }
        return averageQuat;
    }
}


public class TrackersManagerOSC : MonoBehaviour
{
    public string oscServerToListen;
    public List<TrackerPrefabAndSerial> trackersSerialAndPrefab;
    public int destroyTimeoutMs; // after how long (milliseconds) without message the object must be destroyed

    [HideInInspector]
    public Dictionary<string, TrackerObject> trackers;
    private List<string> trackersToCreate;

    private OSCServer serverToListen;

    private readonly object lockObject = new object();
    private static TrackersManagerOSC _instance;
    public static TrackersManagerOSC Instance => _instance;


    private void Awake() 
    {
        if(_instance) return;

        _instance = this;
    }

    public void Start()
    {
        trackers = new Dictionary<string, TrackerObject>();
        trackersToCreate = new List<string>();
    }


    public void AttachOscServer()
    {
        foreach (KeyValuePair<string, ServerLog> pair in OSCHandler.Instance.Servers)
        {
            if (pair.Key == oscServerToListen)
            {
                serverToListen = pair.Value.server;
                serverToListen.PacketReceivedEvent += OnPacketReceived;
                Debug.Log("OSC Server '" + oscServerToListen + "' found on port : " + serverToListen.ServerPort);
            }
        }
        if (serverToListen == null)
        {
            Debug.Log("No server found !");
        }
    }


    void Update()
    {
        lock (lockObject)
        {
            foreach (string s in trackersToCreate)
            {
                if (!trackers.ContainsKey(s))
                    CreateTracker(s);
            }
            trackersToCreate.Clear();
        }

        List<string> toDestroy = new List<string>();
        string debugStr = "";
        foreach(KeyValuePair<string, TrackerObject> tObj in trackers)
        {
            debugStr += tObj.Key + " [" + tObj.Value.gameObject.name + "], ";
            tObj.Value.gameObject.transform.position = tObj.Value.updatedPos;
            tObj.Value.gameObject.transform.rotation = tObj.Value.updatedQuat;

            if (tObj.Value.sw.ElapsedMilliseconds > destroyTimeoutMs)
            {
                toDestroy.Add(tObj.Key);
            }
        }
        //Debug.Log(debugStr);
        foreach(string s in toDestroy)
        {
            DestroyTracker(s);
        }
    }


    private GameObject GetTrackerPrefab(string trackerSerial)
    {
        foreach(TrackerPrefabAndSerial track in trackersSerialAndPrefab)
        {
            if(track.serial == trackerSerial)
            {
                return track.prefab;
            }
        }

        return null;
    }


    public TrackerObject GetTracker(string serial)
    {
        if(trackers.ContainsKey(serial))
            return trackers[serial];
        return null;
    }


    public void CreateTracker(string trackerSerial)
    {
        GameObject prefab = GetTrackerPrefab(trackerSerial);
        string gameObjectName = trackerSerial;
        if (prefab != null && gameObjectName != null) {
            GameObject go = Instantiate(prefab);
            if(go.tag == "ViveTracker")
                go.GetComponent<ViveTrackerSerialNetwork>().trackerSerial = trackerSerial;
            go.name = gameObjectName;

            TrackerObject newTracker = new TrackerObject();
            newTracker.gameObject = go;

            trackers.Add(trackerSerial, newTracker);

            Debug.Log("Create tracker " + trackerSerial);

            // tell it to player manager
            PlayerManager.Instance.SetPlayerTrackerObject(trackerSerial, newTracker);
        }
    }


    public void DestroyTracker(string serial)
    {
        if (trackers.ContainsKey(serial))
        {
            Debug.Log("Remove tracker " + serial);
            Destroy(trackers[serial].gameObject);
            trackers[serial].sw.Stop();

            trackers.Remove(serial);
            PlayerManager.Instance.SetPlayerTrackerObject(serial, null);
        }
    }


    public void DestroyAllTrackers()
    {
        List<string> toDestroy = new List<string>(trackers.Keys);
        foreach (string s in toDestroy)
        {
            DestroyTracker(s);
        }
        trackers.Clear();
    }


    void OnPacketReceived(OSCServer server, OSCPacket packet)
    {
        HandlePacket(packet);
    }


    void HandlePacket(OSCPacket packet)
    {
        if (packet.IsBundle())
        {
            foreach (OSCMessage mes in packet.Data)
            {
                HandleMessage(mes);
            }
        }
        else
        {
            HandleMessage(packet);
        }
    }


    void HandleMessage(OSCPacket mes)
    {
        //Debug.Log("message received : " + mes.Address.ToString());
        switch (mes.Address.ToString().ToLower())
        {
            case "/iss/tracker":
                string serial = mes.Data[0].ToString();
                if (GetTrackerPrefab(serial) != null)
                {
                    if (!trackers.ContainsKey(serial))
                    {
                        lock (lockObject)
                        {
                            trackersToCreate.Add(serial);
                        }
                    }
                    else if (mes.Data.Count >= 8)
                    {
                        Vector3 p = new Vector3();
                        p.x = float.Parse(mes.Data[1].ToString());
                        p.y = float.Parse(mes.Data[2].ToString());
                        p.z = float.Parse(mes.Data[3].ToString());

                        Quaternion q = new Quaternion();
                        q.x = float.Parse(mes.Data[4].ToString());
                        q.y = float.Parse(mes.Data[5].ToString());
                        q.z = float.Parse(mes.Data[6].ToString());
                        q.w = float.Parse(mes.Data[7].ToString());

                        int stab = -1;
                        if(mes.Data.Count > 8)
                            stab = int.Parse(mes.Data[8].ToString());

                        TrackerObject tObj = trackers[serial];
                        tObj.update(p, q, stab);
                    }
                }
                break;
            default:
                break;
        }
    }
}
