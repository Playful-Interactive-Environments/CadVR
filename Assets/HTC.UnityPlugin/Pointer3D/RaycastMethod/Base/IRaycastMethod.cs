//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    public interface IRaycastMethod
    {
        bool enabled { get; }
        void Raycast(BaseRaycaster module, Vector2 position, Camera eventCamera, List<RaycastResult> raycastResults);
    }
}