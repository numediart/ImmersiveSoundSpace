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


using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
    public delegate void EventDelegate();
    public event EventDelegate tKey;
    public event EventDelegate sKey;

    private void Awake()
    {
        var movableObjects = GameObject.FindObjectsOfType<ObjectTrajectory>();

        foreach( var obj in movableObjects)
        {
            if( obj.name == "SineWave")
            {
                tKey += obj.GetComponent<ObjectTrajectory>().BeginTrajectory;
                sKey += obj.GetComponent<ObjectTrajectory>().EndTrajectory;
            }
        }
    }
    void Start()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            tKey();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            sKey();
        }
    }
}
