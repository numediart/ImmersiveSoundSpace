# HTC Tracker Position Sender Script

## Requirements
- Windows 10, might work on earlier versions too
- [Steam](https://store.steampowered.com/about/) (if you don't already have an account, you will need to create one)
- [SteamVR](https://store.steampowered.com/app/250820/SteamVR/) (tested until version 1.20.4, might work with newer versions)
- At least one [HTC Vive Tracker](https://www.vive.com/fr/vive-tracker/) already [paired](https://www.vive.com/us/support/wireless-tracker/category_howto/pairing-vive-tracker.html) to SteamVR
- Two or more HTC Vive Base stations [1.0](https://www.vive.com/fr/accessory/base-station/) or [2.0](https://www.vive.com/fr/accessory/base-station2/) (double check the compatibility with your trackers)
- If you don't want to use the Head Mounted Display (HMD) see the following section

### Using SteamVR without HMD
- Close SteamVR
- Open the file *SteamDirectory\config\steamvr.vrsettings* with a text editor 
    - (*SteamDirectory* is usually *C:\Program Files (x86)\Steam*)
- In the *"steamvr"* section, add the folowing entry: `"requireHmd" : false,`
    - Be careful with the comma, there must be one at the end of each line in the section, except for the last line
    - If needed, check your file with a [JSON validator](https://jsonlint.com/)
- Restart SteamVR


## Installation
- Install [Anaconda](https://www.anaconda.com/distribution/#download-section) with python 3.X (known to work with python 3.7.x and 3.8.1)
    - **Important !** Tick the option to add Anaconda to your system Path during the installation
- [Open a command prompt window](https://www.thewindowsclub.com/how-to-open-command-prompt-from-right-click-menu) (not a PowerShell window)
- Create a conda environment named iss with pip support:
`conda create -n iss pip python=3.8.1`
- Then activate the newly created environment : `activate iss`
- cd to the *Tracking* folder and run `pip install -r requirements.txt`
- Deactivate the environment : `conda deactivate`
- cd to the root folder of this repo
- To check that all libraries installed correctly, try to launch the calibration script : `calibrate.bat`. If no Vive tracker USB dongle is connected to the computer, you should see the following result:
```
Coordinate system is set for Unity
Add OSC listener at 127.0.0.1:9001
origin file not found at <<YOUR_PATH>>\ImmersiveSoundSpace\Tracking\origin_mat.rtm,  
please place the reference and press 'o' key to set a new origin
===========================
Initialize OpenVR ... Impossible to intialize OpenVR
```

### Note on Python libs version
If for any reason the script doesn't work, try to downgrade the problematic library to the known working version.
- openvr : 1.12
- python-osc : 1.7.4
- numpy-quaternion : 2020.5
- numba : 0.50.1
- scipy : 1.5.1
- opencv-python : 4.5.4.60


## Trackers

### Connect the trackers
- Open a command prompt window
- cd to the root folder of this repo

For each tracker : 
- Charge the tracker battery
- Plug the tracker USB dongle to your computer
- Launch the script `calibrate.bat`
- Switch on the tracker by pushing on its central switch for 1 second
- The tracker should start, its LED turn blue, then green; *It's connected and ready to be tracked!*
- You can connect another tracker without needing to remove this one

**If the LED stays blue :**
- Quit the script by pressing the *escape* key when the command prompt has the focus
- Launch SteamVR
- Click on the ![steamVR settings](../img/steamVROptionTab.png) icon on top of the SteamVR window
- Select *Devices* > *Pair Controller*.
- Select *HTC Vive Tracker* device
- Follow the on screen instructions

### Get the Tracker Serial Number
The script will send OSC messages containing the tracker serial number as first argument (see [OSC messages format](#osc-messages-format) for more details).  
This number takes the form *LHR-XXXXXXXX* whers the *X* are replaced with hexadecimal value.
- Open a command prompt window
- cd to the root folder of this repo
- Plug the tracker USB dongle to your computer
- Launch the script `calibrate.bat`
- When the tracker is tracked, a message will show in the command prompt window, giving you the serial
```
New tracker found : LHR-01234567
time :    267.9, fps : 24.8, nb trackers : 1
```
- Alternatively, you can use [this tool](https://github.com/numediart/ISS_Utils/tree/master/OSCListenerProcessing) to display several trackers and their positions

**We strongly advise to put the serial on a tag, on each tracker and its paired dongle.** Makes your life easier for the next steps :wink: 

### Prevent the tracker from shuting down
- Launch SteamVR
- Click on the ![steamVR settings](../img/steamVROptionTab.png) icon on top of the SteamVR window
- Select *Settings*.
- On the *Start/Stop* tab, find the field *Turn off controllers after* and select *Never*

Alternatively, you can edit the *steamvr.vrsettings* file. In the *"power"* section, modify this entry :  
`"turnOffControllersTimeout" : 0` (0 means never)

## Calibration

### Single point calibration
You can use the *calibration.bat* script to create the calibration file (*Tracking/origin_mat.rtm*) that will next be used at each launch to set the origin and axes of your setup. 
To do so :
- In *calibrate.bat file*, @line 19, check that the serial number that will be used to set the origin (given after the `--origin_serial` option) is the same as one of your Vive Trackers.
- Place the tracker on the floor, at the position you want to be the origin (the LED on the tracker correspond to the z axis - forward - in Unity)
- Check the tracker is tracked by the system and press the 'o' key on your keyboard
- If the calibration is successful, you will see the calibration matrix in the output : 
```
set new origin :
Single point calibration
[[-0.39882466 -0.9069581  -0.13552049  0.65755832]
 [ 0.05969087  0.12179389 -0.9907589  -1.15152502]
 [ 0.91508245 -0.40322846  0.00556273 -1.88850129]
 [ 0.          0.          0.          1.        ]]
```
The calibration file must have been created in the *Tracking* folder and will be used each time the *HTCTrackerPositionSender.py* script is called.

### Multi-points calibration
To increase angular precision when calibration, for example to align the Vive Tracker referential with another tracking system, you can use multi-points calibration. To do so, add the option `--real_world_points` to the command line, followed by a path to a JSON file containing an array of points. See [RealWorldPoint.json](RealWorldPoint.json) for an example.

The JSON file consist in a JSON Array of points represented by an id (used for convenience) and coordinates on each axis. The coordinates must be coherent with the coordinates system currently used by the program (Unity, by default). You must define at least 3 points on the floor, but we recommend to define 4 points on the floor plus one at a known height. If there is no point outside of floor plan, the program will create a virtual one using a cross product.

- In *calibrate.bat file*, comment out @line 19, uncomment @line 22, check that the serial number that will be used to set the origin (given after the `--origin_serial` option) is the same as one of your Vive Trackers.
- Check the tracker is tracked by the system and press the 'o' key on your keyboard
- A message will appear in the console, asking you to put the calibration on the first point and then press the 'n' key.
- Then repeat this operation for each calibration point.
- After the last point, if the calibration is successful, you will see the calibration matrix in the output : 
```
set new origin :
Multi-points calibration
Place your origin tracker on point
{'id': 'A', 'x': -0.333, 'y': 0.0, 'z': 0.333}
and press 'n'
[...]
Computing calibration with 4 points
[[-0.39882466 -0.9069581  -0.13552049  0.65755832]
 [ 0.05969087  0.12179389 -0.9907589  -1.15152502]
 [ 0.91508245 -0.40322846  0.00556273 -1.88850129]
 [ 0.          0.          0.          1.        ]]
```
The calibration file must have been created in the *Tracking* folder and will be used each time the *HTCTrackerPositionSender.py* script is called.

## Use with multiple Unity build instances
Once you have created the calibration file, it is possible to automate the launch of several intances (lets say 3 for this exemple) of Unity builds with the *launch.bat* script.
- In Unity, build your application 3 times with different exe names. Ensure that you use the *CommandLineParser.cs* script in your application.
- Place these 3 builds in the same directory

In *launch.bat*
- @line 17 : change the `scene_dir` variable so it matches where you put the builds
- @line 20 : set the number of instances to launch
- @lines 31 to 41 : For each build, specify the path the exe file (relative to `scene_dir`, the OSC port listening and the serial number of the tracker attached to the headphones
- @line 44 : you can give the serial of the tracker used for calibration if you need to recalibrate during live. In that case, you will need to put the tracker on the floor, at the origin and press the 'o' key.
- @line 62 : Check that the framerate is not set to high. Decrease its value if needed.

You're ready to launch :smile:


### Routing audio
If you use several instances of Unity builds, you need to route the audio streams, so that each headphone receives the right stream. Several tools can do this job, they are presented below. If you plan to use a third party program, do not forget to call it from the *launch.bat* script.

#### Windows 10 Audio settings
- Right click on the sound icon in your task bar
- Select *open sound settings*
- Scroll down and click on *Advanced sound setting*
- For each application producing sound, you can select the sound output

*This method does not allow to save the routing, so it will be lost at next boot. It is convenient for testing purpose or one-day exhibitions.*

#### AudioRouter
Go to the [this fork](https://github.com/a-sync/audio-router/releases/) of AudioRouter and download the latest release. Follow the given instructions to route the audio streams.

*This fork seems to have re-implemented auto-routing (e.g. saved routing). Not tested but it is the only option we found for auto booting PC in several-days-long exhibition*

#### CheVolume
Not tested but [cheVolume](http://www.chevolume.com/) is reported to be a stable alternative. It is currently sold at 10$.


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
        [--real_world_points REAL_WORLD_POINTS]
        [--framerate FRAMERATE]
        [--openvr_coordinates]
        [--steam]
        [--oscpattern OSCPATTERN]
        [--bundles]
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

- `--real_world_points`  
The JSON file containing list of points to use for origin calibration.

- `--framerate FRAMERATE`  
Expected framerate as an integer - used to slow down OSC messages as UnityOSC can't handle more than 50 messages per second.  
Default value is 30

- `--openvr_coordinates`  
Use openVR coordinate system if set, or Unity coordinate system (default) if not set

- `--steam`  
Open SteamVR when the script is launched

- `--oscpattern`  
The OSC pattern used to send messages, default is '/iss/tracker'

- `--bundles`  
Send single frame messages as an OSC bundle

### Keystroke interaction
While the script is running, if the command prompt window has the focus, some parameters are controlled by the keys
- *o* :  
If in single point calibration mode, will set a new origin for the coordinate system (and save it in a file called *origin_mat.rtm* in the same folder as the script - overwritting the existing version of the file) as soon as the Tracker with serial = *origin_serial* is tracked. It is recommended to ensure that the origin tracker is already tracked and in the correct position before pressing *o* key.  
If in multi-points calibration mode, starts the calibration process.
- *n* :  
In multi-points calibration mode, save the current tracker position and ask to move it to the next calibration point.
- *r* :  
Reset the origin. The default origin is one of the base stations.
- *u* :  
Switch between Unity and OpenVR coordinate systems.
- *esc* :
Closes OpenVR cleanly and exit the script.

### OSC messages format
If you plan to create your own listener, here is the definition of the OSC messages sent by the python script. There is a new message for each tracker, at each frame. That means, if your setup uses 3 trackers and `--framerate` option is set to 30, you will actually have to manage 90 OSC messages per second. 

An example of a Processing sketch reading this OSC messages is given [in this other repo](https://github.com/numediart/ISS_Utils/tree/master/OSCListenerProcessing).

OSC message : 
`/iss/tracker LHR-752E19DB -0.65759104 1.8885365 -1.1515425 -0.3443579 -0.566191 -0.6154638 -0.4266728`

- Address pattern : `/iss/tracker`
- Typetag : `sfffffff`
- The 3 first float values represent the position (respectively x, y, z in the Unity coordinate system) in the calibrated space, in meters
- The 4 later values represent the rotation as a quaternion (respectively qx, qy, qz, qw)
