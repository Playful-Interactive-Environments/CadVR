//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    [AddComponentMenu("HTC/Pointer3D/Physics Raycast Method")]
    public class PhysicsRaycastMethod : BaseRaycastMethod
    {
        public enum MaskTypeEnum
        {
            Inclusive,
            Exclusive,
        }

        private static readonly RaycastHit[] hits = new RaycastHit[64];

        public MaskTypeEnum maskType;
        public LayerMask mask;

        public int RaycastMask { get { return maskType == MaskTypeEnum.Inclusive ? (int)mask : ~mask; } }
#if UNITY_EDITOR
        protected virtual void Reset()
        {
            maskType = MaskTypeEnum.Exclusive;
            mask = LayerMask.GetMask("Ignore Raycast");
        }
#endif
        public override void Raycast(BaseRaycaster module, Vector2 position, Camera eventCamera, List<RaycastResult> raycastResults)
        {
            var ray = eventCamera.ScreenPointToRay(position);
            var distance = eventCamera.farClipPlane - eventCamera.nearClipPlane;
            var hitCount = Physics.RaycastNonAlloc(ray, hits, distance, RaycastMask);

            for (int i = 0; i < hitCount; ++i)
            {
                raycastResults.Add(new RaycastResult
                {
                    gameObject = hits[i].collider.gameObject,
                    module = module,
                    distance = hits[i].distance,
                    worldPosition = hits[i].point,
                    worldNormal = hits[i].normal,
                    screenPosition = position,
                    index = raycastResults.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                });
            }
        }
    }
}