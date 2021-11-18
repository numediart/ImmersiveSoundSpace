using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector: MonoBehaviour
{
    public GameObject serverCam;
    private Vector3 bckPos;
    private Quaternion bckRot;
    public List<Transform> clientPoses = new List<Transform>();

    private int currentCam = -1;

    void Start()
    {
        bckPos = serverCam.transform.position;
        bckRot = serverCam.transform.rotation;
    }



    void Update()
    {
        UpdateClientsList();

        int nextCam = currentCam;
        if (Input.GetKeyDown(KeyCode.Space)) // reset to server cam
        {
            nextCam = -1;
        }
        if (Input.GetKeyDown(KeyCode.PageUp)) // next cam in list
        {
            nextCam++;
            if (nextCam >= clientPoses.Count) nextCam = -1;
        }
        if(Input.GetKeyDown(KeyCode.PageDown)) // previous cam
        {
            nextCam--;
            if (nextCam < -1) nextCam = clientPoses.Count - 1;
        }

        // prevent index out of bound when followed client disconnects
        if (nextCam >= clientPoses.Count)
        {
            nextCam = -1;
        }

        if (nextCam == -1)
        {
            if (currentCam != nextCam)
            {
                serverCam.transform.position = bckPos;
                serverCam.transform.rotation = bckRot;
                serverCam.GetComponent<CameraSpectator>().enabled = true;
            }
        }
        else
        {
            if (currentCam == -1)
            {
                bckPos = serverCam.transform.position; // backup current server cam transform
                bckRot = serverCam.transform.rotation;
            }
            serverCam.transform.position = clientPoses[nextCam].position;
            serverCam.transform.rotation = clientPoses[nextCam].rotation;
            serverCam.GetComponent<CameraSpectator>().enabled = false;
        }

        currentCam = nextCam;
    }


    public void UpdateClientsList()
    {
        clientPoses.Clear();
        foreach(GameObject cli in GameObject.FindGameObjectsWithTag("MirrorClient"))
        {
            clientPoses.Add(cli.transform);
        }
    }
}
