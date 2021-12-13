# Unity Examples for Immersive Sound Space

## Requirements
- [Unity](https://store.unity.com/) (Developped with version 2020.3.22)
- [UnityOSC](https://github.com/jorgegarcia/UnityOSC) for OSC Messages reception and parsing (the code is already included in this project). Released under the MIT License.
- (For InteractiveExampleScene) [Google Resonance Plugin](https://resonance-audio.github.io/resonance-audio/develop/unity/getting-started) for sound spatialization and echo / reflection / attenuation calculation. Licensed under the Apache License, Version 2.0.
- (For Network examples) [Mirror](https://github.com/vis2k/Mirror), MIT License.

## Mono player Scenes

### SimpleScene
A scene where a tracker moves the main camera. Use it to test if the tracking is correct and the communication with Unity is working.


### LoggerScene
Same as SimpleScene, but with information displayed directly on the screen by using the Logger Scripts and a UI canvas. Optimized for 960 x 540 resolution (Quarter of a fullHD screen).

### InteractiveExampleScene
Same as LoggerScene, plus an AudioScene, containing a [Google Resonance AudioRoom](https://resonance-audio.github.io/resonance-audio/develop/unity/developer-guide#room-effects-in-unity) and five GameObjects with [AudioSource](https://resonance-audio.github.io/resonance-audio/develop/unity/developer-guide#add-a-sound-source-to-your-scene). [[Sound files Credits](Assets/Sounds/Readme.md)]
- *AmbianceAmbix* : An ambisonic soundfield. Sound by Joseph Anderson.
- *Seagull* : A GameObject that will play seagull sound if looked at. Sound by poodaddy69.
- *Whisper* : A GameObject that follow the user head and play whispering sounds 4.8 seconds after the launch. Sound by George Bullen.
- *Steps* : A GameObject that will move through the scene at 28.4 seconds. Sound by falcospizaetus.
- *MusicBox* : A GameObject that will follow a trajectory previously recorded in a csv file. Starts at 36 seconds. Sound by InspectorJ.

## Multi players Scenes

### WaitForConnection

### ISSNetworkScene


## Prefabs
All these prefabs are available in the *prefab* folder.

### MonoPlayer

### NetworkPlayer

### NetworkAudioSource

### TrackerPrefab


## Scripts

### Main utilities (ISS_Scripts)
These scripts are tools necessary to use Immersive Sound Space in a Unity scene. They are used in all the example scenes.

#### CommandLineParser
A simple script to call the compiled Unity application from the command line (and so from batch file) with arguments. It allows to specify :
- the serial number of the Tracker to follow for the main camera (Mono player) or for the Network player (Multi players)
- the port to listen for incomming OSC messages (Mono player only)

Other arguments can be implemented quite easily.  
To use it inside the editor, create an empty GameObject with the script attached and set the values from the editor Inspector.

#### Chrono
A utility that count time from the loading of the current scene. It is used to trigger audio play, trajectories and to get a timestamp on debug messages. It can be paused and resumed with `Chrono.Pause()` and `Chrono.Resume()`, and reset with `Chrono.ResetTimer()`.

#### MoveFromOSC
Script to attach to any object that must follow a HTC Vive tracker. It will parse the OSC messages comming from the Python script and move the object they are attached to accordingly. We recomend to add an offset of -0.15 on the Y axis for the MainCamera, so that it better follows the user's head.

### Loggers
The *logger* scripts are used to display information on the UI in the compiled application. Their goal is to substitute to the Debug.Log function from Unity. Each of them require a UI panel with a TextField. See the [LoggerScene](#loggerscene) for a use-case example.

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


### InteractiveSounds
These scripts are meant to be attached to the GameObject containing an AudioSource. They will give you the ability to chose at which time you want to play the sound, or will allow you to move the object inside the scene, at a specific time. Starting to move a GameObject will trigger the play of every AudioSources contained in itself. See the [InteractiveExampleScene](#interactiveexamplescene) for a use-case example. **All timing values are in seconds (float).**

#### FollowOtherObject
A GameObject with this script will follow another GameObject, with a certain offset in the GameObject coordinates system.  
Attached to the *Whisper* GameObject in the InteractiveExampleScene.

#### PlayOnRaycast
A simple script to attach to and object with boxcollider and audio source that will play the audio file if the main camera point at the object for a certain period of time.  
Attached to the *Seagull* GameObject in the InteractiveExampleScene.

#### PlayWithDelay
Delay the playback of all AudioSources attached to the same GameObject.  
Attached to the *Whisper* GameObject in the InteractiveExampleScene.

#### TrajectoryFromCheckpoints
Allow you to create a path for the GameObject, by giving a list of 3D checkpoints and the last point of the trajectory. The GameObject will always be oriented toward the next point. The start time and the duration of the trajectory can be adjusted.  
Attached to the *Steps* GameObject in the InteractiveExampleScene.

#### TrajectoryFromFile
A script to play back a Tracker trajectory .csv file created with [this tool](https://github.com/numediart/ISS_Utils/tree/master/SaveTrackerTrajectory). If the *Loop* option is activated, the trajectory will be played over a and over, but the sound will be played once only. If the *Ping Pong* option is activated, the trajectory will be played alternatively forward and backward.  
Attached to the *MusicBox* GameObject in the InteractiveExampleScene.

### Network Scripts

#### MirrorAutoconnect

#### PlayerManager

#### PlayerSetup

#### PlayerTrackerIdSender

#### ViveTrackerSerialNetwork

#### OSCManager

#### TrackersManagerOSC
