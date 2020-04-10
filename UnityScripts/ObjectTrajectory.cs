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
using Flee.PublicTypes;
using UnityEngine.EventSystems;

public class ObjectTrajectory : MonoBehaviour {

    public bool beginOnAwake = false;
    public string xTrajectory = "4 * cos(0,2 * 2 * pi * t)";
    public string yTrajectory = "0,5 * t";
    public string zTrajectory = "4 * sin(0,2 * 2 * pi * t)";

    public bool initialPositionAsCenter;

    public string endCondition = "false";

    public bool showTrail = true;
    public float trailDisplayTime = 5;

    private ExpressionContext context;
    private IGenericExpression<double> xExpression;
    private IGenericExpression<double> yExpression;
    private IGenericExpression<double> zExpression;
    private IGenericExpression<bool> endExpression;

    private bool isLaunched = false;
    private float trajectoryT0 = 0;

    private Vector3 posInit;

    private new AudioSource audio;

    // Use this for initialization
    void Start () {

        posInit = transform.position;

        // Define the context of our expression
        context = new ExpressionContext();
        // Allow the expression to use all static public methods of System.Math
        context.Imports.AddType(typeof(System.Math));
        // define variables for expression parser
        context.Variables["t"] = Time.time - trajectoryT0;
        context.Variables["x"] = posInit.x;
        context.Variables["y"] = posInit.y;
        context.Variables["z"] = posInit.z;
        // Create a generic expression that evaluates to an Object
        xExpression = context.CompileGeneric<double>(xTrajectory);
        yExpression = context.CompileGeneric<double>(yTrajectory);
        zExpression = context.CompileGeneric<double>(zTrajectory);
        endExpression = context.CompileGeneric<bool>(endCondition);

        //Fetch the AudioSource from the GameObject
        audio = GetComponentsInChildren<AudioSource>()[0];

        if (beginOnAwake)
            BeginTrajectory();
    }

    // Update is called once per frame
    void Update () {
        if (isLaunched)
        {
            context.Variables["t"] = Time.time - trajectoryT0;
            float x = (float) xExpression.Evaluate();
            float y = (float) yExpression.Evaluate();
            float z = (float) zExpression.Evaluate();
            Vector3 pos = new Vector3(x, y, z);
            if (initialPositionAsCenter)
                pos += posInit;

            transform.LookAt(pos);
            transform.position = pos;

            context.Variables["x"] = pos.x;
            context.Variables["y"] = pos.y;
            context.Variables["z"] = pos.z;
            bool end = endExpression.Evaluate();

            if (!audio.isPlaying || end)
            {
                EndTrajectory();
            }
        }
    }

    public void BeginTrajectory()
    {
        if(!isLaunched)
        { 
            DebugLogger.Instance.Print("Begin trajectory and play audio from " + this.name);
            isLaunched = true;
            trajectoryT0 = Time.time;
            audio.Play();
            gameObject.AddComponent<TrailRenderer>();
            TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.time = trailDisplayTime;
            trail.startWidth = 0.4f;
            trail.endWidth = 0.1f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.blue, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
                );
            trail.colorGradient = gradient;
        }
    }

    public void EndTrajectory()
    {
        if (isLaunched)
        {
            DebugLogger.Instance.Print("End trajectory and stop audio from " + this.name);
            isLaunched = false;
            transform.position = posInit;
            transform.rotation = new Quaternion();
            audio.Stop();
            Destroy(gameObject.GetComponent<TrailRenderer>());
        }
    }
}
