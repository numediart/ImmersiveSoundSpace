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

public enum Trajectory
{
    Linear = 0,
    Ellipse = 1
};

public class MovingObject : MonoBehaviour {

    public Trajectory trajectory = Trajectory.Linear;
    public Vector3 point1;
    public Vector3 point2;
    public float speed;

    private bool direction = true;


	// Use this for initialization
	void Start () {
        transform.position = point1;
        transform.LookAt(point2);
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 lastPos = transform.position;
        Vector3 newPos = lastPos;
        Quaternion newRot = transform.rotation;

        Vector3 target = lastPos;
        float step = 0;

        switch (trajectory)
        {
            case Trajectory.Linear:
                if (direction)
                    target = point2;
                else
                    target = point1;
                step = speed * Time.deltaTime;
                newPos = Vector3.MoveTowards(lastPos, target, step);
                newRot.SetLookRotation(target - lastPos);
                if (Vector3.Distance(target, newPos) < 0.4)
                    direction = !direction;
                break;
            case Trajectory.Ellipse:
                Transform t = transform;
                t.Rotate(new Vector3(0, 1, 0), 0.2f);
                t.Translate(new Vector3(0.0f, 0.0f, 1.0f));
                target = t.position;
                step = speed * Time.deltaTime;
                newPos = Vector3.MoveTowards(lastPos, target, step);
                newRot.SetLookRotation(target - lastPos);
                break;
            default:
                break;
        }

        transform.position = newPos;
        transform.rotation = newRot;
    }
}
