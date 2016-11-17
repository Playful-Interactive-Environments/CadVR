//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.PoseTracker;
using UnityEngine;
using Valve.VR;

namespace HTC.UnityPlugin.Vive
{
    /// <summary>
    /// To provide static APIs to retrieve devices' tracking status
    /// </summary>
    public static partial class VivePose
    {
        /// <summary>
        /// Returns true if input focus captured by current process
        /// Usually the process losses focus when player switch to deshboard by clicking Steam button
        /// </summary>
        public static bool HasFocus() { return hasFocus; }

        /// <summary>
        /// Returns true if the process has focus and the device identified by role is connected / has tracking
        /// </summary>
        public static bool IsValid(HandRole role) { return IsValid(role.ToDeviceRole()); }

        /// <summary>
        /// Returns true if the process has focus and the device identified by role is connected / has tracking
        /// </summary>
        public static bool IsValid(DeviceRole role)
        {
            var index = ViveRole.GetDeviceIndex(role);
            return index < poses.Length && poses[index].bDeviceIsConnected && poses[index].bPoseIsValid && hasFocus;
        }

        /// <summary>
        /// Returns true if the device identified by role is connected.
        /// </summary>
        public static bool IsConnected(HandRole role) { return IsConnected(role.ToDeviceRole()); }

        /// <summary>
        /// Returns true if the device identified by role is connected.
        /// </summary>
        public static bool IsConnected(DeviceRole role)
        {
            var index = ViveRole.GetDeviceIndex(role);
            return index < poses.Length && poses[index].bDeviceIsConnected;
        }

        /// <summary>
        /// Returns true if tracking data of the device identified by role has valid value.
        /// </summary>
        public static bool HasTracking(HandRole role) { return HasTracking(role.ToDeviceRole()); }

        /// <summary>
        /// Returns true if tracking data of the device identified by role has valid value.
        /// </summary>
        public static bool HasTracking(DeviceRole role)
        {
            var index = ViveRole.GetDeviceIndex(role);
            return index < poses.Length && poses[index].bPoseIsValid;
        }

        public static bool IsOutOfRange(HandRole role) { return IsOutOfRange(role.ToDeviceRole()); }

        public static bool IsOutOfRange(DeviceRole role)
        {
            var index = ViveRole.GetDeviceIndex(role);
            return index < poses.Length && (poses[index].eTrackingResult == ETrackingResult.Running_OutOfRange || poses[index].eTrackingResult == ETrackingResult.Calibrating_OutOfRange);
        }

        public static bool IsCalibrating(HandRole role) { return IsCalibrating(role.ToDeviceRole()); }

        public static bool IsCalibrating(DeviceRole role)
        {
            var index = ViveRole.GetDeviceIndex(role);
            return index < poses.Length && (poses[index].eTrackingResult == ETrackingResult.Calibrating_InProgress || poses[index].eTrackingResult == ETrackingResult.Calibrating_OutOfRange);
        }

        public static bool IsUninitialized(HandRole role) { return IsUninitialized(role.ToDeviceRole()); }

        public static bool IsUninitialized(DeviceRole role)
        {
            var index = ViveRole.GetDeviceIndex(role);
            return index < poses.Length && poses[index].eTrackingResult == ETrackingResult.Uninitialized;
        }

        public static Vector3 GetVelocity(HandRole role, Transform origin = null) { return GetVelocity(role.ToDeviceRole(), origin); }

        public static Vector3 GetVelocity(DeviceRole role, Transform origin = null)
        {
            var index = ViveRole.GetDeviceIndex(role);
            var rawValue = Vector3.zero;

            if (index < poses.Length)
            {
                rawValue = new Vector3(poses[index].vVelocity.v0, poses[index].vVelocity.v1, -poses[index].vVelocity.v2);
            }

            return origin == null ? rawValue : origin.TransformVector(rawValue);
        }

        public static Vector3 GetAngularVelocity(HandRole role, Transform origin = null) { return GetAngularVelocity(role.ToDeviceRole(), origin); }

        public static Vector3 GetAngularVelocity(DeviceRole role, Transform origin = null)
        {
            var index = ViveRole.GetDeviceIndex(role);
            var rawValue = Vector3.zero;

            if (index < poses.Length)
            {
                rawValue = new Vector3(-poses[index].vAngularVelocity.v0, -poses[index].vAngularVelocity.v1, poses[index].vAngularVelocity.v2);
            }

            return origin == null ? rawValue : origin.TransformVector(rawValue);
        }

        /// <summary>
        /// Returns tracking pose of the device identified by role
        /// </summary>
        public static Pose GetPose(HandRole role, Transform origin = null) { return GetPose(role.ToDeviceRole(), origin); }

        /// <summary>
        /// Returns tracking pose of the device identified by role
        /// </summary>
        public static Pose GetPose(DeviceRole role, Transform origin = null)
        {
            var index = ViveRole.GetDeviceIndex(role);
            var rawPose = new Pose();

            if (index < rigidPoses.Length) { rawPose = rigidPoses[index]; }

            if (origin != null)
            {
                rawPose = new Pose(origin) * rawPose;
                rawPose.pos.Scale(origin.localScale);
            }

            return rawPose;
        }

        /// <summary>
        /// Set target pose to tracking pose of the device identified by role relative to the origin
        /// </summary>
        public static void SetPose(Transform target, HandRole role, Transform origin = null) { SetPose(target, role.ToDeviceRole(), origin); }

        /// <summary>
        /// Set target pose to tracking pose of the device identified by role relative to the origin
        /// </summary>
        public static void SetPose(Transform target, DeviceRole role, Transform origin = null) { Pose.SetPose(target, GetPose(role), origin); }
    }
}