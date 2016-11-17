//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.PoseTracker;
using Valve.VR;

namespace HTC.UnityPlugin.Vive
{
    public interface INewPoseListener
    {
        void BeforeNewPoses();
        void OnNewPoses();
        void AfterNewPoses();
    }
    
    /// <summary>
    /// To manage all NewPoseListeners
    /// </summary>
    public static partial class VivePose
    {
        private static bool hasFocus;

        private static readonly Pose[] rigidPoses = new Pose[OpenVR.k_unMaxTrackedDeviceCount];
        private static TrackedDevicePose_t[] poses;

        private static IndexedSet<INewPoseListener> listeners = new IndexedSet<INewPoseListener>();

        static VivePose()
        {
            var system = OpenVR.System;
            if (system != null)
            {
                OnInputFocus(!system.IsInputFocusCapturedByAnotherProcess());
            }
            else
            {
                OnInputFocus(true);
            }

            if (SteamVR_Render.instance != null)
            {
                OnNewPoses(SteamVR_Render.instance.poses);
            }
            else
            {
                OnNewPoses(new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount]);
            }

            SteamVR_Utils.Event.Listen("input_focus", OnInputFocus);
            SteamVR_Utils.Event.Listen("new_poses", OnNewPoses);
        }

        public static bool AddNewPosesListener(INewPoseListener listener)
        {
            return listeners.AddUnique(listener);
        }

        public static bool RemoveNewPosesListener(INewPoseListener listener)
        {
            return listeners.Remove(listener);
        }

        private static void OnInputFocus(params object[] args)
        {
            hasFocus = (bool)args[0];
        }

        private static void OnNewPoses(params object[] args)
        {
            var tempListeners = ListPool<INewPoseListener>.Get();
            tempListeners.AddRange(listeners);

            for (int i = listeners.Count - 1; i >= 0; --i)
            {
                tempListeners[i].BeforeNewPoses();
            }

            poses = (TrackedDevicePose_t[])args[0];
            for (int i = poses.Length - 1; i >= 0; --i)
            {
                if (!poses[i].bDeviceIsConnected || !poses[i].bPoseIsValid) { continue; }
                var steamPose = new SteamVR_Utils.RigidTransform(poses[i].mDeviceToAbsoluteTracking);
                rigidPoses[i].pos = steamPose.pos;
                rigidPoses[i].rot = steamPose.rot;
            }

            for (int i = listeners.Count - 1; i >= 0; --i)
            {
                tempListeners[i].OnNewPoses();
            }

            for (int i = listeners.Count - 1; i >= 0; --i)
            {
                tempListeners[i].AfterNewPoses();
            }

            ListPool<INewPoseListener>.Release(tempListeners);
        }
    }
}