//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    public class Pointer3DEventData : PointerEventData
    {
        public Vector3 position3D;
        public Quaternion rotation;

        public Vector3 position3DDelta;
        public Quaternion rotationDelta;

        public Vector3 pressPosition3D;
        public Quaternion pressRotation;

        public float pressDistance;

        public Pointer3DEventData(EventSystem eventSystem) : base(eventSystem)
        {
            Pointer3DInputModule.AssignPointerId(this);
        }

        public virtual bool GetPress() { return false; }

        public virtual bool GetPressDown() { return false; }

        public virtual bool GetPressUp() { return false; }
    }
}