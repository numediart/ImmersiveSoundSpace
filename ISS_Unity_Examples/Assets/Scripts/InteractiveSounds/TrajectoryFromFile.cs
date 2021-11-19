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
using System.Globalization;
using UnityEngine;

public class TrajectoryFromFile : MonoBehaviour
{
    public TextAsset trajectoryFile; 
    // csv file created with SaveTrackerTrajectory sketch found at https://github.com/numediart/ISS_Utils
    private string[] lines;
    private int lineIndex;
    private float trajectoryDuration;

    public bool relativeToPosInit;
    private Vector3 posInit;

    public float startTime;
    public bool loop;
    public bool pingPong;
    private int increment = 1;

    private AudioSource[] sources;

    // Start is called before the first frame update
    void Start()
    {
        string text = trajectoryFile.text;  //this is the content as string
        lines = text.Split('\n');

        int endOffset = 1;
        string lastLine = lines[lines.Length - endOffset];
        while(lastLine.Length <= 0)
        {
            endOffset++;
            lastLine = lines[lines.Length - endOffset];
        }

        string[] lastLineValues = lastLine.Split(',');
        int trajectoryDurationMillis;
        int.TryParse(lastLineValues[0], out trajectoryDurationMillis);
        trajectoryDuration = trajectoryDurationMillis / 1000.0f;
        lineIndex = 0;

        posInit = transform.position;

        sources = GetComponentsInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Chrono.Timer > startTime && Chrono.Timer < (startTime + 0.1) && !sources[0].isPlaying)
        {
            string logString = "[" + Chrono.TimeString + "] : ";
            logString += "Play sounds and trajectory from " + name;
            DebugLogger.Instance.Print(logString);
            foreach (AudioSource source in sources)
            {
                source.Play();
            }
        }

        // get current line values
        if (lineIndex >= 0)
        {
            string[] valuesStr = lines[lineIndex].Split(',');
            int millis;
            int.TryParse(valuesStr[0], out millis);

            // go to next line
            if (Chrono.Timer >= startTime + millis / 1000.0f && increment > 0 ||
                   Chrono.Timer - startTime >= trajectoryDuration - millis / 1000.0f && increment < 0)
            {
                if (lines[lineIndex].Length > 0)
                {
                    float[] valuesF = new float[7];
                    for (int i = 1; i < 8; i++)
                    {
                        valuesF[i - 1] = float.Parse(valuesStr[i], CultureInfo.InvariantCulture);
                    }
                    transform.SetPositionAndRotation(new Vector3(valuesF[0], valuesF[1], valuesF[2]) + (relativeToPosInit? posInit : new Vector3()),
                        new Quaternion(valuesF[3], valuesF[4], valuesF[5], valuesF[6]));
                }

                lineIndex += increment;
                if (lineIndex >= lines.Length && increment > 0 || increment < 0 && lineIndex < 0)
                {
                    startTime = Chrono.Timer;
                    if (loop)
                    {
                        if (pingPong)
                        {
                            lineIndex = increment > 0 ? lines.Length - 2 : 0;
                            increment = increment > 0 ? -1 : 1;
                        }
                        else
                        {
                            lineIndex = 0;
                        }
                    }
                    else
                    {
                        if (pingPong)
                        {
                            if (increment == 1)
                            {
                                lineIndex = lines.Length - 2;
                                increment = -1;
                            }
                            else
                            {
                                lineIndex = -1;
                            }
                        }
                        else
                        {
                            lineIndex = -1;
                        }
                    }
                }
            }
        }
    }
}
