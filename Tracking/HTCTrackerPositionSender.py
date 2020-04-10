 # © - 2020 – UMONS - CLICK' Living Lab
 # 
 # Immersive Sound Space of University of MONS – CLICK' Living Lab (Fabien Grisard) is free software: 
 # you can redistribute it and/or modify it under the terms of the 3-Clause BSD licence. 
 # This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
 # without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 # See the 3-Clause BSD licence License for more details.
 # 
 # ​​​​​You should have received a copy of the 3-Clause BSD licence along with this program.  
 # 
 # Each use of this software must be attributed to University of MONS – CLICK' Living Lab  (Fabien Grisard).


# parse arguments from the command line
import argparse

# compute / set framerate
import time

# get keystrokes under Windows
import os
import msvcrt

# get data from the trackers
import openvr

# math utils to change axes origin
import numpy as np
import quaternion

# communication to other programs (OSC)
from pythonosc import udp_client, osc_message_builder

# load / save origin matrix
from pathlib import Path
import pickle




# get keystrokes under Windows
def kbfunc(): 
    x = msvcrt.kbhit()
    if x: 
        ret = ord(msvcrt.getch()) 
    else: 
        ret = 0 
    return ret



def vive_pose_to_numpy_matrix_4x4(vive_pos):
    m_4x4 = np.identity(4)
    for x in range(0,3):
        for y in range (0,4):
            m_4x4[x][y] = vive_pos[x][y]
    
    return m_4x4



def main():

    # arguments parsing
    parser = argparse.ArgumentParser()
    parser.add_argument("--listeners", default="127.0.0.1:9001", help="The ip:port of the OSC programs listening, different listeners must be separated with ';', if multiple ports are used on one ip, it possible to use this form ip1:port1-1;port1-2;port1-3;ip2:port2-1;port2-2...")
    parser.add_argument("--origin_serial", default="LHR-0EBE46E8", help="The serial number of the tracker used for origin calibration")
    parser.add_argument("--framerate", type=int, default=30, help="Expected framerate - used to slow down OSC messages as UnityOSC can't handle more than 50 messages per second")
    parser.add_argument("--opengl", default=False, action="store_true", help="Use openGL coordinate system if set, or Unity coordinate system if not set")
    parser.add_argument("--steam", default=False, action="store_true", help="Open SteamVR when the script is launched")
    args = parser.parse_args()
    
    
    # global program variables
    listeners = []     # array of OSC clients
    origin_serial = args.origin_serial
    origin_mat_inv = np.identity(4)
    # booleans for keystroke interactions
    to_unity_world = True # can be modified by --opengl option or 'u' key
    set_origin = False # change state with 'o' key
    escape = False # change state with 'escape' key
    # filter used to smooth framerate computation for display
    t0_list = []
    t0_list_max_size = 60
    # array of ids of currently tracked trackers (used to identify newly tracked trackers)
    trackers_list = []


    # if --opengl option is used, compute coordinates for opengl coordinate system
    # if not, compute for Unity coordinate system
    to_unity_world = not args.opengl
    coordsys = "Unity"
    if not to_unity_world :
        coordsys = "OpenGL"
    print("Coordinate system is set for {0}".format(coordsys))

    # parse the ip and ports from listeners and create OSC clients
    listeners_str = args.listeners.split(";")
    overall_ip = "127.0.0.1"
    for l in listeners_str :
        listener = l.split(":")
        port = -1
        if len(listener) == 2 :
            overall_ip = listener[0]
            port = int(listener[1])
        elif len(listener) == 1 :
            port = int(listener[0])
        ip = overall_ip
        print("Add OSC listener at {0}:{1}".format(ip, port))
        listeners.append(udp_client.SimpleUDPClient(ip, port))


    # load last origin matrix from origin_mat.rtm file, 
    # the file must be located in the same folder as this script
    origin_file_path = os.path.dirname(os.path.abspath(__file__))+"\origin_mat.rtm"
    #print("try to open origin file at {0}".format(origin_file_path))
    origin_file = Path(origin_file_path)
    if origin_file.exists():
        with open(origin_file_path, 'rb') as loaded_file:
            depickler = pickle.Unpickler(loaded_file)
            origin_mat_inv = depickler.load()
            print("origin loaded from file " + origin_file_path)
            #print(origin_mat_inv)
    else:
        print("origin file not found at " + origin_file_path + ", please place the reference and press 'o' key to set a new origin")

    # init OpenVR
    # VRApplication_Other will not open SteamVR
    # VRApplication_Scene will open SteamVR
    vrapplication_type = openvr.VRApplication_Other
    if args.steam:
        vrapplication_type = openvr.VRApplication_Scene
        print("open SteamVR")
    print("===========================")
    print("Initialize OpenVR", end='')
    try:
        openvr.init(vrapplication_type)
    except openvr.OpenVRError as e:
            print("Impossible to intialize OpenVR")
            print(e)
            exit(0)
    vrsystem = openvr.VRSystem()
    print(" ... OK")
    print("===========================")
    
    program_t0 = time.time()
    while(not escape):
    
        # keep track of loop time
        loop_t0 = time.time()
        t0_list.append(loop_t0)
        if len(t0_list) >= t0_list_max_size:
            del t0_list[0]
            
        # handle keyboard inputs
        key = kbfunc()
        if key != 0:
            if key == ord('o'):
                # set up origin
                set_origin = True
                print("\nset new origin : ")
            elif key == ord('r'):
                print("\nreset origin")
                origin_mat_inv = np.identity(4)
            elif key == ord('u'):
                to_unity_world = not to_unity_world
                print("\nto unity world = {0}".format(to_unity_world))
            elif key == 27: # escape
                escape = True
                openvr.shutdown()
        
        # get all devices currently tracked, it include HMD, controllers, lighthouses and trackers
        poses_t = openvr.TrackedDevicePose_t * openvr.k_unMaxTrackedDeviceCount
        tracked_devices = poses_t()
        tracked_devices = vrsystem.getDeviceToAbsoluteTrackingPose(
            openvr.TrackingUniverseStanding, 
            0, 
            len(tracked_devices))
        # array to store content that must be sent over OSC
        osc_content = []
        
        current_loop_trackers_list = []
        for _i, device in enumerate(tracked_devices):
            # broswe the array and keey only correctly tracked trackers
            if device.bPoseIsValid and vrsystem.getTrackedDeviceClass(_i) == openvr.TrackedDeviceClass_GenericTracker:
                tracker_id = vrsystem.getStringTrackedDeviceProperty(_i, openvr.Prop_SerialNumber_String)
                current_loop_trackers_list.append(tracker_id)
                # add tracker_id to list if not already in it
                if tracker_id not in trackers_list:
                    trackers_list.append(tracker_id)
                    print("\nNew tracker found : {}".format(trackers_list[-1]))
                # compute relative position (vector3) and rotation(quaternion) from 3x4 openvr matrix
                m_4x4 = vive_pose_to_numpy_matrix_4x4(device.mDeviceToAbsoluteTracking)
                m_corrected = np.matmul(origin_mat_inv, m_4x4)
                m_rot = np.identity(3);
                for x in range(0,3):
                    for y in range (0,3):
                        m_rot[x][y] = m_corrected[x][y]
                quat = quaternion.from_rotation_matrix(m_rot)
                # append computed pos/rot to the list in opengl or unity coordinate system
                content = [tracker_id, m_corrected[0][3], m_corrected[1][3], m_corrected[2][3], quat.x, quat.y, quat.z, quat.w]
                if to_unity_world:
                    content = [tracker_id, -m_corrected[0][3], -m_corrected[2][3], m_corrected[1][3], quat.x, quat.z, -quat.y, quat.w]
                osc_content.append(content)
                # set new origin if requested
                if vrsystem.getStringTrackedDeviceProperty(_i, openvr.Prop_SerialNumber_String) == origin_serial and set_origin:
                    set_origin = False
                    origin_mat_inv = np.linalg.inv(m_4x4)
                    with open(origin_file_path, 'wb') as saved_file:
                        pickler = pickle.Pickler(saved_file)
                        pickler.dump(origin_mat_inv)
                    print(m_4x4)
        # remove trackers that are not tracked any more
        for t in trackers_list:
            if t not in current_loop_trackers_list:
                print("\n\tTracker lost : {}".format(t))
                trackers_list.remove(t)
        
        # send osc content to all listeners if needed
        for c in osc_content:
            for l in listeners:
                l.send_message("/iss/tracker", c)
       
        #calulate fps
        fps = 0.0
        if len(t0_list) > 1:
           fps = len(t0_list) / (t0_list[-1] - t0_list[0])
        # update state display
        print("\rtime : {0:8.1f}, fps : {1:4.1f}, nb trackers : {2}        ".format(loop_t0 - program_t0, fps, len(osc_content)), end="") 
            
        # ensure fps is respected to avoid OSC messages queue overflow
        time.sleep(max(1.0 / args.framerate - (time.time() - loop_t0), 0))




if __name__ == "__main__":
    main()
