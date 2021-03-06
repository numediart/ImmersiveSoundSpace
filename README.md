# Immersive Sound Space
A bunch of scripts to follow wireless headphones in 3D space (based on HTC Vive trackers) and compute spacialized sound on Unity

The cornerstone of this system is the HTC Tracker Position Sender Script in the Tracking folder. It is written in python and was made possible mainly thanks to [python openvr](https://github.com/cmbruns/pyopenvr) and [python-osc](https://github.com/attwad/python-osc). More instructions for the installation can be found in the folder [Readme](Tracking/README.md).

Some helpful tools to be used alongside this script can be found in this [other repo](https://github.com/numediart/ISS_Utils).

In the UnityExample folder, you will find Unity scenes and C# scripts to listen the positions of the trackers and interact with the GameObjects in the scenes. See the folder [Readme](ISS_UnityExamples/Readme.md) for more information.

At the root of the repo, you will find two batch (*.bat) scripts.
To see how they work, please check the Tracking folder Readme for [calibration](Tracking/README.md#calibration) or [launch](Tracking/README.md#use-with-multiple-unity-build-instances).


## License
© - 2020 – UMONS - CLICK' Living Lab

Immersive Sound Space of University of MONS – CLICK' Living Lab (Fabien Grisard) is free software: 
you can redistribute it and/or modify it under the terms of the 3-Clause BSD licence. 
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the 3-Clause BSD licence License for more details.
 
​​​​​You should have received a copy of the 3-Clause BSD licence along with this program.  
 
Each use of this software must be attributed to University of MONS – CLICK' Living Lab  (Fabien Grisard).

## Legal Notices
This work was produced as part of the FEDER Digistorm project, co-financed by the European Union and the Wallonia Region.

![Logo FEDER-FSE](https://www.enmieux.be/sites/default/files/assets/media-files/signatures/vignette_FEDER%2Bwallonie.png)
