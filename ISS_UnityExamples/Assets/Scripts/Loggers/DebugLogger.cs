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

public class DebugLogger : MonoBehaviour {

    public static Text debugConsole;

    #region Singleton Constructors
    static DebugLogger()
    {
    }

    DebugLogger()
    {
    }

    public static DebugLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("DebugLogger").AddComponent<DebugLogger>();
                _textField = debugConsole;
                if(_textField)
                    _logLength = 360 / (_textField.fontSize + _textField.lineSpacing);
            }

            return _instance;
        }
    }
    #endregion

    #region Member Variables
    private static DebugLogger _instance = null;
    private static Text _textField = null;
    private static float _logLength = 12;

    private List<string> lines = new List<string>();
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Print(string str)
    {
        Debug.Log(str);
        if(_textField)
        {
            lines.Add(str);
            if(lines.Count > _logLength - 1)
            {
                lines.RemoveAt(0);
            }
            _textField.text = "Debug :";
            foreach(string l in lines)
            {
                _textField.text += "\n" + l;
            }
            
        }
    }
    #endregion
}
