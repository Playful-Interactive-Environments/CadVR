//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.PoseTracker;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HTC.UnityPlugin.Vive
{
    [AddComponentMenu("HTC/Vive/Vive Pose Tracker")]
    // Simple component to track Vive devices.
    public class VivePoseTracker : BasePoseTracker, INewPoseListener
    {
        [Serializable]
        public class IsValidChangedEvent : UnityEvent<bool> { }

        private bool isValid;

        public Transform origin;
        public DeviceRole role = DeviceRole.Hmd;
        public IsValidChangedEvent onIsValidChanged;

        protected virtual void Start()
        {
            isValid = VivePose.IsValid(role);
            onIsValidChanged.Invoke(isValid);
        }

        protected virtual void OnEnable()
        {
            VivePose.AddNewPosesListener(this);
        }

        protected virtual void OnDisable()
        {
            VivePose.RemoveNewPosesListener(this);
        }

        public virtual void BeforeNewPoses() { }

        public virtual void OnNewPoses()
        {
            TrackPose(VivePose.GetPose(role), origin);

            var isValidCurrent = VivePose.IsValid(role);
            if (isValid != isValidCurrent)
            {
                isValid = isValidCurrent;
                onIsValidChanged.Invoke(isValid);
            }
        }

        public virtual void AfterNewPoses() { }
    }
}