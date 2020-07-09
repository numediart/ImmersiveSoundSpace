# HTC Tracker Position Sender Script

## Requirements
- Windows 10, might work on earlier versions too
- [Steam](https://store.steampowered.com/about/) (if you don't already have one, you will need to create an account)
- [SteamVR](https://store.steampowered.com/app/250820/SteamVR/)
- At least one [HTC Vive Tracker](https://www.vive.com/fr/vive-tracker/) already [paired](https://www.vive.com/us/support/wireless-tracker/category_howto/pairing-vive-tracker.html) to SteamVR
- Two or more HTC Vive Base stations [1.0](https://www.vive.com/fr/accessory/base-station/) or [2.0](https://www.vive.com/fr/accessory/base-station2/) (double check the compatibility with your trackers)
- If you don't want to use the Head Mounted Display (HMD) see the following section

## Using SteamVR without HMD
- Close SteamVR
- Open the file *SteamDirectory\config\steamvr.vrsettings* with a text editor 
    - (*SteamDirectory* is usually *C:\Program Files (x86)\Steam*)
- In the *"steamvr"* section, add the folowing entry: `"requireHmd" : false,`
    - Be careful with the comma, there must be one at the end of each line in the section, except for the last line
    - If needed, check your file with a [JSON validator](https://jsonlint.com/)
- Restart SteamVR

## Installation
- Install [Anaconda](https://www.anaconda.com/distribution/#download-section) with python 3.X
- [Open a command prompt window](https://www.thewindowsclub.com/how-to-open-command-prompt-from-right-click-menu) (not a PowerShell window)
- Create a conda environment named iss with pip support:
`conda create -n iss pip`
- Then activate the newly created environment : `activate iss`
- Install required dependancies : `pip install openvr python-osc numpy-quaternion numba scipy`   
or cd to the *Tracking* folder and run `pip install -r requirements.txt`
- Deactivate the environment : `conda deactivate`
- cd to the root folder of this repo
- To check that all libraries installed correctly, try to launch the calibration script : `calibrate.bat`. If no Vive Tracker is connected to the computer, you should see the following result:
```
Coordinate system is set for Unity
Add OSC listener at 127.0.0.1:9001
origin file not found at <<YOUR_PATH>>\ImmersiveSoundSpace\Tracking\origin_mat.rtm,  
please place the reference and press 'o' key to set a new origin
===========================
Initialize OpenVR ... Impossible to intialize OpenVR
```

### Note on Python libs version
We recommend to install the latest version available but, if for any reason the script doesn't work, try to downgrade the problematic library to the known working version.
- openvr : 1.12
- python-osc : 1.7.4
- numpy-quaternion : 2020.5
- numba : 0.50.1
- scipy : 1.5.1

## Get the Tracker Serial Number


## Calibration
You can use the *calibration.bat* script to create the calibration file (*Tracking/origin_mat.rtm*) that will next be used at each launch to set the origin and axes of your setup. 
To do so :
- In *calibrate.bat file*, @line 18, check that the serial number that will be used to set the origin (given after the `--origin_serial` option) is the same as one of your Vive Trackers.

## First launch

## Use with multiple Unity build instances


## Using the script alone
- Plug in at least one of the USB dongles that comes with the HTC Vive Tracker
- If you didn't add the `"requireHmd" : false` line in *steamvr.vrsettings*, also connect the HMD
- Open a command prompt window
- cd to the script folder
- Activate the *iss* environment : `activate iss`
- Launch the script `python HTCTrackerPositionSender.py`, options are detailled in the next section

### Command line options
```
python HTCTrackerPositionSender.py 
        [-h]
        [--listeners LISTENERS]
        [--origin_serial ORIGIN_SERIAL]
        [--framerate FRAMERATE]
        [--opengl]
        [--steam]
```

All arguments are optional:
- `-h, --help`  
show the help message and exit

- `--listeners LISTENERS`  
The *ipv4:port* of the OSC programs listening. Different listeners must be separated with *;*  
If multiple ports are used on a single ip, it possible to use this form `ip1:port1-1;port1-2;port1-3;ip2:port2-1;port2-2...`  
Example : `--listeners 192.168.0.42:5001;50002;127.0.0.1:9001`
will send values to ports *5001* and *5002* on the computer with IP = *192.168.0.42* and to the port *9001* on *localhost*.  
Default value is `127.0.0.1:9001`

- `--origin_serial ORIGIN_SERIAL`  
The serial number of the tracker used for origin calibration. This option is needed only if you plan to set the origin of your 3D space by pressing *o* key while the script is running. Trackers have a serial number starting with *LHR-* followed by 8 characters in hexadecimal format.  
Example : `--origin_serial LHR-0A1B2C3D`

- `--framerate FRAMERATE`  
Expected framerate as an integer - used to slow down OSC messages as UnityOSC can't handle more than 50 messages per second.  
Default value is 30

- `--opengl`  
Use openGL coordinate system if set, or Unity coordinate system (default) if not set

- `--steam`  
Open SteamVR when the script is launched

### Keystroke interaction
While the script is running, if the command prompt window has the focus, some parameters are controlled by the keys
- *o* :  
Will set a new origin for the coordinate system (and save it in a file called *origin_mat.rtm* in the same folder as the script - overwritting the existing version of the file) as soon as the Tracker with serial = *origin_serial* is tracked. It is recommended to ensure that the origin tracker is already tracked and in the correct position before pressing *o* key.
- *r* :  
Reset the origin. The default origin is one of the base stations.
- *u* :  
Switch between Unity and OpenGl coordinate systems.
- *esc* :
Closes OpenVR cleanly and exit the script.