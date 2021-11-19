using UnityEngine;

public class PlayerSetTransform : MonoBehaviour
{
    [Tooltip("Tracker to follow serial number \nIf empty, parameters from CLI will be used")]
    public string trackerSerialToFollow;

    [Header("Constraints")]
    [SerializeField]
    private bool _position;

    [SerializeField]
    private bool _rotation;


    private void Start() 
    {
        if(!string.IsNullOrEmpty(trackerSerialToFollow)) return;
        trackerSerialToFollow = CommandLineParser.trackerSerial;
        Debug.Log($"Got to get serial number => <color=cyan>{trackerSerialToFollow}</color>");
    }

    void Update()
    {
        //this could be handled by a tracker manager that broadcast a message or is observed by this behaviour
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
            if(_position) transform.position = trackerToFollow.transform.position;
            if(_rotation) transform.rotation = trackerToFollow.transform.rotation;
        }
    }
    
    private GameObject trackerToFollow;
}