//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    [DisallowMultipleComponent]
    public abstract class BaseMultiMethodRaycaster : BaseRaycaster
    {
        private readonly IndexedSet<IRaycastMethod> methods = new IndexedSet<IRaycastMethod>();

        [NonSerialized]
        public readonly List<RaycastResult> sortedRaycastResults = new List<RaycastResult>();

        public bool showDebugRay = true;
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            if (GetComponent<PhysicsRaycastMethod>() == null) { gameObject.AddComponent<PhysicsRaycastMethod>(); }
            if (GetComponent<CanvasRaycastMethod>() == null) { gameObject.AddComponent<CanvasRaycastMethod>(); }
        }
#endif
        protected virtual Comparison<RaycastResult> GetRaycasterResultComparer()
        {
            return Pointer3DInputModule.defaultRaycastComparer;
        }

        public RaycastResult FirstRaycastResult()
        {
            for (int i = 0, imax = sortedRaycastResults.Count; i < imax; ++i)
            {
                if (sortedRaycastResults[i].gameObject == null) { continue; }
                return sortedRaycastResults[i];
            }
            return new RaycastResult();
        }

        public void AddRaycastMethod(IRaycastMethod obj)
        {
            methods.AddUnique(obj);
        }

        public void RemoveRaycastMethod(IRaycastMethod obj)
        {
            methods.Remove(obj);
        }

        // should do raycast and store results in SortedRaycastResults
        public virtual void Raycast(Vector2 screenPosition)
        {
            sortedRaycastResults.Clear();
            if (eventCamera == null) { return; }

            for (var i = methods.Count - 1; i >= 0; --i)
            {
                var method = methods[i];
                if (!method.enabled) { continue; }
                method.Raycast(this, screenPosition, eventCamera, sortedRaycastResults);
            }

            var comparer = GetRaycasterResultComparer();
            if (comparer != null)
            {
                sortedRaycastResults.Sort(comparer);
            }
#if UNITY_EDITOR
            if (showDebugRay)
            {
                var ray = eventCamera.ScreenPointToRay(screenPosition);
                if (sortedRaycastResults.Count > 0)
                {
                    Debug.DrawRay(ray.origin, ray.direction * sortedRaycastResults[0].distance, Color.green);
                }
                else if (isActiveAndEnabled)
                {
                    Debug.DrawRay(ray.origin, ray.direction * (eventCamera.farClipPlane - eventCamera.nearClipPlane), Color.red);
                }
            }
#endif
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            sortedRaycastResults.Clear();
            if (eventCamera == null) { return; }

            for (var i = methods.Count - 1; i >= 0; --i)
            {
                var method = methods[i];
                if (!method.enabled) { continue; }
                method.Raycast(this, eventData.position, eventCamera, sortedRaycastResults);
            }

            var comparer = GetRaycasterResultComparer();
            if (comparer != null)
            {
                sortedRaycastResults.Sort(comparer);
            }

            for (int i = 0, imax = sortedRaycastResults.Count; i < imax; ++i)
            {
                resultAppendList.Add(sortedRaycastResults[i]);
            }
        }
    }
}