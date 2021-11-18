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

public class PlayOnRaycast : MonoBehaviour {

    /// Main camera.
    public Camera userCamera;
    public float stayTime;
    public float reenterTimeout;

    private AudioSource[] sources;

    private bool currentlyHit;
    private bool previouslyHit;
    private float hitEnterT0;
    private float hitLeaveT0;

    // Use this for initialization
    void Start () {
        //Fetch the AudioSource from the GameObject
        sources = GetComponentsInChildren<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        // Raycast against the object.
        Ray ray = userCamera.ViewportPointToRay(0.5f * Vector2.one);
        RaycastHit hit;
        currentlyHit = Physics.Raycast(ray, out hit) && hit.transform == transform;
        if(currentlyHit)
        {
            GetComponent<Renderer>().material.color = Color.red;
            if (!previouslyHit && Time.time - hitLeaveT0 > reenterTimeout)
            {
                // entering target, reset hitEnter timer
                hitEnterT0 = Time.time;
            }

            if(Time.time - hitEnterT0 >= stayTime && !sources[0].isPlaying)
            {
                // on target for more than 'stayTime' seconds
                DebugLogger.Instance.Print("Play audio from " + this.name);
                foreach (AudioSource source in sources)
                {
                    source.Play();
                }
            }
        }
        else if(previouslyHit)
        {
            // leaving target
            hitLeaveT0 = Time.time;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }

        previouslyHit = currentlyHit;
    }
}
