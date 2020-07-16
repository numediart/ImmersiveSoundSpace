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

public class PlayWithDelay : MonoBehaviour
{
    public float delay;

    private AudioSource[] sources;
    // Start is called before the first frame update
    void Start()
    {
        sources = GetComponentsInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if it is time to start sound
        if (Chrono.Timer > delay && Chrono.Timer < (delay + 0.1) && !sources[0].isPlaying)
        {
            string logString = "[" + Chrono.TimeString + "] : ";
            logString += "Play sounds from " + name;
            DebugLogger.Instance.Print(logString);
            foreach (AudioSource source in sources)
            {
                source.Play();
            }
        }
    }
}
