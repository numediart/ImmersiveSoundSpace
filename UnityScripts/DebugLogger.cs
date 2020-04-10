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

using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogger : MonoBehaviour {

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
                _textField = GameObject.Find("DebugConsole").GetComponent<Text>();
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
