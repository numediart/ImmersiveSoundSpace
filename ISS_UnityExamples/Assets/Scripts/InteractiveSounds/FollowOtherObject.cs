using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOtherObject: MonoBehaviour {

    public GameObject objectToFollow;
    public Vector3 offset;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.SetPositionAndRotation(objectToFollow.transform.position, objectToFollow.transform.rotation);
        transform.Translate(offset, Camera.main.transform);
    }
}
