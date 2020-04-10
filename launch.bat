:: BSD Licence
::
:: Copyright (c) 2017-2020, UMONS - CLICK' Living Lab
:: All rights reserved.
:: Redistribution and use in source and binary forms, with or without
:: modification, are permitted provided that the following conditions are met:
:: 
:: * Redistributions of source code must retain the above copyright
::   notice, this list of conditions and the following disclaimer.
:: * Redistributions in binary form must reproduce the above copyright
::   notice, this list of conditions and the following disclaimer in the
::   documentation and/or other materials provided with the distribution.
:: * Neither the name of the UMONS - nor CLICK' Living Lab - nor the
::   names of its contributors may be used to endorse or promote products
::   derived from this software without specific prior written permission.
:: 
:: THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS ``AS IS'' AND ANY
:: EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
:: WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
:: DISCLAIMED. IN NO EVENT SHALL THE REGENTS AND CONTRIBUTORS BE LIABLE FOR ANY
:: DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
:: (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
:: LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
:: ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
:: (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
:: SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


@echo off

:: set path to the executable scenes
set scenes_dir=D:\%USERNAME%\Documents\SceneBuilds

:: set number of scenes to launch (e.g. number of audio headsets)
set nb_scenes=1

:: clients ip is always 127.0.0.1 (localhost) if running this script
set clients=127.0.0.1:

:: set configuration for each client
:: relative path to the executable scene,
:: port on which this app must listen incomming OSC
:: tracker serial the app is following

:: !!first client is at index 1!!
set config[1].Scene=scene01\AudioScene01.exe
set config[1].Port=5005
set config[1].Tracker=LHR-0ABDF9E5

set config[2].Scene=scene02\AudioScene02.exe
set config[2].Port=5006
set config[2].Tracker=LHR-18B6CE65

set config[3].Scene=scene03\AudioScene03.exe
set config[3].Port=5007
set config[3].Tracker=LHR-08B84174

:: set origin tracker serial if space calibration is needed
set origin_serial=LHR-0EBE46E8

:: set anaconda environment for the python script
call activate iss


setlocal ENABLEDELAYEDEXPANSION
for /l %%i in (1, 1, %nb_scenes%) do (
	:: start scenes listening on the righ ports
	echo "%scenes_dir%\!config[%%i].Scene!" -p !config[%%i].Port! -t !config[%%i].Tracker!
	start cmd /C "%scenes_dir%\!config[%%i].Scene!" -p !config[%%i].Port! -t !config[%%i].Tracker!
	sleep 15
	if %%i==1 (call set "clients=%%clients%%!config[%%i].Port!") else (call set "clients=%%clients%%;!config[%%i].Port!")
)

timeout /t 30
:: start vive tracking
echo python Tracking/HTCTrackerPositionSender.py --clients %clients% --origin_serial %origin_serial% --fps 20
start cmd /C python Tracking/HTCTrackerPositionSender.py --clients %clients% --origin_serial %origin_serial% --fps 20
endlocal