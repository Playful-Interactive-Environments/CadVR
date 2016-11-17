//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    [RequireComponent(typeof(BaseMultiMethodRaycaster))]
    public abstract class BaseRaycastMethod : MonoBehaviour, IRaycastMethod
    {
        public BaseMultiMethodRaycaster raycaster { get; private set; }

        protected virtual void Awake()
        {
            raycaster = GetComponent<BaseMultiMethodRaycaster>();
            raycaster.AddRaycastMethod(this);
        }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void OnDestroy()
        {
            raycaster.RemoveRaycastMethod(this);
        }

        public abstract void Raycast(BaseRaycaster module, Vector2 position, Camera eventCamera, List<RaycastResult> raycastResults);
    }
}