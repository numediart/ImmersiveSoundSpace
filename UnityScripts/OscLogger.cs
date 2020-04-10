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

public class OscLogger : MonoBehaviour
{

    #region Singleton Constructors
    static OscLogger()
    {
    }

    OscLogger()
    {
    }

    public static OscLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("OscLogger").AddComponent<OscLogger>();
                _textField = GameObject.Find("OscConsole").GetComponent<Text>();
                _logLength = 360 / (_textField.fontSize + _textField.lineSpacing);
            }

            return _instance;
        }
    }
    #endregion

    #region Member Variables
    private static OscLogger _instance = null;
    private static Text _textField = null;
    private static float _logLength = 12;

    private List<string> lines = new List<string>();
    private bool collapse = true;
    private int collapeCount = 0;
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

            if (lines.Count > 0 && collapse)
            {
                if (str.Equals(lines[lines.Count - 1], StringComparison.Ordinal))
                {
                    collapeCount++;
                }
                else
                {
                    lines.Add(str);
                    collapeCount = 0;
                }
            }
            else
            {
                lines.Add(str);
            }

            if (lines.Count > _logLength - 1)
            {
                lines.RemoveAt(0);
            }

            _textField.text = "OSC : ";
            foreach (string l in lines)
            {
                _textField.text += "\n" + l;
                if(collapeCount > 0)
                    _textField.text += "\t" + collapeCount;
            }

        }
    }

    public void SetCollapse(bool c)
    {
        collapse = c;
    }
    #endregion
}
