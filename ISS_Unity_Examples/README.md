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
Both scenes must be implemented for using ISS with several interacting users. To be able to test your scene, you will need to build it (do not forget to add both scenes to the build), and launch it as a client from the command line with the parameter `--tracker LHR-XXXXXXXX` (Fill in the tracker serial with you own values). Then, you can launch the server by simply pressing the *run* button on the Unity Editor.

If you find it more convenient (for Debug.Log messages) that both the client AND the server are launched from the Unity Editor, you can use [ParrelSync](https://github.com/VeriorPies/ParrelSync).

### WaitForConnection
This is the offline scene, displayed to the clients while not connected to the server. It should not be displayed for more than a second if the server is running on localhost before you launch the client.
It contains essentially the [NetworkManager](https://mirror-networking.gitbook.io/docs/components/network-manager), the [MirrorAutoconnect](#mirrorautoconnect) script and the [CommandLineParser](#commandlineparser).

All these element must stay in that scene (except MirrorAutoconnect, if you handle the connection to the server by another way, for example using [NetworkManagerHUD](https://mirror-networking.gitbook.io/docs/components/network-manager-hud)).

Note that NetworkManager and CommandLineParser will not be destroyed when loading the NetworkScene.

You can clone this scene and customize it to fit your needs.

### ISSNetworkScene
The scene in which you will run your experience scenario.  
All the assets children of the *ServerAssets* GameObject will be enabled only on the server, as long as they are referenced in the *Children To Activate On Server* array in the *ServerSetup* script.

Basically, the *CameraManager* allows to switch between clients cam and a FPS like camera on the server, the *OSCManager* opens the OSC server for receiving OSC messages (check the receiving port matches the one used by the python tracking script), the *TrackersManager* keeps track of currently tracked trackers and the *PlayerManager* sets the position and rotation of each player in its own instance.


## Scripts

### Main utilities (ISS_Scripts)
These scripts are tools necessary to use Immersive Sound Space in a Unity scene. They are used in all the example scenes.

#### CommandLineParser
(Singleton)  
A simple script to call the compiled Unity application from the command line (and so from batch file) with arguments. It allows to specify :
- the serial number of the Tracker to follow for the main camera (Mono player) or for the Network player (Multi players)
- the port to listen for incomming OSC messages (Mono player only)

Other arguments can be implemented quite easily.  
To use it inside the editor, create an empty GameObject with the script attached and set the values from the editor Inspector.

#### OSCManagerMonoPlayer
Creates a single OSC server called *MoveObject* on the port defined with the `-p` or `--port` command line argument. The OSC messages will be forwarded to all the MoveFromOSC scripts in the scene.

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
Script attached to MirrorAutoConnect GameObject in WaitForConnection scene.  
Will force the instance as server if command line argument `-s` or `--server` is set.  
Else, switch to client mode and tries to connect to server at startup and retries every 12 seconds while attemps fail.  
Display a connection message on a Canvas.

#### ServerSetup
(Only on server)  
Enables or disables children and components if the instance is server.
Manages OSC server creation and links it to TrackersManagers.

#### OSCManager
(Only on server)  
Creates (when requested by ServerSetup) and updates OSC servers that will listen for incomming OSC messages. For each server, a different OSC port and identifying string must be specified.

#### TrackersManagerOSC
(Only on server)  
Attaches to one server created by the OSCManager. Keeps a list of currently tracked Vive trackers and update their position / rotation. Destroy a tracker if no OSC message with its serial is received for 2 seconds.

#### PlayerManager
(Singleton)  
Keeps a list of all clients connected to the server, matches Tracker serials and clients and set the position / rotation of each player.

#### PlayerSetup
(Only on clients)  
Enables or disables children and components if the instance is local client.  
Attached to the NetworkPlayer prefab.

#### PlayerTrackerIdSender
(Only on clients)  
On connection, retrieves the tracker serial to follow from the CommandLineParser and send it to the PlayerManager on the server.
Attached to the NetworkPlayer prefab.

#### ViveTrackerSerialNetwork
Contains only a [syncvar](https://mirror-networking.gitbook.io/docs/guides/synchronization/syncvars) for the serial number of the tracker, so that every instances of the game can identify it.  
Attached to the TrackerPrefab prefab.


## Prefabs
All these prefabs are available in the *prefab* folder.

### MonoPlayer (No network, OSC only)
A simple representation of the head of a player, with camera, AudioListener and ResonnanceAudioListener components. It also listens incomming OSC messages and apply the position / rotation if the OSC Message Vive Tracker serial matches the one used with command line at launch of the build instance.

### NetworkPlayer
Same 3D representation of the head as for Mono Player prefab. Also have camera, AudioListener and ResonnanceAudioListener but they are disabled at start and will be enabled on local client only. This type of player is positionned by the PlayerManager script on the server, therefore, **it does not have client authority over its NetworkTransform**.

### EmptyPrefab
A prefab that contains... nothing. It's just used by the TrackersManager (only for trackers corresponding to clients, in other words, trackers mounted on headset) to avoid null reference exception.

### TrackerPrefab
A prefab for trackers that represent interactive objects in the scene. It is spawned and its position / rotation is syncronized on every clients. It must have the "ViveTracker" tag and be referenced in NetworkManager registered spawnable prefabs **AND** in TrackersManagerOSC.

### NetworkAudioSource
An audio source that can be spawned on every clients and moved by the server.

