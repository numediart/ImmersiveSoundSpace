// © - 2020 – UMONS - CLICK' Living Lab
// 
// Immersive Sound Space of University of MONS – CLICK' Living Lab (Fabien Grisard) is free software: 
// you can redistribute it and/or modify it under the terms of the 3-Clause BSD licence. 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the 3-Clause BSD licence License for more details.
// 
// ​​​​​You should have received a copy of the 3-Clause BSD licence along with this program.  
// 
// Each use of this software must be attributed to University of MONS – CLICK' Living Lab  (Fabien Grisard).

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryFromCheckpoints: MonoBehaviour {

    public float startTime;
    public float duration = 5.0f; // trajectory duration in seconds
    public Vector3 endPoint; // end of trajectory
    public List<Vector3> checkpoints;
    private float trajectoryLength;
    private float speed;
    //public TextAsset recordFile;

    public bool showTrail = true;
    public float trailDisplayTime = 5;

    private bool isLaunched = false;
    private bool hasPlayed = false;
    private float trajectoryT0 = 0;
    private int checkpointIndex = 0;
    private float vecMagDiff = 0.05f;
    private Vector3 target;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private AudioSource[] sources;

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        trajectoryLength = 0.0f;
        Vector3 previousPoint = transform.position;
        foreach(Vector3 point in checkpoints)
        {
            trajectoryLength += Vector3.Distance(point, previousPoint);
            previousPoint = point;
        }
        trajectoryLength += Vector3.Distance(endPoint, previousPoint);
        speed = trajectoryLength / duration;

        sources = GetComponentsInChildren<AudioSource>();
    }

    private void OnDrawGizmos() 
    {
        for (int i = 0; i < checkpoints.Count; i++)
        {
            Gizmos.color =  Color.red;
            Gizmos.DrawSphere(checkpoints[i], 0.2f);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLaunched && !hasPlayed && Chrono.Timer >= startTime)
        { 
            BeginTrajectory();
        }

        if (isLaunched)
        {
            bool newTarget = false;
            if(checkpointIndex >= checkpoints.Count)
            {
                if (target != endPoint)
                {
                    target = endPoint;
                    newTarget = true;
                }
            }
            else
            {
                if (target != checkpoints[checkpointIndex])
                {
                    target = checkpoints[checkpointIndex];
                    newTarget = true;
                }
            }

            if (newTarget) transform.LookAt(target);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            Vector3 diff = target - transform.position;
            if(diff.magnitude < vecMagDiff)
            {
                if(target != endPoint) checkpointIndex++;
                else EndTrajectory();
            }

            if (sources.Length >= 0) if (!sources[0].isPlaying)
            {
                EndTrajectory();
            }
        }
    }


    public void BeginTrajectory()
    {
        if (!isLaunched)
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            trajectoryLength = 0.0f;
            Vector3 previousPoint = transform.position;
            foreach (Vector3 point in checkpoints)
            {
                trajectoryLength += Vector3.Distance(point, previousPoint);
                previousPoint = point;
            }
            trajectoryLength += Vector3.Distance(endPoint, previousPoint);
            speed = trajectoryLength / duration;

            string logString = "[" + Chrono.TimeString + "] : ";
            logString += "Begin trajectory ";
            logString += sources.Length >= 0 ? "from " : "and play audio from ";
            logString += this.name;
            DebugLogger.Instance.Print(logString);

            isLaunched = true;
            trajectoryT0 = Time.time;
            foreach (AudioSource source in sources)
            {
                source.Play();
            }
            gameObject.AddComponent<TrailRenderer>();
            TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.time = trailDisplayTime;
            trail.startWidth = 0.4f;
            trail.endWidth = 0.1f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
                );
            trail.colorGradient = gradient;
        }
    }


    public void EndTrajectory()
    {
        if (isLaunched)
        {
            hasPlayed = true;
            string logString = "End trajectory ";
            logString += sources.Length >= 0 ? "from " : "and stop audio from ";
            logString += this.name;
            logString += " in " + (Time.time - trajectoryT0) + " seconds";
            DebugLogger.Instance.Print(logString);
            isLaunched = false;
            foreach (AudioSource source in sources)
            {
                source.Stop();
            }
            Destroy(gameObject.GetComponent<TrailRenderer>());
        }
    }

    public void ResetToDefault()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        string logString = "[" + Chrono.TimeString + "] : ";
        logString += this.name;
        logString += " is back to its intial position";
        DebugLogger.Instance.Print(logString);
        isLaunched = false;
        foreach (AudioSource source in sources)
        {
            source.Stop();
        }
        Destroy(gameObject.GetComponent<TrailRenderer>());
    }
}
