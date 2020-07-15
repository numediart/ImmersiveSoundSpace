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
