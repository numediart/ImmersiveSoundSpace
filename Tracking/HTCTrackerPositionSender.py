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
import cv2

# communication to other programs (OSC)
from pythonosc import udp_client, osc_message_builder, osc_bundle_builder

# load / save origin matrix
from pathlib import Path
import pickle

# load real world points coordinates for multi-points calibration
import json




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
    parser.add_argument("--origin_serial", default=None, help="The serial number of the tracker used for origin calibration")
    parser.add_argument("--real_world_points", default=None, help="The JSON file containing list of points to use for origin calibration")
    parser.add_argument("--framerate", type=int, default=60, help="Expected framerate - used to slow down OSC messages if OSC listener can't handle the framerate")
    parser.add_argument("--openvr_coordinates", default=False, action="store_true", help="Use openVR native coordinate system if set, or Unity coordinate system if not set")
    parser.add_argument("--steam", default=False, action="store_true", help="Open SteamVR when the script is launched")
    parser.add_argument("--oscpattern", default="/iss/tracker", help="The OSC pattern used to send messages, default is '/iss/tracker'")
    parser.add_argument("--bundles", default=False, action="store_true", help="Send single frame messages as a OSC bundle")
    args = parser.parse_args()
    
    
    # global program variables
    listeners = []     # array of OSC clients
    origin_serial = args.origin_serial
    real_world_points = []
    real_world_points_array = []
    different_h = False # to check if all points are at the same height
    current_point_index = -1
    calib_points = []
    calib_mat = np.identity(4)
    # booleans for keystroke interactions
    to_unity_world = True # can be modified by --openvr_coordinates option or 'u' key
    set_origin = False # change state with 'o' key
    calib_next_point = False
    escape = False # change state with 'escape' key
    # filter used to smooth framerate computation for display
    t0_list = []
    t0_list_max_size = args.framerate / 2
    # array of ids of currently tracked trackers (used to identify newly tracked trackers)
    trackers_list = []

    print("======================================")
    print("")
    print("---------------------------------------")
    print("   Coordinate system and calibration   ")
    print("---------------------------------------")
    # if --openvr_coordinates option is used, compute coordinates for openvr coordinate system
    # if not, compute for Unity coordinate system
    to_unity_world = not args.openvr_coordinates
    coordsys = "Unity"
    if not to_unity_world :
        coordsys = "openVR"
    print("Coordinate system is set for {0}".format(coordsys))
    
    # load last origin matrix from origin_mat.rtm file, 
    # the file must be located in the same folder as this script
    origin_file_path = os.path.dirname(os.path.abspath(__file__))+"\origin_mat.rtm"
    #print("try to open origin file at {0}".format(origin_file_path))
    origin_file = Path(origin_file_path)
    if origin_file.exists():
        with open(origin_file_path, 'rb') as loaded_file:
            depickler = pickle.Unpickler(loaded_file)
            calib_mat = depickler.load()
            print("origin loaded from file " + origin_file_path)
            #print(calib_mat)
    else:
        print("origin file not found at " + origin_file_path + ", please place the reference and press 'o' key to set a new origin")

    if origin_serial:
        print("Calibration can be done with tracker serial " + origin_serial)
        if args.real_world_points:
            fp = open(args.real_world_points, 'r')
            real_world_points = json.load(fp)
            if len(real_world_points) < 4:
                real_world_points = None
                print("Calibration file must contain at least 4 points")
            else:
                print("Load real world points from JSON file for calibration")
                real_world_points_array = np.zeros((len(real_world_points), 3), np.float32)
                different_h = False
                last_h = None
                for i, pt in enumerate(real_world_points):         
                    if to_unity_world:
                        real_world_points_array[i][0] = -pt['x']
                        real_world_points_array[i][1] =  pt['z']
                        real_world_points_array[i][2] = -pt['y']
                    else:
                        real_world_points_array[i][0] =  pt['x']
                        real_world_points_array[i][1] =  pt['y']
                        real_world_points_array[i][2] =  pt['z']
                    # check if there is at least one point outside of floor plan
                    h = real_world_points_array[i][2]
                    if last_h is None:
                        last_h = h
                    if last_h != h:
                        different_h = True
                    last_h = h
                    
                    print("[{0}] : {1:3.2f}, {2:3.2f}, {3:3.2f}".format(pt['id'], 
                        real_world_points_array[i][0], 
                        real_world_points_array[i][1], 
                        real_world_points_array[i][2]))
                # if all points are at the same height, 
                # create a virtual point and append it to the array
                if not different_h:
                    print("All points are at the same height, creating a virtual point for calibration")
                    A = real_world_points_array[0]
                    B = real_world_points_array[1]
                    C = real_world_points_array[2]
                    virtual_point = A + np.cross(np.subtract(B, A), np.subtract(C, A))
                    real_world_points_array = np.vstack([real_world_points_array, virtual_point])
                    print("[{0}] : {1:3.2f}, {2:3.2f}, {3:3.2f}".format('virtual point', 
                        virtual_point[0], 
                        virtual_point[1], 
                        virtual_point[2]))
    
    
    print("")
    print("---------------------------------------")
    print("             OSC parameters            ")
    print("---------------------------------------")
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

    print("OSC pattern : " + args.oscpattern)
    
    send_bundles = "Send separate OSC messages for each tracker"
    if args.bundles:
        send_bundles = "Send simultaneous tracker OSC messages as bundles"
    print(send_bundles)


    print("")
    print("---------------------------------------")
    print("                 OpenVR                ")
    print("---------------------------------------")
    # init OpenVR
    # VRApplication_Other will not open SteamVR
    # VRApplication_Scene will open SteamVR
    vrapplication_type = openvr.VRApplication_Other
    if args.steam:
        vrapplication_type = openvr.VRApplication_Scene
        print("open SteamVR")
    print("Initialize OpenVR ... ", end='')
    try:
        openvr.init(vrapplication_type)
    except openvr.OpenVRError as e:
            print("Impossible to intialize OpenVR")
            print(e)
            exit(0)
    vrsystem = openvr.VRSystem()
    print("OK")
    print("======================================")
    print("")
    
    program_t0 = time.perf_counter()
    while(not escape):
    
        # keep track of loop time
        loop_t0 = time.perf_counter()
        t0_list.append(loop_t0)
        if len(t0_list) >= t0_list_max_size:
            del t0_list[0]
            
        # handle keyboard inputs
        key = kbfunc()
        if key != 0:
            if key == ord('o'):
                if origin_serial:
                    # set up origin
                    set_origin = True
                    print("\nset new origin : ")
                    if real_world_points:
                        current_point_index = -1
                        print("Multi-points calibration")
                        calib_next_point = True
                    else:
                        print("Single point calibration")
            elif key == ord('n'):
                calib_next_point = True
                
            elif key == ord('r'):
                print("\nreset origin")
                calib_mat = np.identity(4)
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
                m_corrected = np.matmul(calib_mat, m_4x4)
                
                m_rot = np.identity(3);
                for x in range(0,3):
                    for y in range (0,3):
                        m_rot[x][y] = m_corrected[x][y]
                quat = quaternion.from_rotation_matrix(m_rot)
                
                # append computed pos/rot to the list
                content = [tracker_id, m_corrected[0][3], m_corrected[1][3], m_corrected[2][3], quat.x, quat.y, quat.z, quat.w]
                if to_unity_world:
                    # switch and invert coordinates if Unity coordinate system is required
                    content = [tracker_id, -m_corrected[0][3], -m_corrected[2][3], m_corrected[1][3], quat.x, quat.z, -quat.y, quat.w]
                osc_content.append(content)
                
                # set new origin if requested
                if vrsystem.getStringTrackedDeviceProperty(_i, openvr.Prop_SerialNumber_String) == origin_serial and set_origin:
                    # perform multi-points calibration
                    if len(real_world_points) >= 4:
                        if calib_next_point:
                            if current_point_index < len(real_world_points) - 1:
                                print("Place your origin tracker on point")
                                print(real_world_points[current_point_index+1])
                                print("and press 'n'")
                            if current_point_index < 0:
                                pass
                            else:
                                openvr_pt = m_4x4[0:3,3]
                                calib_points.append(openvr_pt)
                            calib_next_point = False
                            current_point_index = current_point_index + 1
                            if current_point_index >= len(real_world_points):
                                print("Computing calibration with {} points".format(len(real_world_points)))
                                calib_points =  np.stack(calib_points)
                                if not different_h:
                                    print("All real world points are at same height, creating virtual point")
                                    A = calib_points[0]
                                    B = calib_points[1]
                                    C = calib_points[2]
                                    virtual_calib_point = A + np.cross(np.subtract(B, A), np.subtract(C, A))
                                    calib_points = np.vstack([calib_points, virtual_calib_point])
                                
                                retval, M, inliers = cv2.estimateAffine3D(calib_points, real_world_points_array)
                                calib_mat = np.vstack([M, [0, 0, 0, 1]])
                                with open(origin_file_path, 'wb') as saved_file:
                                    pickler = pickle.Pickler(saved_file)
                                    pickler.dump(calib_mat)
                                print(calib_mat.round(3))
                                set_origin = False
                            
                    # perform single point calibration (e.g, tracker position is origin, rotation matters)
                    else:
                        set_origin = False
                        calib_mat = np.linalg.inv(m_4x4)
                        with open(origin_file_path, 'wb') as saved_file:
                            pickler = pickle.Pickler(saved_file)
                            pickler.dump(calib_mat)
                        print(calib_mat.round(3))
        # remove trackers that are not tracked any more
        for t in trackers_list:
            if t not in current_loop_trackers_list:
                print("\n\tTracker lost : {}".format(t))
                trackers_list.remove(t)
        
        # send osc content to all listeners if needed
        for l in listeners:
            if args.bundles:
                bundle_builder = osc_bundle_builder.OscBundleBuilder(osc_bundle_builder.IMMEDIATELY)
                for c in osc_content:
                    msg = osc_message_builder.OscMessageBuilder(address=args.oscpattern)
                    for oscarg in c:
                        msg.add_arg(oscarg)
                    bundle_builder.add_content(msg.build())
                l.send(bundle_builder.build())
            else:
                for c in osc_content:
                    l.send_message(args.oscpattern, c)

       
        #calulate fps
        fps = 0
        if len(t0_list) > 1:
           fps = len(t0_list) / (t0_list[-1] - t0_list[0])
        # update state display
        if (not set_origin):
            print("\rtime : {0:8.1f}, fps : {1:4.1f}, nb trackers : {2}        ".format(loop_t0 - program_t0, fps, len(osc_content)), end="") 
        
        # ensure fps is respected to avoid OSC messages queue overflow
        while time.perf_counter() - loop_t0 <= 1.0 / args.framerate:
            pass




if __name__ == "__main__":
    main()
