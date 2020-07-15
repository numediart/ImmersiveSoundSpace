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
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionLogger : MonoBehaviour
{

    #region Singleton Constructors
    static PositionLogger()
    {
    }

    PositionLogger()
    {
    }

    public static PositionLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("PositionLogger").AddComponent<PositionLogger>();
                try
                {
                    _textField = GameObject.Find("PositionConsole").GetComponent<Text>();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            return _instance;
        }
    }
    #endregion

    #region Member Variables
    private static PositionLogger _instance = null;
    private static Text _textField = null;
    private static float _logLength = 2;

    private List<string> lines = new List<string>();
    #endregion

    #region Methods
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Print(string str)
    {
        if (_textField)
        {
            lines.Add(str);
            if (lines.Count > _logLength - 1)
            {
                lines.RemoveAt(0);
            }
            _textField.text = "Position :\n";
            foreach (string l in lines)
            {
                _textField.text += l;
            }

        }
    }
    #endregion
}
