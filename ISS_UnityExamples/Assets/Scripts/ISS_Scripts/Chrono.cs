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
