# Unity Scripts for ImmersiveSoundSpace

## Requirements
- [Unity](https://store.unity.com/) (Tested with version 2017.4, might work with newer versions)
- [UnityOSC](https://github.com/jorgegarcia/UnityOSC) for OSC Messages reception and parsing
- (recommended) [Google Resonance Plugin](https://resonance-audio.github.io/resonance-audio/develop/unity/getting-started) for sound spatialization and echo / reflection / attenuation calculation
- (optional) [Flee](https://github.com/mparlak/Flee) for parsing mathematical expressions - used to move objects in the scene with trajectory equations

## Scripts


### Main utilities

#### CommandLineParser
A simple script to call the compiled Unity application from the command line (and so from batch file) with arguments. It allows to specify :
- the serial number of the Tracker to follow for the main camera (and so the audio listener)
- the port to listen for incomming OSC messages (this script creates the server)

Other arguments can be implemented quite easily.  
To use it inside the editor, replace `string[] args` with hard coded values.

#### EventManager
A simple event manager that allow to control object trajectories with *t* (start) and *s* (stop) keys. Other event can be implemented quite easily.


### Loggers
The *logger* scripts are used to display information on the UI in the compiled application. Their goal is to substitute to the Debug.Log function from Unity. Each of them require a UI panel with a TextField.

#### OscLogger
Used to display incomming OSC messages. It is possible to collapse similar messages in a single line.  
Used like this : `OscLogger.Instance.Print("A string to print");`  
For example, it is called from MoveFromOSC script.

#### DebugLogger
Used to substitute to the Debug.Log function from Unity.  
Used like this : `DebugLogger.Instance.Print("A string to print");`  

#### PositionLogger
Used to display X, Y, Z position from an object on a single line.  
Used like this : `PositionLogger.Instance.Print("A string to print");`  

#### LogObjectPosition
Script to attach to the main camera. It will display its position on the PositionLogger panel.


### Interaction with objects

#### MoveFromOSC
Script to attach to the main camera and every object that must follow a HTC Vive tracker. It will parse the OSC messages comming from the Python script and move the object they are attached to accordingly.

#### ObjectTrajectory
Needs [Flee](https://github.com/mparlak/Flee) library to work.  
Allow to program a trajectory based on equations on x, y and z.  
For example, an equation of an helicoidal trajectory where *t* is time parameter: 
```
public string xTrajectory = "4 * cos(0,2 * 2 * pi * t)";
public string yTrajectory = "0,5 * t";
public string zTrajectory = "4 * sin(0,2 * 2 * pi * t)";
```
The beggining of the trajectory will be either triggered at the scene loading or with the 
EventManager. The object this script is attached to will have a colored trail behind it.

#### PlayOnRaycast
A simple script to attach to and object with boxcollider and audio source that will play the audio file if the main camera point at the object for a certain period of time.


