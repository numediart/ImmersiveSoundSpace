## Requirements

- Windows 10, should work on earlier versions too
- [Steam](https://store.steampowered.com/about/)
- [SteamVR](https://store.steampowered.com/app/250820/SteamVR/)
- At least one [HTC Vive Tracker](https://www.vive.com/fr/vive-tracker/) already [paired](https://www.vive.com/us/support/wireless-tracker/category_howto/pairing-vive-tracker.html)
- Two or more HTC Vive Base stations [1.0](https://www.vive.com/fr/accessory/base-station/) or [2.0](https://www.vive.com/fr/accessory/base-station2/) compatible with your trackers
- If you don't want to use the Head Mounted Display (HMD) see the following section

## Use SteamVR without HMD
- Close SteamVR
- Open the file *SteamDirectory\config\steamvr.vrsettings* with a text editor 
    - (*SteamDirectory* is usually *C:\Program Files (x86)\Steam*)
- In the *"steamvr"* section, add the folowing entry: `"requireHmd" : false,`
    - Be careful with the comma, there must be one at the end of each line in the section, except for the last line
    - If needed, check your file with a [JSON validator](https://jsonlint.com/)
- Restart SteamVR


## Installation

- Install [Anaconda](https://www.anaconda.com/distribution/#download-section) with python 3.X
- [Open a command prompt window](https://www.thewindowsclub.com/how-to-open-command-prompt-from-right-click-menu)
- Create a conda environment named iss with pip support:
`conda create -n iss pip`
- Then activate the newly created environment : `activate iss`
- Install required dependancies :
```
pip install openvr
pip install python-osc
pip install numpy-quaternion
pip install numba
pip install scipy
```
- Deactivate the environment : `conda deactivate`
- cd to the 'calibrate.bat' folder
- Try to launch the calibration script : `calibrate.bat`

