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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chrono : MonoBehaviour {

    private static float timer;
    public static float Timer { get { return timer; } }
    public static string TimeString {
        get {
            int minutes = (int)timer / 60;
            int seconds = (int)timer % 60;
            int millis = (int)(1000 * (timer - (int) timer));
            return String.Format("{0:00}:{1:00}.{2:0}", minutes, seconds, millis / 100);
        }
    }
    private static bool pause = false;


    void Start()
    {
    }

    void Update()
    {
        if (!pause)
            timer += Time.deltaTime;
    }

    public static void ResetTimer()
    {
        timer = 0.0f;
    }

    public static void Pause()
    {
        pause = true;
    }

    public static void Resume()
    {
        pause = false;
    }
}
