:: © - 2020 – UMONS - CLICK' Living Lab
:: 
:: Immersive Sound Space of University of MONS – CLICK' Living Lab (Fabien Grisard) is free software: 
:: you can redistribute it and/or modify it under the terms of the 3-Clause BSD licence. 
:: This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
:: without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
:: See the 3-Clause BSD licence License for more details.
:: 
:: ​​​​​You should have received a copy of the 3-Clause BSD licence along with this program.  
:: 
:: Each use of this software must be attributed to University of MONS – CLICK' Living Lab  (Fabien Grisard).


@echo off

:: set path to the executable scenes
set scenes_dir=D:\%USERNAME%\Documents\SceneBuilds

:: set number of scenes to launch (e.g. number of audio headsets)
set nb_scenes=3

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
echo python Tracking/HTCTrackerPositionSender.py --listeners %clients% --origin_serial %origin_serial% --framerate 20
start cmd /C python Tracking/HTCTrackerPositionSender.py --listeners %clients% --origin_serial %origin_serial% --framerate 20
endlocal
