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
