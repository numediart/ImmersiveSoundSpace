/* BSD Licence

 * Copyright (c) 2017-2020, UMONS - CLICK' Living Lab
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * * Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 * * Neither the name of the UMONS - nor CLICK' Living Lab - nor the
 *   names of its contributors may be used to endorse or promote products
 *   derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE REGENTS AND CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnRaycast : MonoBehaviour {

    /// Main camera.
    public Camera mainCamera;
    public float stayTime;
    public float reenterTimeout;

    new AudioSource audio;

    private bool currentlyHit;
    private bool previouslyHit;
    private float hitEnterT0;
    private float hitLeaveT0;

    // Use this for initialization
    void Start () {
        //Fetch the AudioSource from the GameObject
        audio = GetComponentsInChildren<AudioSource>()[0];
    }
	
	// Update is called once per frame
	void Update () {
        // Raycast against the object.
        Ray ray = mainCamera.ViewportPointToRay(0.5f * Vector2.one);
        RaycastHit hit;
        currentlyHit = Physics.Raycast(ray, out hit) && hit.transform == transform;
        if(currentlyHit)
        {
            if (!previouslyHit && Time.time - hitLeaveT0 > reenterTimeout)
            {
                // entering target, reset hitEnter timer
                hitEnterT0 = Time.time;
            }

            if(Time.time - hitEnterT0 >= stayTime && !audio.isPlaying)
            {
                // on target for more than 'stayTime' seconds
                audio.Play();
                DebugLogger.Instance.Print("Play audio from " + this.name);
            }
        }
        else if(previouslyHit)
        {
            // leaving target
            hitLeaveT0 = Time.time;
        }

        previouslyHit = currentlyHit;
    }
}
